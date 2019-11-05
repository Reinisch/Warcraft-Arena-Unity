using System;
using System.Collections;
using Bolt;
using Bolt.Matchmaking;
using UdpKit;
using Common;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    internal class PhotonBoltController : GlobalEventListener, IPhotonBoltController
    {
        private enum State
        {
            Inactive,
            Active,
            Starting,
            Connecting
        }

        [SerializeField, UsedImplicitly] private PhotonBoltBaseListener boltSharedListener;
        [SerializeField, UsedImplicitly] private PhotonBoltBaseListener boltServerListener;
        [SerializeField, UsedImplicitly] private PhotonBoltBaseListener boltClientListener;

        private const float MaxConnectionAttemptTime = 50.0f;

        private readonly ConnectionAttemptInfo connectionAttemptInfo = new ConnectionAttemptInfo();
        private NetworkingMode networkingMode;
        private WorldManager worldManager;
        private BoltConfig config;
        private State state;

        public Map<Guid, UdpSession> Sessions => BoltNetwork.SessionList;
        public string Version => "1.0.53";

        internal void Register()
        {
            config = BoltRuntimeSettings.instance.GetConfigCopy();
            config.connectionRequestTimeout = (int)(MaxConnectionAttemptTime * 1000.0f);

            SetListeners(false, false, false);

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        internal void Unregister()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            SetListeners(false, false, false);
        }

        public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
        {
            base.SessionListUpdated(sessionList);

            EventHandler.ExecuteEvent(this, GameEvents.SessionListUpdated);
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }

        public void StartServer(ServerRoomToken serverToken, bool withClientLogic, Action onStartSuccess, Action onStartFail)
        {
            StopAllCoroutines();

            networkingMode = withClientLogic ? NetworkingMode.Both : NetworkingMode.Server;
            serverToken.Version = Version;

            StartCoroutine(StartServerRoutine(serverToken, false, onStartSuccess, onStartFail));
        }

        public void StartClient(Action onStartSuccess, Action onStartFail, bool forceRestart)
        {
            StopAllCoroutines();

            networkingMode = NetworkingMode.Client;
            StartCoroutine(StartClientRoutine(onStartSuccess, onStartFail, forceRestart));
        }

        public void StartSinglePlayer(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail)
        {
            StopAllCoroutines();

            networkingMode = NetworkingMode.Both;
            serverToken.Version = Version;

            StartCoroutine(StartServerRoutine(serverToken, true, onStartSuccess, onStartFail));
        }

        public void StartConnection(UdpSession session, ClientConnectionToken token, Action onConnectSuccess, Action<ClientConnectFailReason> onConnectFail)
        {
            StopAllCoroutines();

            networkingMode = NetworkingMode.Client;
            state = State.Connecting;
            token.Version = Version;

            StartCoroutine(ConnectClientRoutine(session, token, onConnectSuccess, onConnectFail));
        }

        public override void BoltStartBegin()
        {
            base.BoltStartBegin();

            BoltNetwork.RegisterTokenClass<ServerRoomToken>();
            BoltNetwork.RegisterTokenClass<ClientConnectionToken>();
            BoltNetwork.RegisterTokenClass<ClientRefuseToken>();
            BoltNetwork.RegisterTokenClass<SpellProcessingToken>();
            BoltNetwork.RegisterTokenClass<Creature.CreateToken>();
            BoltNetwork.RegisterTokenClass<Player.CreateToken>();
            BoltNetwork.RegisterTokenClass<Player.ControlGainToken>();
        }

        public override void BoltStartDone()
        {
            base.BoltStartDone();

            bool hasServerLogic = networkingMode == NetworkingMode.Server || networkingMode == NetworkingMode.Both;
            bool hasClientLogic = networkingMode == NetworkingMode.Client || networkingMode == NetworkingMode.Both;

            SetListeners(true, hasServerLogic, hasClientLogic);
        }

        public override void BoltStartFailed()
        {
            base.BoltStartFailed();

            SetListeners(false, false, false);
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            base.BoltShutdownBegin(registerDoneCallback, disconnectReason);

            if (worldManager != null && worldManager.HasServerLogic)
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromMaster);
        }

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (BoltNetwork.IsConnected)
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.GameMapLoaded, map, networkingMode);
        }

        public override void Connected(BoltConnection connection)
        {
            base.Connected(connection);

            if (state == State.Connecting)
            {
                connectionAttemptInfo.IsConnected = true;
                state = State.Active;
            }
        }

        public override void Disconnected(BoltConnection connection)
        {
            base.Disconnected(connection);

            if (networkingMode == NetworkingMode.Client)
            {
                StopAllCoroutines();

                Debug.LogError("Disconnected: reason: " + connection.DisconnectReason);
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.DisconnectedFromHost, connection.DisconnectReason);
            }
        }

        public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
        {
            base.ConnectAttempt(endpoint, token);

            if (state == State.Connecting)
                connectionAttemptInfo.TimeSinceAttempt = 0.0f;
        }

        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            base.ConnectFailed(endpoint, token);

            if (state == State.Connecting)
            {
                connectionAttemptInfo.IsFailed = true;
                state = BoltNetwork.IsRunning ? State.Active : State.Inactive;
            }
        }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            base.ConnectRefused(endpoint, token);

            if (state == State.Connecting)
            {
                connectionAttemptInfo.IsRefused = true;
                connectionAttemptInfo.RefuseReason = (token as ClientRefuseToken)?.Reason ?? ConnectRefusedReason.None;
                state = BoltNetwork.IsRunning ? State.Active : State.Inactive;
            }
        }

        public override void SessionConnectFailed(UdpSession session, IProtocolToken token)
        {
            base.SessionConnectFailed(session, token);

            if (state == State.Connecting)
            {
                connectionAttemptInfo.IsFailed = true;
                state = BoltNetwork.IsRunning ? State.Active : State.Inactive;
            }
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            boltSharedListener.Initialize(worldManager);

            if (worldManager.HasServerLogic)
                boltServerListener.Initialize(worldManager);
            if (worldManager.HasClientLogic)
                boltClientListener.Initialize(worldManager);
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            SetListeners(false, false, false);

            boltSharedListener.Deinitialize();

            if (worldManager.HasServerLogic)
                boltServerListener.Deinitialize();
            if (worldManager.HasClientLogic)
                boltClientListener.Deinitialize();

            this.worldManager = null;
        }

        private void SetListeners(bool shared, bool server, bool client)
        {
            boltSharedListener.enabled = false;
            boltServerListener.enabled = false;
            boltClientListener.enabled = false;

            boltSharedListener.enabled = shared;
            boltServerListener.enabled = server;
            boltClientListener.enabled = client;

            state = server || client ? State.Active : State.Inactive;
        }

        private IEnumerator StartServerRoutine(ServerRoomToken serverToken, bool singlePlayer, Action onStartSuccess, Action onStartFail)
        {
            if (BoltNetwork.IsRunning && !BoltNetwork.IsServer)
            {
                BoltLauncher.Shutdown();

                yield return new WaitUntil(NetworkIsInactive);
            }

            state = State.Starting;

            if (singlePlayer)
                BoltLauncher.StartSinglePlayer(config);
            else
                BoltLauncher.StartServer(config);

            yield return new WaitUntil(NetworkIsIdle);

            for (int i = 0; i < 3; i++)
                yield return new WaitForEndOfFrame();

            if (BoltNetwork.IsServer)
            {
                onStartSuccess?.Invoke();

                if (!singlePlayer)
                    BoltMatchmaking.CreateSession(Guid.NewGuid().ToString(), serverToken);

                BoltNetwork.LoadScene(serverToken.Map, serverToken);
            }
            else
            {
                onStartFail?.Invoke();
            }
        }

        private IEnumerator StartClientRoutine(Action onStartSuccess, Action onStartFail, bool forceRestart)
        {
            if (BoltNetwork.IsRunning && !BoltNetwork.IsClient || BoltNetwork.IsRunning && forceRestart)
            {
                BoltLauncher.Shutdown();

                yield return new WaitUntil(NetworkIsInactive);
            }

            if (!BoltNetwork.IsClient)
            {
                state = State.Starting;

                BoltLauncher.StartClient(config);
                yield return new WaitUntil(NetworkIsIdle);
            }

            if (BoltNetwork.IsClient)
            {
                onStartSuccess?.Invoke();
            }
            else
            {
                onStartFail?.Invoke();
            }
        }

        private IEnumerator ConnectClientRoutine(UdpSession session, ClientConnectionToken token, Action onConnectSuccess, Action<ClientConnectFailReason> onConnectFail)
        {
            if (BoltNetwork.IsRunning && !BoltNetwork.IsClient)
            {
                BoltLauncher.Shutdown();

                yield return new WaitUntil(NetworkIsInactive);
            }

            if (!BoltNetwork.IsClient)
            {
                state = State.Starting;

                BoltLauncher.StartClient();
                yield return new WaitUntil(NetworkIsIdle);
            }

            if (!BoltNetwork.IsClient)
            {
                onConnectFail?.Invoke(ClientConnectFailReason.FailedToConnectToMaster);
                yield break;
            }

            state = State.Connecting;
            connectionAttemptInfo.Reset();
            BoltNetwork.Connect(session, token);

            while (true)
            {
                connectionAttemptInfo.TimeSinceAttempt += Time.deltaTime;

                if (connectionAttemptInfo.TimeSinceAttempt > MaxConnectionAttemptTime)
                {
                    onConnectFail?.Invoke(ClientConnectFailReason.ConnectionTimeout);

                    state = BoltNetwork.IsRunning ? State.Active : State.Inactive;

                    yield break;
                }

                if (connectionAttemptInfo.IsRefused)
                {
                    onConnectFail?.Invoke(connectionAttemptInfo.RefuseReason.ToConnectFailReason());
                    yield break;
                }

                if (connectionAttemptInfo.IsFailed)
                {
                    onConnectFail?.Invoke(ClientConnectFailReason.FailedToConnectToSession);
                    yield break;
                }

                if (connectionAttemptInfo.IsConnected)
                {
                    onConnectSuccess?.Invoke();
                    yield break;
                }

                yield return null;
            }
        }

        private bool NetworkIsInactive()
        {
            return !BoltNetwork.IsRunning;
        }

        private bool NetworkIsIdle()
        {
            return state == State.Inactive || state == State.Active;
        }
    }
}
