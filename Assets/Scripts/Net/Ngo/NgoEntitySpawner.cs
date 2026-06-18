using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Core;
using Common;
using Core;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Net.Ngo
{
    /// <summary>
    /// Server-side: mirrors each authoritative Core unit with a network shadow. Reacts to the
    /// <see cref="UnitManager"/> attach/detach events. Only the server spawns shadows,
    /// clients receive them and recreate units via <see cref="EntityNetworkView"/>.
    /// </summary>
    public sealed class NgoEntitySpawner : IDisposable, INetConnectionPlayers
    {
        private readonly UnitManager unitManager;
        private readonly NgoNetSettings settings;
        private readonly INetworkController controller;
        private readonly World world;
        private readonly DiContainer container;
        private readonly EventBus eventBus;
        private readonly BalanceReference balance;
        private readonly Dictionary<Unit, NetworkObject> shadows = new Dictionary<Unit, NetworkObject>();
        private readonly Dictionary<ulong, Player> playersByClient = new Dictionary<ulong, Player>();
        private readonly Dictionary<ulong, CancellationTokenSource> streamCts = new Dictionary<ulong, CancellationTokenSource>();

        // Units that attached while clients are already connected wait here to be revealed (next frame, then
        // rate-limited) instead of NetworkShow-ing synchronously inside Spawn (which NGO drops) or all-at-once
        // (which floods the transport for a bursty scenario). Drained by a single RevealPump.
        private readonly Queue<Unit> pendingReveal = new Queue<Unit>();
        private bool revealPumpRunning;

        // Cancels in-flight reveal/stream awaits when the spawner is torn down.
        private readonly CancellationTokenSource lifetimeCts = new CancellationTokenSource();

        // While set, the next shadow spawned (synchronously, by SpawnConnectionPlayer) is owned by this
        // client instead of the server — so it becomes that client's local PlayerManager.Player.
        private ulong? pendingOwnerClientId;

        // Joining clients don't get the whole world in the connection burst (which overflows the transport for
        // a many-unit scenario and stalls the join over Relay). Instead we let the client load its map, then
        // reveal units in small batches via NetworkShow.
        private const float MapLoadGraceSeconds = 1.0f;
        private const int ShowBatchSize = 4;
        private const int ShowBatchIntervalMs = 100;

        public NgoEntitySpawner(UnitManager unitManager, NgoNetSettings settings, INetworkController controller,
            World world, DiContainer container, EventBus eventBus, BalanceReference balance)
        {
            this.unitManager = unitManager;
            this.settings = settings;
            this.controller = controller;
            this.world = world;
            this.container = container;
            this.eventBus = eventBus;
            this.balance = balance;

            unitManager.EventEntityAttached += OnUnitAttached;
            unitManager.EventEntityDetach += OnUnitDetached;

            // PeerConnected is raised from NgoNetworkController.Start, by which point the NetworkManager
            // Singleton reliably exists (the spawner's own ctor runs at DI build, before that).
            controller.PeerConnected += OnPeerConnected;

            // A client leaving (or a failed/timed-out connection that still reached OnConnect) must have its
            // server-side player destroyed — otherwise the Core unit lingers, gets re-synced to every future
            // joiner, and bloats the spawn burst (making the NEXT join more likely to fail).
            controller.PeerDisconnected += OnPeerDisconnected;

            // Movement authority: when a connection player can't move itself (root/stun/polymorph), the
            // server re-owns its shadow so it drives the (blocked/forced) movement; the client follows.
            eventBus.RegisterEvent<Unit, bool>(GameEvents.ServerPlayerMovementControlChanged, OnMovementControlChanged);
        }

        private static NetworkManager Manager => NetworkManager.Singleton;

        // A client connected: show every already-spawned shadow to it (it missed their original spawn), then
        // spawn an authoritative player owned by that connection so it gets its own controllable unit.
        private void OnPeerConnected(NetId peer)
        {
            NetworkManager nm = Manager;
            if (nm == null || !nm.IsServer || peer.Value == nm.LocalClientId)
                return;

            // The client's own player (owned) is shown immediately; the rest of the world is revealed gradually
            // (after a grace period for map load) so the connection burst stays small.
            SpawnConnectionPlayer(peer.Value);
            StreamUnitsToClientAsync(peer.Value).Forget();
        }

        // Reveal the existing same-map units to a freshly-joined client in small batches, after letting it load
        // its map. This keeps the connection-approval payload tiny (only the owned player) and spreads the
        // spawns across several ticks so the transport queue never overflows — the cause of stalled joins in
        // many-unit scenarios over Relay.
        private async UniTaskVoid StreamUnitsToClientAsync(ulong clientId)
        {
            CancelStream(clientId);
            var cts = new CancellationTokenSource();
            streamCts[clientId] = cts;
            CancellationToken token = cts.Token;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(MapLoadGraceSeconds), cancellationToken: token);

                int inBatch = 0;
                foreach (var entry in new List<KeyValuePair<Unit, NetworkObject>>(shadows))
                {
                    token.ThrowIfCancellationRequested();
                    if (entry.Key.Map != ClientMap(clientId))
                        continue;

                    if (Show(entry.Value, clientId) && ++inBatch >= ShowBatchSize)
                    {
                        inBatch = 0;
                        await UniTask.Delay(ShowBatchIntervalMs, cancellationToken: token);
                    }
                }
            }
            catch (OperationCanceledException) { /* client disconnected mid-stream */ }
            finally
            {
                if (streamCts.TryGetValue(clientId, out CancellationTokenSource stored) && stored == cts)
                {
                    streamCts.Remove(clientId);
                    cts.Dispose();
                }
            }
        }

        private void CancelStream(ulong clientId)
        {
            if (!streamCts.TryGetValue(clientId, out CancellationTokenSource cts))
                return;

            streamCts.Remove(clientId);
            cts.Cancel();
            cts.Dispose();
        }

        // The map a client observes: its connection player's map, or the primary map before that player exists.
        // Used to keep replication map-bound — a client never sees units from a map it isn't on.
        // TODO(client-map-traversal): a remote client only ever loads the primary map it joined; additive maps
        // stay server-side. To let clients actually enter an additive map, replicate that map-load to the
        // client (LoadScenarioCommand for the additive map) and ensure its player's Map updates so this resolves
        // to the new map. Today this only guarantees the boundary is clean (no leakage), not traversal.
        private Map ClientMap(ulong clientId) =>
            playersByClient.TryGetValue(clientId, out Player player) && player != null
                ? player.Map
                : world.MapController.PrimaryMap;

        private void SpawnConnectionPlayer(ulong clientId)
        {
            Map map = world.MapController.PrimaryMap;
            if (map == null)
                return;

            WorldEntityPrefab playerPrefab = settings.PlayerShadowPrefab != null
                ? settings.PlayerShadowPrefab.GetComponent<EntityNetworkView>()?.CoreEntityPrefab
                : null;
            if (playerPrefab == null)
                return;

            // Spawn the client's player as the class it chose in its lobby (sent in the connection token),
            // sanitised to a playable class — an unknown/None value spawns a Mage rather than rejecting the join.
            ClientConnectionToken token = controller.GetConnectionToken(new NetId(clientId));
            ClassType playerClass = balance.ResolvePlayableClass(token?.PreferredClass ?? ClassType.Mage);

            // The Player created here triggers OnUnitAttached synchronously; the pending owner makes its
            // shadow owned by the connecting client.
            pendingOwnerClientId = clientId;
            try { world.SpawnConnectionPlayer(playerPrefab, map, playerClass); }
            finally { pendingOwnerClientId = null; }
        }

        private void OnUnitAttached(Unit unit)
        {
            NetworkManager nm = Manager;
            if (nm == null || !nm.IsServer)
                return;

            GameObject shadowPrefab = unit is Player ? settings.PlayerShadowPrefab : settings.CreatureShadowPrefab;
            if (shadowPrefab == null || shadowPrefab.GetComponent<NetworkObject>() == null)
                return;

            // CRITICAL ORDER: instantiate (un-spawned) → Bind (set the replicated snapshot) → spawn with the
            // final owner. NGO serializes the spawn payload (incl. the NetworkVariable snapshot + owner) AT
            // spawn time and sends it to observers/owner immediately, so Bind/ownership MUST be set first —
            // otherwise the client receives a default snapshot (Kind=Creature) and mis-creates its player.
            // Instantiated via the container so the shadow's [Inject] fields resolve on the server too.
            GameObject instance = UnityEngine.Object.Instantiate(shadowPrefab, unit.Position, unit.Rotation);
            container.InjectGameObject(instance);

            NetworkObject shadow = instance.GetComponent<NetworkObject>();
            instance.GetComponent<EntityNetworkView>().Bind(unit);

            // Don't auto-add observers at spawn or on client connect — otherwise a connecting client gets EVERY
            // same-map unit crammed into its connection burst (the many-unit join stall). We reveal via NetworkShow.
            // TODO(map-visibility): runtime map changes (cross-map teleport) need a manual NetworkShow/Hide at the
            // point of the change — visibility is otherwise only established when we reveal a unit.
            shadow.SpawnWithObservers = false;

            ulong owner = pendingOwnerClientId ?? NetworkManager.ServerClientId;
            if (owner == NetworkManager.ServerClientId)
                shadow.Spawn();
            else
                shadow.SpawnWithOwnership(owner);

            shadows[unit] = shadow;

            // With SpawnWithObservers off, NGO never adds the SERVER to a shadow's observer list (it only does
            // so for SpawnWithObservers=true). But movement-control ownership flips (root/polymorph re-own the
            // shadow to the server via ChangeOwnership) silently no-op if the target isn't an observer — so the
            // server must observe its own shadows.
            Show(shadow, NetworkManager.ServerClientId);

            // Track ONLY connection-controlled players (pendingOwnerClientId set) so the command router can
            // act on the sender's player. Server-spawned AI/NPC players (SpawnPlayerAI) and the host's own
            // player are NOT mapped here — they'd otherwise collide on the server client id; the host's own
            // player is resolved via PlayerManager.Player fallback.
            if (unit is Player player && pendingOwnerClientId.HasValue)
                playersByClient[pendingOwnerClientId.Value] = player;

            // Reveal to already-connected clients that share this unit's map — NOT synchronously here. A
            // NetworkShow issued in the same frame as Spawn() is dropped by NGO (the CreateObject is never
            // sent), so a unit that attaches while a client is already connected (e.g. arena bots spawned at
            // match start) would otherwise stay invisible to that client while still receiving its unreliable
            // state RPCs (→ "Deferred OnSpawn ... NetworkObject was not received"). The pump defers one frame
            // and then rate-limits, so a scenario that spawns a burst of units can't flood the transport.
            EnqueueReveal(unit);
        }

        // Queue a freshly-spawned unit for batched reveal and make sure the pump is running.
        private void EnqueueReveal(Unit unit)
        {
            pendingReveal.Enqueue(unit);
            if (!revealPumpRunning)
                RevealPumpAsync().Forget();
        }

        // Single drain loop for post-attach reveals. Waits one frame (a same-frame NetworkShow is dropped by
        // NGO), then shows queued units to every eligible client, pausing every ShowBatchSize reveals so a
        // burst of spawns spreads across ticks instead of overflowing the transport — the same budget the
        // join-time stream uses. Picks up units enqueued while it runs; exits (and is restartable) when drained.
        private async UniTaskVoid RevealPumpAsync()
        {
            revealPumpRunning = true;
            CancellationToken token = lifetimeCts.Token;
            try
            {
                await UniTask.NextFrame(token);

                int inBatch = 0;
                while (pendingReveal.Count > 0)
                {
                    NetworkManager nm = Manager;
                    if (nm == null || !nm.IsServer)
                        break;

                    Unit unit = pendingReveal.Dequeue();
                    // Despawned/detached before we got to it — its shadow is no longer tracked.
                    if (!shadows.TryGetValue(unit, out NetworkObject shadow) || shadow == null || !shadow.IsSpawned)
                        continue;

                    bool shown = false;
                    foreach (ulong clientId in nm.ConnectedClientsIds)
                        if (clientId != nm.LocalClientId && unit.Map == ClientMap(clientId))
                            shown |= Show(shadow, clientId);

                    if (shown && ++inBatch >= ShowBatchSize)
                    {
                        inBatch = 0;
                        await UniTask.Delay(ShowBatchIntervalMs, cancellationToken: token);
                    }
                }
            }
            catch (OperationCanceledException) { /* spawner torn down mid-reveal */ }
            finally { revealPumpRunning = false; }
        }

        private static bool Show(NetworkObject shadow, ulong clientId)
        {
            if (shadow == null || !shadow.IsSpawned || shadow.IsNetworkVisibleTo(clientId))
                return false;

            shadow.NetworkShow(clientId);
            return true;
        }

        // Movement authority follows ownership: revoke (re-own to server) when the connection player can't
        // freely move (root/stun/polymorph) so the server drives it; restore to the client when it can.
        private void OnMovementControlChanged(Unit unit, bool hasFreeMovement)
        {
            NetworkManager nm = Manager;
            if (nm == null || !nm.IsServer)
                return;

            if (!shadows.TryGetValue(unit, out NetworkObject shadow) || shadow == null || !shadow.IsSpawned)
                return;

            if (!TryGetOwningClient(unit, out ulong clientId))
                return; // only connection-controlled players have authority to flip

            ulong target = hasFreeMovement ? clientId : NetworkManager.ServerClientId;
            if (shadow.OwnerClientId != target)
                shadow.ChangeOwnership(target);
        }

        // Server: a client disconnected — destroy its connection player. Destroying the Core unit fires
        // OnUnitDetached, which despawns the shadow and drops the playersByClient mapping. NGO may already have
        // auto-despawned the (owner-owned) shadow; OnUnitDetached's IsSpawned guard makes that a no-op.
        private void OnPeerDisconnected(NetId peer, DisconnectReason reason)
        {
            NetworkManager nm = Manager;
            if (nm == null || !nm.IsServer)
                return;

            CancelStream(peer.Value); // stop revealing units to a client that's gone

            if (playersByClient.TryGetValue(peer.Value, out Player player) && player != null)
                world.UnitManager.Destroy(player);
        }

        private bool TryGetOwningClient(Unit unit, out ulong clientId)
        {
            foreach (var entry in playersByClient)
                if (entry.Value == unit)
                {
                    clientId = entry.Key;
                    return true;
                }

            clientId = 0;
            return false;
        }

        private void OnUnitDetached(Unit unit)
        {
            if (!shadows.TryGetValue(unit, out NetworkObject shadow))
                return;

            shadows.Remove(unit);
            // Remove by the player's original client id (the shadow's owner may currently be the server if
            // movement control was revoked).
            if (unit is Player && TryGetOwningClient(unit, out ulong clientId))
                playersByClient.Remove(clientId);

            if (shadow != null && shadow.IsSpawned)
                shadow.Despawn(true);
        }

        bool INetConnectionPlayers.TryGetPlayer(NetId connection, out Player player) =>
            playersByClient.TryGetValue(connection.Value, out player);

        bool INetConnectionPlayers.TryGetConnection(Unit player, out NetId connection)
        {
            foreach (var entry in playersByClient)
                if (entry.Value == player)
                {
                    connection = new NetId(entry.Key);
                    return true;
                }

            connection = NetId.None;
            return false;
        }

        public void Dispose()
        {
            unitManager.EventEntityAttached -= OnUnitAttached;
            unitManager.EventEntityDetach -= OnUnitDetached;
            controller.PeerConnected -= OnPeerConnected;
            controller.PeerDisconnected -= OnPeerDisconnected;
            eventBus.UnregisterEvent<Unit, bool>(GameEvents.ServerPlayerMovementControlChanged, OnMovementControlChanged);

            foreach (CancellationTokenSource cts in streamCts.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            streamCts.Clear();

            lifetimeCts.Cancel();
            lifetimeCts.Dispose();
            pendingReveal.Clear();
        }
    }
}
