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
    public enum GameSessionState
    {
        None,
        SinglePlayer,
        Host,
        Client,
    }

    /// <summary>
    /// Single source of truth for the current world/session lifecycle. 
    /// </summary>
    public sealed class GameSession : IDisposable
    {
        private static ClassType ReadPreferredClass() => (ClassType)PlayerPrefs.GetInt(UnitUtils.PreferredClassPrefName, (int)ClassType.Mage);

        private const float JoinTimeoutSeconds = 10f;

        private readonly INetworkController networkController;
        private readonly MapController mapController;
        private readonly World world;
        private readonly BalanceReference balance;
        private readonly EventBus eventBus;

        private bool tearingDown;

        // Cancels an in-flight Join. Distinct from the internal connect timeout.
        private CancellationTokenSource joinCts;
        private string lastDisconnectReason = "Connection lost";

        /// <summary>Round-trip time to the server (ms) while a remote client; 0 otherwise.</summary>
        public int RoundTripTimeMs => networkController.RoundTripTimeMs;

        /// <summary>Estimated wire latency (ms) — RTT minus local tick-batching overhead</summary>
        public int EstimatedWireLatencyMs => networkController.EstimatedWireLatencyMs;

        public bool IsBusy { get; private set; }
        public bool IsConnecting { get; private set; }
        public bool IsRemoteClient => State == GameSessionState.Client;
        public GameSessionState State { get; private set; } = GameSessionState.None;

        public string Version => networkController.Version;
        public IReadOnlyList<SessionInfo> AvailableSessions => networkController.Sessions;
        public SessionSource SessionSource => networkController.SessionSource;
        public bool CanStartNewSession => State == GameSessionState.None && !IsBusy;
        public bool CanLoadAdditive => HasServerLogic && !IsBusy;
        public bool CanLeave => State != GameSessionState.None && !IsBusy;
        public bool CanCancelJoin => IsConnecting;
        public bool HasServerLogic => State == GameSessionState.Host || State == GameSessionState.SinglePlayer;

        public event Action EventStateChanged;
        public event Action<string> EventSessionLost;
        public event Action<string> EventJoinFailed;
        public event Action EventSessionsChanged;

        public GameSession(INetworkController networkController, MapController mapController, World world,
            BalanceReference balance, EventBus eventBus)
        {
            this.networkController = networkController;
            this.mapController = mapController;
            this.world = world;
            this.balance = balance;
            this.eventBus = eventBus;

            networkController.Stopped += OnNetworkStopped;
            networkController.PeerDisconnected += OnPeerDisconnected;
            networkController.SessionsUpdated += OnSessionsUpdated;

            // A scenario (e.g. the arena, when a match ends) can request returning to the lobby. Raised
            // server-side; on the host that's us, and remote clients follow when the host shuts down.
            eventBus.RegisterEvent(GameEvents.SessionLeaveRequested, OnSessionLeaveRequested);
        }

        public void Dispose()
        {
            networkController.Stopped -= OnNetworkStopped;
            networkController.PeerDisconnected -= OnPeerDisconnected;
            networkController.SessionsUpdated -= OnSessionsUpdated;
            eventBus.UnregisterEvent(GameEvents.SessionLeaveRequested, OnSessionLeaveRequested);
        }

        public UniTask StartSinglePlayerAsync(ScenarioDefinition scenario) =>
            StartHostInternalAsync(scenario, advertise: false, GameSessionState.SinglePlayer);

        public UniTask StartHostAsync(ScenarioDefinition scenario) =>
            StartHostInternalAsync(scenario, advertise: true, GameSessionState.Host);

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

                bool started = session.HasValue
                    ? (await networkController.ConnectAsync(session.Value, BuildConnectionToken())).Success
                    : await networkController.StartClientAsync();

                // Unity-services joins resolve only once already connected (the Sessions API awaits the Relay
                // connection), so skip the wait in that case; LAN joins start instantly and wait for the peer.
                if (started)
                    connected = networkController.IsConnectedClient || await WaitForConnectionAsync(joinCts.Token);
            }
            finally
            {
                // TODO: revisit, a connect timeout fires its cancellation on a timer (thread-pool) thread
                await UniTask.SwitchToMainThread();

                bool userCanceled = joinCts.IsCancellationRequested;
                bool live = connected && networkController.IsConnectedClient && !userCanceled;

                if (live)
                {
                    State = GameSessionState.Client;
                }
                else
                {
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

            ClientConnectionToken BuildConnectionToken() => new()
            {
                Name = PlayerPrefs.GetString(PrefUtils.PlayerNamePref, "Player"),
                Version = networkController.Version,
                PreferredClass = ReadPreferredClass(),
            };
        }

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

        public async UniTask LoadAdditiveAsync(ScenarioDefinition scenario)
        {
            if (!CanLoadAdditive || scenario == null)
                return;

            await mapController.LoadMapAsync(scenario, unloadOthers: false);
        }

        public void CancelJoin() => joinCts?.Cancel();

        public void SetSessionSource(SessionSource source) => networkController.SetSessionSource(source);

        public void RefreshSessions() => networkController.RefreshSessions();

        private void OnNetworkStopped()
        {
            if (tearingDown)
                return;

            if (State == GameSessionState.None)
                return;

            HandleDisconnectAsync().Forget();
        }

        private void OnSessionLeaveRequested() => LeaveAsync().Forget();

        private void OnPeerDisconnected(NetId peer, DisconnectReason reason)
        {
            string detail = networkController.LastDisconnectReason;
            lastDisconnectReason = string.IsNullOrEmpty(detail) ? reason.ToString() : detail;
        }

        private void OnSessionsUpdated(IReadOnlyList<SessionInfo> sessions) => EventSessionsChanged?.Invoke();

        private async UniTask StartHostInternalAsync(ScenarioDefinition scenario, bool advertise, GameSessionState targetState)
        {
            if (!CanStartNewSession || scenario == null)
                return;

            await RunTransitionAsync(async () =>
            {
                if (!await networkController.StartHostAsync(BuildRoomToken(scenario), advertise))
                    return false;

                world.ConfigureLogic(hasServerLogic: true, hasClientLogic: true);
                world.LocalPlayerClass = balance.ResolvePlayableClass(ReadPreferredClass());

                await mapController.LoadMapAsync(scenario, unloadOthers: true);
                State = targetState;
                return true;
            });

            ServerRoomToken BuildRoomToken(ScenarioDefinition roomScenario) => new(
                PlayerPrefs.GetString(PrefUtils.PlayerServerNamePref, "Server"),
                PlayerPrefs.GetString(PrefUtils.PlayerNamePref, "Player"),
                roomScenario.Map.MapName,
                scenario: 0)
            {
                Version = networkController.Version,
                TeamSize = roomScenario.IsArena ? roomScenario.TeamSize : 0,
            };
        }

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

        private async UniTaskVoid HandleDisconnectAsync()
        {
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
    }
}
