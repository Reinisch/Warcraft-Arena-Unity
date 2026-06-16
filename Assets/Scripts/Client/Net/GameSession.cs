using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Core;
using Cysharp.Threading.Tasks;
using Net;
using UnityEngine;

namespace Client
{
    /// <summary>What kind of world session is currently live. Drives lobby action availability.</summary>
    public enum GameSessionState
    {
        /// <summary>No world loaded, networking stopped. The only state from which a new session may start.</summary>
        None,

        /// <summary>Local, non-advertised host — plays single-player but still runs the in-process net bus.</summary>
        SinglePlayer,

        /// <summary>Advertised host (listen server): server logic + local client, joinable by remote clients.</summary>
        Host,

        /// <summary>Connected to a remote server; world arrives via replication.</summary>
        Client,
    }

    /// <summary>
    /// Single source of truth for the current world/session lifecycle. Replaces the ad-hoc start/teardown
    /// that used to live in <see cref="LobbyPresenter"/> so we always KNOW which session is active and only
    /// allow transitions that are valid for the current state.
    ///
    /// Model (user-chosen): STRICT GATING — a new session can only be started from <see cref="GameSessionState.None"/>;
    /// <see cref="LeaveAsync"/> tears the current one down to get back there. Every transition is atomic and
    /// guarded by <see cref="IsBusy"/> so the UI can't fire overlapping/invalid actions.
    ///
    /// Losing the network connection involuntarily returns to the lobby (the world is torn down) — a live
    /// multiplayer world can't continue solo without the deferred reconnect/resync feature. An explicit leave
    /// does the same.
    /// </summary>
    public sealed class GameSession : IDisposable
    {
        // How long to wait for a join to actually connect before giving up (no Relay yet → a wrong/absent
        // host would otherwise hang on the transport's own long timeout).
        private const float JoinTimeoutSeconds = 10f;

        private readonly INetworkController networkController;
        private readonly MapController mapController;
        private readonly World world;
        private readonly BalanceReference balance;

        // Set while LeaveAsync/transitions deliberately stop networking, so the resulting Stopped callback
        // isn't mistaken for an involuntary disconnect.
        private bool tearingDown;

        // Cancels an in-flight Join (user pressed Cancel). Distinct from the internal connect timeout.
        private CancellationTokenSource joinCts;

        // Last reason reported by the transport, surfaced in the "session lost" message.
        private string lastDisconnectReason = "Connection lost";

        public GameSessionState State { get; private set; } = GameSessionState.None;
        public bool IsBusy { get; private set; }

        /// <summary>A join is in progress (StartClient issued, awaiting the actual connection). Cancelable.</summary>
        public bool IsConnecting { get; private set; }

        /// <summary>True while connected to a remote server (latency is meaningful — the world arrives over the wire).</summary>
        public bool IsRemoteClient => State == GameSessionState.Client;

        /// <summary>Round-trip time to the server (ms) while a remote client; 0 otherwise.</summary>
        public int RoundTripTimeMs => networkController.RoundTripTimeMs;

        /// <summary>Estimated wire latency (ms) — RTT minus local tick-batching overhead, closer to the true
        /// network round trip; 0 when not a remote client.</summary>
        public int EstimatedWireLatencyMs => networkController.EstimatedWireLatencyMs;

        /// <summary>Discoverable sessions for the lobby's session list, from the active <see cref="SessionSource"/>.</summary>
        public IReadOnlyList<SessionInfo> AvailableSessions => networkController.Sessions;

        /// <summary>The active session source (LAN vs Unity services) backing the list and host/join.</summary>
        public SessionSource SessionSource => networkController.SessionSource;

        /// <summary>Switch the session source (the lobby's Local/Online tabs).</summary>
        public void SetSessionSource(SessionSource source) => networkController.SetSessionSource(source);

