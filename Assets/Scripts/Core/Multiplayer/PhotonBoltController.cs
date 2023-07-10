﻿using System;
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
        private World world;
        private BoltConfig config;
        private State state;

        public Map<Guid, UdpSession> Sessions => BoltNetwork.SessionList;
        public string Version => "1.0.92";

        internal void Register()
        {
            config = BoltRuntimeSettings.instance.GetConfigCopy();
            config.connectionRequestTimeout = (int)(MaxConnectionAttemptTime * 1000.0f);

            SetListeners(false, false, false);

            EventHandler.RegisterEvent<World, bool>(GameEvents.WorldStateChanged, OnWorldStateChanged);
        }

        internal void Unregister()
        {
            EventHandler.UnregisterEvent<World, bool>(GameEvents.WorldStateChanged, OnWorldStateChanged);

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

            if (world != null && world.HasServerLogic)
                EventHandler.ExecuteEvent(GameEvents.DisconnectedFromMaster);
        }

        public override void SceneLoadLocalDone(string map)
        {
            // TODO(TwiiK): After upgrading Bolt from 1.2.9 to 1.2.15 the "Launcher" scene would be passed in here as
            // well, which cause errors like duplicate players etc. It wasn't like this originally, but I'm not sure
            // what exactly has changed in Bolt to cause this. I'm sure this can be fixed properly, and not with a hack
            // like this, but I'm not going to investigate that at the moment. In another project I've upgraded Bolt to
            // 1.3.2 and the problem is there as well, so I assume this is due to some change in Bolt itself.
            if (map == "Launcher") {
                return;
            }
            
            base.SceneLoadLocalDone(map);

            if (BoltNetwork.IsConnected)
                EventHandler.ExecuteEvent(GameEvents.GameMapLoaded, map, networkingMode);
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
                EventHandler.ExecuteEvent(GameEvents.DisconnectedFromHost, connection.DisconnectReason);
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

        private void OnWorldStateChanged(World world, bool created)
        {
            if (created)
            {
                this.world = world;

                boltSharedListener.Initialize(world);

                if (world.HasServerLogic)
                    boltServerListener.Initialize(world);
                if (world.HasClientLogic)
                    boltClientListener.Initialize(world);
            }
            else
            {
                SetListeners(false, false, false);

                boltSharedListener.Deinitialize();

                if (world.HasServerLogic)
                    boltServerListener.Deinitialize();
                if (world.HasClientLogic)
                    boltClientListener.Deinitialize();

                this.world = null;
            }
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
            BoltMatchmaking.JoinSession(session, token);

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
