using Bolt;
using Bolt.Utils;
using Core;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Server
{
    public partial class PhotonBoltServerListener : PhotonBoltBaseListener
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;

        private new WorldServerManager World { get; set; }
        private ServerLaunchState LaunchState { get; set; }
        private ServerRoomToken ServerToken { get; set; }

        public void Initialize(WorldServerManager worldManager)
        {
            base.Initialize(worldManager);

            World = worldManager;
            World.MapManager.EventMapInitialized += OnMapInitialized;
        }

        public new void Deinitialize()
        {
            World.MapManager.EventMapInitialized -= OnMapInitialized;
            World = null;
            LaunchState = 0;

            base.Deinitialize();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            base.SceneLoadRemoteDone(connection);

            World.CreatePlayer(connection);
        }

        public override void SessionCreated(UdpSession session)
        {
            base.SessionCreated(session);

            ServerToken = (ServerRoomToken)session.GetProtocolToken();
            ProcessServerLaunchState(ServerLaunchState.SessionCreated);
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            base.ConnectRequest(endpoint, token);

            if (!(token is ClientConnectionToken clientToken))
            {
                BoltNetwork.Refuse(endpoint, new ClientRefuseToken(ConnectRefusedReason.InvalidToken));
                return;
            }

            if (clientToken.UnityId == SystemInfo.unsupportedIdentifier)
            {
                BoltNetwork.Refuse(endpoint, new ClientRefuseToken(ConnectRefusedReason.UnsupportedDevice));
                return;
            }

            if (clientToken.Version != ServerToken.Version)
            {
                BoltNetwork.Refuse(endpoint, new ClientRefuseToken(ConnectRefusedReason.InvalidVersion));
                return;
            }

            BoltNetwork.Accept(endpoint);
        }

        public override void Connected(BoltConnection boltConnection)
        {
            base.Connected(boltConnection);

            World.SetScope(boltConnection, true);
        }

        public override void Disconnected(BoltConnection boltConnection)
        {
            base.Disconnected(boltConnection);

            World.SetNetworkState(boltConnection, PlayerNetworkState.Disconnected);
        }

        public override void EntityAttached(BoltEntity entity)
        {
            base.EntityAttached(entity);

            World.EntityAttached(entity);
        }

        public override void EntityDetached(BoltEntity entity)
        {
            base.EntityDetached(entity);

            World.EntityDetached(entity);
        }

        private void OnMapInitialized()
        {
            ProcessServerLaunchState(ServerLaunchState.MapLoaded);
        }

        private void ProcessServerLaunchState(ServerLaunchState state)
        {
            LaunchState |= state;

            if (LaunchState == ServerLaunchState.Complete)
                World.ServerLaunched(ServerToken);
        }
    }
}