        /// <summary>Force a clean re-scan of discoverable sessions (manual lobby refresh / lobby reopen).</summary>
        public void RefreshSessions() => networkController.RefreshSessions();

        /// <summary>Raised whenever <see cref="State"/>, <see cref="IsBusy"/> or <see cref="IsConnecting"/> changes (UI refresh).</summary>
        public event Action EventStateChanged;

        /// <summary>Raised when the network session is lost involuntarily (world torn down, back to lobby).
        /// Carries a short reason for display.</summary>
        public event Action<string> EventSessionLost;

        /// <summary>Raised when a join attempt fails (timed out / connection refused) — but NOT when the user
        /// cancels. Carries the server's reason (e.g. version mismatch / server full), or null if none.</summary>
        public event Action<string> EventJoinFailed;

        /// <summary>Raised when the discoverable session list (<see cref="AvailableSessions"/>) changes.</summary>
        public event Action EventSessionsChanged;

        public GameSession(INetworkController networkController, MapController mapController, World world, BalanceReference balance)
        {
            this.networkController = networkController;
            this.mapController = mapController;
            this.world = world;
            this.balance = balance;

            networkController.Stopped += OnNetworkStopped;
            networkController.PeerDisconnected += OnPeerDisconnected;
            networkController.SessionsUpdated += OnSessionsUpdated;
        }

        private void OnPeerDisconnected(NetId peer, DisconnectReason reason)
        {
            // Prefer the server's specific message (set on a refusal/kick) over the generic enum.
            string detail = networkController.LastDisconnectReason;
            lastDisconnectReason = string.IsNullOrEmpty(detail) ? reason.ToString() : detail;
        }

        private void OnSessionsUpdated(IReadOnlyList<SessionInfo> sessions) => EventSessionsChanged?.Invoke();

        /// <summary>A brand-new session (single-player / host / join) can only start from a clean state.</summary>
        public bool CanStartNewSession => State == GameSessionState.None && !IsBusy;

        /// <summary>Additive maps are a server concern — only valid while we own the world (host/single-player).</summary>
        public bool CanLoadAdditive => (State == GameSessionState.SinglePlayer || State == GameSessionState.Host) && !IsBusy;

        /// <summary>Leaving is valid whenever we're in any session (but not mid-transition).</summary>
        public bool CanLeave => State != GameSessionState.None && !IsBusy;

        /// <summary>A join can be canceled while it's connecting.</summary>
        public bool CanCancelJoin => IsConnecting;

        /// <summary>Start a local single-player session (non-advertised host, both logics).</summary>
        public UniTask StartSinglePlayerAsync(ScenarioDefinition scenario) =>
            StartHostInternalAsync(scenario, advertise: false, GameSessionState.SinglePlayer);

        /// <summary>Start an advertised host other clients can join.</summary>
        public UniTask StartHostAsync(ScenarioDefinition scenario) =>
            StartHostInternalAsync(scenario, advertise: true, GameSessionState.Host);

        private async UniTask StartHostInternalAsync(ScenarioDefinition scenario, bool advertise, GameSessionState targetState)
        {
            if (!CanStartNewSession || scenario == null)
                return;

            await RunTransitionAsync(async () =>
            {
                if (!await networkController.StartHostAsync(BuildRoomToken(scenario), advertise))
                    return false;

                world.ConfigureLogic(hasServerLogic: true, hasClientLogic: true);
                // Host's own player spawns as the lobby-chosen class (sanitised) when the scenario loads below.
                world.LocalPlayerClass = balance.ResolvePlayableClass(ReadPreferredClass());
                await mapController.LoadMapAsync(scenario, unloadOthers: true);
                State = targetState;
                return true;
            });
        }

        // Session metadata broadcast in the host's LAN beacon (server name + map), from the local player prefs
        // and the chosen scenario. Only an advertised host actually broadcasts it.
        // The class chosen in the lobby (persisted by the class slots). 0/None or any non-playable value is
        // sanitised to Mage when the player actually spawns (host: World.LocalPlayerClass; client: the spawner).
        private static ClassType ReadPreferredClass() => (ClassType)PlayerPrefs.GetInt(UnitUtils.PreferredClassPrefName, (int)ClassType.Mage);

