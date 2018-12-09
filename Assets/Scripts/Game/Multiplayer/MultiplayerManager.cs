using System;
using System.Collections;
using Bolt;
using Client;
using Core;
using Server;
using UnityEngine;
using JetBrains.Annotations;
using UdpKit;

namespace Game
{
    public class MultiplayerManager : PhotonBoltManager
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

        private const float MaxConnectionAttemptTime = 2.0f;

        private readonly ConnectionAttemptInfo connectionAttemptInfo = new ConnectionAttemptInfo();
        private GameManager.NetworkingMode networkingMode;
        private State state;

        public event Action<string, GameManager.NetworkingMode> EventGameMapLoaded;
        public event Action<UdpConnectionDisconnectReason> EventDisconnectedFromHost;

        public void Initialize()
        {
            SetListeners(false, false, false);
        }

        public void Deinitialize()
        {
            SetListeners(false, false, false);
        }

        public void InitializeWorld(WorldManager worldManager, bool serverLogic, bool clientLogic)
        {
            boltSharedListener.Initialize(worldManager);

            if(serverLogic)
                boltServerListener.Initialize(worldManager);
            if (clientLogic)
                boltClientListener.Initialize(worldManager);
        }

        public void DeinitializeWorld(bool serverLogic, bool clientLogic)
        {
            boltSharedListener.Deinitialize();

            if (serverLogic)
                boltServerListener.Deinitialize();
            if (clientLogic)
                boltClientListener.Deinitialize();
        }

        public override void StartServer(ServerRoomToken serverToken, Action onStartSuccess, Action onStartFail)
        {
            StopAllCoroutines();

            StartCoroutine(StartServerRoutine(serverToken, onStartSuccess, onStartFail));
        }

        public override void StartClient(Action onStartSuccess, Action onStartFail)
        {
            StopAllCoroutines();

            StartCoroutine(StartClientRoutine(onStartSuccess, onStartFail));
        }

        public override void StartConnection(UdpSession session, Action onConnectSuccess, Action onConnectFail)
        {
            StopAllCoroutines();

            state = State.Connecting;

            StartCoroutine(ConnectClientRoutine(session, onConnectSuccess, onConnectFail));
        }

        public override void BoltStartBegin()
        {
            base.BoltStartBegin();

            BoltNetwork.RegisterTokenClass<ServerRoomToken>();
        }

        public override void BoltStartDone()
        {
            base.BoltStartDone();

            SetListeners(true, BoltNetwork.isServer, BoltNetwork.isClient || BoltNetwork.isServer);
        }

        public override void BoltStartFailed()
        {
            base.BoltStartFailed();

            SetListeners(false, false, false);
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback)
        {
            base.BoltShutdownBegin(registerDoneCallback);

            SetListeners(false, false, false);
        }

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            EventGameMapLoaded?.Invoke(map, networkingMode);
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
            base.Connected(connection);

            if (networkingMode == GameManager.NetworkingMode.Client)
                EventDisconnectedFromHost?.Invoke(connection.DisconnectReason);
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
                state = BoltNetwork.isRunning ? State.Active : State.Inactive;
            }
        }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            base.ConnectRefused(endpoint, token);

            if (state == State.Connecting)
            {
                connectionAttemptInfo.IsFailed = true;
                state = BoltNetwork.isRunning ? State.Active : State.Inactive;
            }
        }

        public override void SessionConnectFailed(UdpSession session, IProtocolToken token)
        {
            base.SessionConnectFailed(session, token);

            if (state == State.Connecting)
            {
                connectionAttemptInfo.IsFailed = true;
                state = BoltNetwork.isRunning ? State.Active : State.Inactive;
            }
        }

        private void SetListeners(bool shared, bool server, bool client)
        {
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
            if (BoltNetwork.isRunning && !BoltNetwork.isServer)
            {
                BoltNetwork.ShutdownImmediate();

                yield return new WaitUntil(NetworkIsInactive);
            }

            state = State.Starting;

            BoltLauncher.StartServer();
            yield return new WaitUntil(NetworkIsIdle);

            if (BoltNetwork.isServer)
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

        private IEnumerator StartClientRoutine(Action onStartSuccess, Action onStartFail)
        {
            if (BoltNetwork.isRunning && !BoltNetwork.isClient)
            {
                BoltNetwork.ShutdownImmediate();

                yield return new WaitUntil(NetworkIsInactive);
            }

            if (!BoltNetwork.isClient)
            {
                state = State.Starting;

                BoltLauncher.StartClient();
                yield return new WaitUntil(NetworkIsIdle);
            }

            if (BoltNetwork.isClient)
            {
                onStartSuccess?.Invoke();
            }
            else
            {
                onStartFail?.Invoke();
            }
        }

        private IEnumerator ConnectClientRoutine(UdpSession session, Action onConnectSuccess, Action onConnectFail)
        {
            if (BoltNetwork.isRunning && !BoltNetwork.isClient)
            {
                BoltNetwork.ShutdownImmediate();

                yield return new WaitUntil(NetworkIsInactive);
            }

            if (!BoltNetwork.isClient)
            {
                state = State.Starting;

                BoltLauncher.StartClient();
                yield return new WaitUntil(NetworkIsIdle);
            }

            if (!BoltNetwork.isClient)
            {
                onConnectFail?.Invoke();
                yield break;
            }

            state = State.Connecting;
            connectionAttemptInfo.Reset();
            BoltNetwork.Connect(session);

            while (true)
            {
                connectionAttemptInfo.TimeSinceAttempt += Time.deltaTime;

                if (connectionAttemptInfo.TimeSinceAttempt > MaxConnectionAttemptTime)
                {
                    onConnectFail?.Invoke();

                    state = BoltNetwork.isRunning ? State.Active : State.Inactive;

                    yield break;
                }

                if (connectionAttemptInfo.IsFailed || connectionAttemptInfo.IsRefused)
                {
                    onConnectFail?.Invoke();
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
            return !BoltNetwork.isRunning;
        }

        private bool NetworkIsIdle()
        {
            return state == State.Inactive || state == State.Active;
        }
    }
}
