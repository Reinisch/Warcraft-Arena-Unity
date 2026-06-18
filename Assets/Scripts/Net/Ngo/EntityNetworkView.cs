using Assets.Scripts.Core;
using Common;
using Core;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Net.Ngo
{
    /// <summary>
    /// Network shadow for a Core entity — a SEPARATE NetworkObject representing a Core <see cref="Unit"/>
    /// Server: linked to the authoritative unit via <see cref="Bind"/>; publishes its spawn snapshot.
    /// Client: recreates a Core unit from the replicated snapshot — but only once the client has its own map.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public sealed class EntityNetworkView : NetworkBehaviour
    {
        [SerializeField] private WorldEntityPrefab coreEntityPrefab;

        [Inject] private NgoEntityRegistry registry;
        [Inject] private World world;
        [Inject] private BalanceReference balance;
        [Inject] private EventBus eventBus;

        // Spawn-time snapshot used to recreate the unit on clients (read once at spawn). Kept as a reliable
        // NetworkVariable; it's the ONLY per-unit state in the connection-approval burst now, so the burst stays
        // small enough for many-unit scenarios to finish joining.
        private readonly NetworkVariable<NgoUnitSnapshot> state = new NetworkVariable<NgoUnitSnapshot>();

        // The connection this entity LOGICALLY belongs to (its controlling client, or the server for AI/NPCs),
        // captured at spawn and never touched by movement-control ownership flips. A client decides "is this MY
        // player" from this, not the live NetworkObject owner — so a player the server took movement control of
        // (e.g. an arena player rooted during warmup) is still recognised as local and keeps its camera/HUD.
        private readonly NetworkVariable<ulong> ownerClient = new NetworkVariable<ulong>();

        // Continuous state (transform + vitals + target + cast) is streamed UNRELIABLY at SendRateHz. RPCs only
        // reach a NetworkObject's observers, so map-scoped visibility still applies to this channel.
        private const float SendRateHz = 20f;
        private const float SendIntervalSeconds = 1f / SendRateHz;
        private float sendAccumulator;

        // Server: the latest transform submitted by the owning client (for client-owned units, e.g. a player).
        private NgoNetTransform submittedTransform;
        private bool hasSubmittedTransform;

        // Client: the latest snapshot received from the server, applied each frame (full-state, latest-wins).
        private NgoStateSnapshot latestSnapshot;
        private bool hasSnapshot;
        private int appliedCastSpellId;

        // Visible auras/buffs — server-authoritative, sent only when they change (not the per-tick duration
        // countdown, which the client ticks locally). A list since most units have few active auras.
        private readonly NetworkList<NgoAuraSlot> netAuras = new NetworkList<NgoAuraSlot>();
        private readonly NetAuraSlot[] auraBuffer = new NetAuraSlot[Unit.MaxVisibleAuraSlots];
        private readonly NetAuraSlot[] lastAuras = new NetAuraSlot[Unit.MaxVisibleAuraSlots];

        private Unit unit;
        private bool createdLocally;
        private bool waitingForMap;

        private bool IsLogicalOwner => NetworkManager != null && ownerClient.Value == NetworkManager.LocalClientId;

        public WorldEntityPrefab CoreEntityPrefab => coreEntityPrefab;

        /// <summary>
        /// Server: link the authoritative unit. Call BEFORE spawning — the snapshot is captured in
        /// <see cref="OnNetworkSpawn"/> (a NetworkVariable can't be written before spawn).
        /// </summary>
        public void Bind(Unit boundUnit)
        {
            unit = boundUnit;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Now that the NetworkVariable is initialised, publish the snapshot + register the id↔unit.
                // The shadow is shown to clients (NetworkShow) only after this, so they get the real snapshot.
                if (unit != null)
                {
                    state.Value = NgoUnitSnapshot.From(unit.CaptureState());
                    // OwnerClientId here is the spawn-time (intended) owner — OnNetworkSpawn runs synchronously
                    // inside Spawn, before any later root/polymorph re-owns the shadow to the server.
                    ownerClient.Value = OwnerClientId;
                    ApplyAuthority();

                    // Runtime faction/FFA changes (e.g. arena team assignment) re-publish the reliable snapshot
                    // so current observers AND late-revealed clients get the corrected value — without a per-tick
                    // send (the change is rare).
                    eventBus.RegisterEvent(unit, GameEvents.UnitFactionChanged, OnServerFactionChanged);
                }

                Register();
                return;
            }

            state.OnValueChanged += OnSnapshotChanged;
            netAuras.OnListChanged += OnAurasChanged;
            TryCreateClientUnit();
        }

        // Server: this unit's faction (or free-for-all) changed — re-publish the snapshot NetworkVariable.
        private void OnServerFactionChanged()
        {
            if (unit != null)
                state.Value = NgoUnitSnapshot.From(unit.CaptureState());
        }

        // Client: apply a replicated faction/FFA change once the unit exists (recolours nameplates/selection).
        private void OnSnapshotChanged(NgoUnitSnapshot previous, NgoUnitSnapshot current)
        {
            if (unit != null)
                unit.SetNetworkFaction(current.FactionId, current.FreeForAll);
        }

        // A unit runs its own physics only on its owner; everyone else puppets it from netTransform.
        protected override void OnOwnershipChanged(ulong previous, ulong current) => ApplyAuthority();

        private void ApplyAuthority()
        {
            if (unit != null)
                unit.SetNetworkControlled(!IsOwner);
        }

        
        private void Update()
        {
            if (unit == null || !IsSpawned)
                return;

            if (IsServer)
                ServerUpdate();
            else
                ClientUpdate();
        }

        // Server: apply any client-submitted transform to our puppet, maintain auras, and broadcast the full
        // continuous state to observing clients at SendRateHz (unreliable).
        private void ServerUpdate()
        {
            // Client-owned units (a remote player): drive our puppet from the client's submitted transform so
            // server-side logic + the host's view track it. Server-owned units (boss/AI/host player) run their
            // own motor.
            if (!IsOwner && hasSubmittedTransform)
                unit.SetNetworkTransform(submittedTransform.Position, submittedTransform.Rotation,
                    (MovementFlags)submittedTransform.MovementFlags);

            UpdateServerAuras();

            sendAccumulator += Time.deltaTime;
            if (sendAccumulator < SendIntervalSeconds)
                return;
            sendAccumulator = 0f;

            // Server-owned: author the transform from the unit. Client-owned: forward the latest transform the
            // owner submitted. Full movement flags either way — receivers use what they need.
            NgoNetTransform transform = IsOwner
                ? new NgoNetTransform(unit.Position, unit.Rotation, (int)unit.MovementFlags)
                : submittedTransform;

            SnapshotRpc(new NgoStateSnapshot
            {
                Transform = transform,
                Vitals = NgoUnitVitals.From(unit.CaptureVitals()),
                TargetId = unit.Target != null ? registry.GetId(unit.Target).Value : 0UL,
                Cast = unit.SpellCast.IsCasting
                    ? new NgoCastState { SpellId = unit.SpellCast.CastingSpellInfo.Id, CastTime = unit.SpellCast.CastTime }
                    : default,
            });
        }

        // Client: stream our owned unit's transform up to the server (unreliable, throttled), and apply the
        // latest server snapshot — vitals/cast for every unit, transform/target only for units we don't own
        // (the owner keeps its local authoritative transform + optimistic target).
        private void ClientUpdate()
        {
            if (IsOwner)
            {
                sendAccumulator += Time.deltaTime;
                if (sendAccumulator >= SendIntervalSeconds)
                {
                    sendAccumulator = 0f;
                    SubmitTransformRpc(new NgoNetTransform(unit.Position, unit.Rotation, (int)unit.MovementFlags));
                }
            }

            if (!hasSnapshot)
                return;

            unit.ApplyVitals(latestSnapshot.Vitals.To());
            ApplyClientCast(latestSnapshot.Cast);

            if (!IsOwner)
            {
                Unit target = latestSnapshot.TargetId != 0UL && registry.TryGet(new NetId(latestSnapshot.TargetId), out Unit t)
                    ? t : null;
                unit.SetNetworkTarget(target);

                NgoNetTransform transform = latestSnapshot.Transform;
                unit.SetNetworkTransform(transform.Position, transform.Rotation, (MovementFlags)transform.MovementFlags);
            }
        }

        // Owner client → server: latest authoritative transform of an owned unit. Unreliable + latest-wins.
        [Rpc(SendTo.Server, Delivery = RpcDelivery.Unreliable)]
        private void SubmitTransformRpc(NgoNetTransform transform)
        {
            submittedTransform = transform;
            hasSubmittedTransform = true;
        }

        // Server → observing clients: the full continuous state. Unreliable; only reaches this object's
        // observers, so map-bound visibility still applies.
        [Rpc(SendTo.NotServer, Delivery = RpcDelivery.Unreliable)]
        private void SnapshotRpc(NgoStateSnapshot snapshot)
        {
            latestSnapshot = snapshot;
            hasSnapshot = true;
        }

        // Client: drive the cast bar from replicated cast state. Apply only on change (start/stop); the
        // SpellCast ticks the bar down locally between updates.
        private void ApplyClientCast(NgoCastState cast)
        {
            int spellId = cast.SpellId;
            if (spellId == appliedCastSpellId)
                return;

            appliedCastSpellId = spellId;
            if (spellId != 0 && balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                unit.SetNetworkCast(spellInfo, cast.CastTime);
            else
                unit.ClearNetworkCast();
        }

        // Server: rebuild the replicated aura list only when auras actually change (ignoring the per-tick
        // duration countdown — the client ticks DurationLeft locally between updates).
        private void UpdateServerAuras()
        {
            unit.CaptureAuras(auraBuffer);

            bool changed = false;
            for (int i = 0; i < auraBuffer.Length; i++)
                if (auraBuffer[i].AuraId != lastAuras[i].AuraId ||
                    auraBuffer[i].DurationMax != lastAuras[i].DurationMax ||
                    auraBuffer[i].Charges != lastAuras[i].Charges)
                {
                    changed = true;
                    break;
                }

            if (!changed)
                return;

            netAuras.Clear();
            for (int i = 0; i < auraBuffer.Length; i++)
            {
                lastAuras[i] = auraBuffer[i];
                if (auraBuffer[i].HasAura)
                    netAuras.Add(new NgoAuraSlot
                    {
                        SlotIndex = i,
                        AuraId = auraBuffer[i].AuraId,
                        DurationMax = auraBuffer[i].DurationMax,
                        DurationLeft = auraBuffer[i].DurationLeft,
                        Charges = auraBuffer[i].Charges,
                    });
            }
        }

        private void OnAurasChanged(NetworkListEvent<NgoAuraSlot> _) => ApplyClientAuras();

        private void ApplyClientAuras()
        {
            if (unit == null)
                return;

            var slots = new NetAuraSlot[Unit.MaxVisibleAuraSlots];
            foreach (NgoAuraSlot aura in netAuras)
                if (aura.SlotIndex >= 0 && aura.SlotIndex < slots.Length)
                    slots[aura.SlotIndex] = new NetAuraSlot
                    {
                        AuraId = aura.AuraId,
                        DurationMax = aura.DurationMax,
                        DurationLeft = aura.DurationLeft,
                        Charges = aura.Charges,
                    };

            unit.ApplyNetworkAuras(slots);
        }

        public override void OnNetworkDespawn()
        {
            state.OnValueChanged -= OnSnapshotChanged; // client subscription (no-op on server)
            netAuras.OnListChanged -= OnAurasChanged;

            if (waitingForMap)
            {
                world.MapController.EventMapLoaded -= OnMapLoaded;
                waitingForMap = false;
            }

            if (unit != null)
            {
                eventBus.UnregisterEvent(unit, GameEvents.UnitFactionChanged, OnServerFactionChanged); // server subscription (no-op on client)
                registry?.Unregister(new NetId(NetworkObjectId), unit);

                // On a WHOLE-session shutdown (leave / lost connection) don't destroy here — GameSession tears
                // the world down via MapController.UnloadAllAsync, so destroying now would double-destroy.
                // Only a single-object despawn mid-session (server removed THIS unit) destroys it here.
                bool fullShutdown = NetworkManager.Singleton != null && NetworkManager.Singleton.ShutdownInProgress;
                if (createdLocally && !fullShutdown)
                    world.UnitManager.Destroy(unit);
            }

            unit = null;
            createdLocally = false;
        }

        private void TryCreateClientUnit()
        {
            Map map = world.MapController.PrimaryMap;
            if (map == null)
            {
                // No map yet — the client loads its own map; buffer until it exists.
                waitingForMap = true;
                world.MapController.EventMapLoaded += OnMapLoaded;
                return;
            }

            CreateClientUnit(map);
        }

        private void OnMapLoaded(Map map)
        {
            world.MapController.EventMapLoaded -= OnMapLoaded;
            waitingForMap = false;
            CreateClientUnit(map);
        }

        private void CreateClientUnit(Map map)
        {
            unit = world.SpawnFromState(state.Value.To(), coreEntityPrefab, map, asLocalPlayer: IsLogicalOwner);
            createdLocally = true;
            ApplyAuthority();
            ApplyClientAuras(); // auras already replicated before the unit materialised (buffering)
            Register();
        }

        private void Register() => registry?.Register(new NetId(NetworkObjectId), unit);
    }
}