        // What a joining client sends to the host: build version (gated by approval) + name + chosen class.
        private ClientConnectionToken BuildConnectionToken() => new ClientConnectionToken
        {
            Name = PlayerPrefs.GetString(PrefUtils.PlayerNamePref, "Player"),
            Version = networkController.Version,
            PreferredClass = ReadPreferredClass(),
        };

        private ServerRoomToken BuildRoomToken(ScenarioDefinition scenario) => new ServerRoomToken(
            PlayerPrefs.GetString(PrefUtils.PlayerServerNamePref, "Server"),
            PlayerPrefs.GetString(PrefUtils.PlayerNamePref, "Player"),
            scenario.Map.MapName,
            scenario: 0)
        {
            Version = networkController.Version,
        };

        /// <summary>
        /// Join a remote host. Stays in a cancelable "connecting" state until the client actually connects
        /// (PeerConnected), the attempt fails/refuses, the <see cref="JoinTimeoutSeconds"/> elapses, or the
        /// user cancels via <see cref="CancelJoin"/>. The world/player arrive via replication afterwards.
        /// </summary>
        public async UniTask JoinAsync(SessionInfo? session = null)
        {
            if (!CanStartNewSession)
                return;

            IsBusy = true;
            IsConnecting = true;
            EventStateChanged?.Invoke();

            joinCts = new CancellationTokenSource();
            bool connected = false;
            try
            {
                world.ConfigureLogic(hasServerLogic: false, hasClientLogic: true);

                // A discovered session connects to its advertised address; a bare join uses the configured one.
                // The token carries our build version (gated by the host) + the lobby-chosen class.
                ClientConnectionToken connectToken = BuildConnectionToken();
                bool started = session.HasValue
                    ? (await networkController.ConnectAsync(session.Value, connectToken)).Success
                    : await networkController.StartClientAsync();
                // Unity-services joins resolve only once already connected (the Sessions API awaits the Relay
                // connection), so skip the wait in that case; LAN joins start instantly and wait for the peer.
                if (started)
                    connected = networkController.IsConnectedClient || await WaitForConnectionAsync(joinCts.Token);
            }
            finally
            {
                // A connect timeout fires its cancellation on a timer (thread-pool) thread, so the continuation
                // can resume off the main thread. Everything below touches Unity (UI events, world config), so
                // get back onto the main thread first.
                await UniTask.SwitchToMainThread();

                bool userCanceled = joinCts.IsCancellationRequested;

                // Commit only if the connection is LIVE *and* the user didn't cancel. Re-verifying IsConnectedClient
                // catches a connect-then-immediately-drop (the Stopped callback fired while State was still None, so
                // OnNetworkStopped no-ops). Honouring userCanceled here is what actually aborts a join the user
                // cancelled — an Online/Relay ConnectAsync isn't cancelable and resolves already-connected, so
                // without this a cancel would still commit Client. Either way we fall into the teardown path.
                bool live = connected && networkController.IsConnectedClient && !userCanceled;

                if (live)
                {
                    State = GameSessionState.Client;
                }
                else
                {
                    // Failed / timed out / canceled / dropped-during-connect — stop the half-open client (and
                    // leave any Unity session), unload any map that started loading while briefly connected, and
                    // return to a clean state.
                    tearingDown = true;
                    try
                    {
                        await networkController.ShutdownAsync();
                        await mapController.UnloadAllAsync();
                    }
                    finally { tearingDown = false; }

                    world.ConfigureLogic(hasServerLogic: true, hasClientLogic: true);
                    State = GameSessionState.None;
                }

                IsConnecting = false;
                IsBusy = false;
                joinCts.Dispose();
                joinCts = null;
                EventStateChanged?.Invoke();

                if (!live && !userCanceled)
                    EventJoinFailed?.Invoke(networkController.LastDisconnectReason);
            }
        }

