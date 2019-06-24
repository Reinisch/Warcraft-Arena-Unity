using System;
using System.Collections;
using Bolt;
using Client;
using Common;
using Core;
using Server;
using UnityEngine;
using JetBrains.Annotations;
using UdpKit;

using EventHandler = Common.EventHandler;

namespace Game
{
    public class MultiplayerController : PhotonBoltController
    {
        private enum State
        {
            Inactive,
            Active,
            Starting,
            Connecting
        }

        [SerializeField, UsedImplicitly] private PhotonBoltSharedListener boltSharedListener;
        [SerializeField, UsedImplicitly] private PhotonBoltServerListener boltServerListener;
        [SerializeField, UsedImplicitly] private PhotonBoltClientListener boltClientListener;

        private const float MaxConnectionAttemptTime = 10.0f;

        private readonly ConnectionAttemptInfo connectionAttemptInfo = new ConnectionAttemptInfo();
        private GameManager.NetworkingMode networkingMode;
        private WorldManager worldManager;
        private BoltConfig config;
        private State state;

        public override string Version => "1.0.1";

        protected override void OnRegistered()
        {
            config = BoltRuntimeSettings.instance.GetConfigCopy();
            config.connectionRequestTimeout = (int)(MaxConnectionAttemptTime * 1000.0f);

            SetListeners(false, false, false);

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        protected override void OnUnregistered()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            SetListeners(false, false, false);
        }

        public override void StartServer(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail)
        {
            StopAllCoroutines();

            serverToken.Version = Version;

            StartCoroutine(StartServerRoutine(serverToken, onStartSuccess, onStartFail));
        }

        public override void StartClient(Action onStartSuccess, Action onStartFail, bool forceRestart)
        {
            StopAllCoroutines();

            StartCoroutine(StartClientRoutine(onStartSuccess, onStartFail, forceRestart));
        }

        public override void StartConnection(UdpSession session, ClientConnectionToken token, Action onConnectSuccess, Action<ClientConnectFailReason> onConnectFail)
        {
            StopAllCoroutines();

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
            BoltNetwork.RegisterTokenClass<Unit.CreateToken>();
            BoltNetwork.RegisterTokenClass<Player.CreateToken>();
        }

        public override void BoltStartDone()
        {
            base.BoltStartDone();

            SetListeners(true, BoltNetwork.IsServer, BoltNetwork.IsClient || BoltNetwork.IsServer);
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

            if (networkingMode == GameManager.NetworkingMode.Client)
            {
                Debug.LogError("Disconnected: reason: " + connection.DisconnectReason + " from " + connection);
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

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            boltSharedListener.Initialize(worldManager);

            if (worldManager.HasServerLogic)
                boltServerListener.Initialize((WorldServerManager)worldManager);
            if (worldManager.HasClientLogic)
                boltClientListener.Initialize(worldManager);
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            SetListeners(false, false, false);

            boltSharedListener.Deinitialize();

            if (worldManager.HasServerLogic)
                boltServerListener.Deinitialize();
            if (worldManager.HasServerLogic)
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

            if (server && client)
            {
                state = State.Active;
                networkingMode = GameManager.NetworkingMode.Both;
            }
            else if (server)
            {
                state = State.Active;
                networkingMode = GameManager.NetworkingMode.Server;
            }
            else if (client)
            {
                state = State.Active;
                networkingMode = GameManager.NetworkingMode.Client;
            }
            else
            {
                state = State.Inactive;
                networkingMode = GameManager.NetworkingMode.None;
            }
        }

        private IEnumerator StartServerRoutine(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail)
        {
            if (BoltNetwork.IsRunning && !BoltNetwork.IsServer)
            {
                BoltLauncher.Shutdown();

                yield return new WaitUntil(NetworkIsInactive);
            }

            state = State.Starting;

            BoltLauncher.StartServer(config);
            yield return new WaitUntil(NetworkIsIdle);

            if (BoltNetwork.IsServer)
            {
                onStartSuccess?.Invoke();

                BoltNetwork.SetServerInfo(Guid.NewGuid().ToString(), serverToken);
                BoltNetwork.LoadScene(serverToken.Map);
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