        /// <summary>Abort an in-flight join early (user pressed Cancel) instead of waiting for the timeout.</summary>
        public void CancelJoin() => joinCts?.Cancel();

        // Resolve once the client connects (PeerConnected → true) or fails (PeerDisconnected → false); a linked
        // token adds the join timeout on top of the user-cancel token, both surfacing as cancellation → false.
        private async UniTask<bool> WaitForConnectionAsync(CancellationToken cancelToken)
        {
            var connected = new UniTaskCompletionSource<bool>();
            Action<NetId> onConnected = _ => connected.TrySetResult(true);
            Action<NetId, DisconnectReason> onDisconnected = (_, __) => connected.TrySetResult(false);

            networkController.PeerConnected += onConnected;
            networkController.PeerDisconnected += onDisconnected;
            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
                timeoutCts.CancelAfter(TimeSpan.FromSeconds(JoinTimeoutSeconds));
                try
                {
                    return await connected.Task.AttachExternalCancellation(timeoutCts.Token);
                }
                catch (OperationCanceledException)
                {
                    return false; // timed out or canceled
                }
            }
            finally
            {
                networkController.PeerConnected -= onConnected;
                networkController.PeerDisconnected -= onDisconnected;
            }
        }

        /// <summary>Add a map to the running session (host/single-player only).</summary>
        public async UniTask LoadAdditiveAsync(ScenarioDefinition scenario)
        {
            if (!CanLoadAdditive || scenario == null)
                return;

            await mapController.LoadMapAsync(scenario, unloadOthers: false);
        }

        /// <summary>Explicitly end the current session: stop networking, unload the world, return to None.</summary>
        public async UniTask LeaveAsync()
        {
            if (!CanLeave)
                return;

            await RunTransitionAsync(async () =>
            {
                tearingDown = true;
                try
                {
                    await networkController.ShutdownAsync();
                    await mapController.UnloadAllAsync();
                    world.ConfigureLogic(hasServerLogic: true, hasClientLogic: true);
                    State = GameSessionState.None;
                    return true;
                }
                finally
                {
                    tearingDown = false;
                }
            });
        }

        // Networking stopped on its own (lost connection / kicked / server went away). Return to a clean,
        // session-less state so the HUD graph shows the lobby (which has the Create/Join/Single Player
        // buttons). The world is torn down — a live multiplayer world can't keep playing solo without the
        // (deferred) reconnect/resync feature, so persisting it would only leave a frozen, button-less scene.
        private void OnNetworkStopped()
        {
            if (tearingDown)
                return; // a deliberate Leave/transition handles its own state

            if (State == GameSessionState.None)
                return;

            HandleDisconnectAsync().Forget();
        }

        private async UniTaskVoid HandleDisconnectAsync()
        {
            // Switch to the lobby first (State drives the HUD graph), then tear the world down behind it so
            // there's no flash of an empty battle HUD while it unloads.
            world.ConfigureLogic(hasServerLogic: true, hasClientLogic: true);
            State = GameSessionState.None;
            EventStateChanged?.Invoke();
            EventSessionLost?.Invoke(lastDisconnectReason);

            await mapController.UnloadAllAsync();
        }

        private async UniTask RunTransitionAsync(Func<UniTask<bool>> transition)
        {
            IsBusy = true;
            EventStateChanged?.Invoke();
            try
            {
                await transition();
            }
            finally
            {
                IsBusy = false;
                EventStateChanged?.Invoke();
            }
        }

        public void Dispose()
        {
            networkController.Stopped -= OnNetworkStopped;
            networkController.PeerDisconnected -= OnPeerDisconnected;
            networkController.SessionsUpdated -= OnSessionsUpdated;
        }
    }
}
