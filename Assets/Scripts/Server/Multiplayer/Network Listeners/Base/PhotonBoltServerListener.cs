using Bolt;
using Core;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Server
{
    public partial class PhotonBoltServerListener : PhotonBoltBaseListener
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;

        private new WorldServerManager WorldManager { get; set; }
        private ServerRoomToken ServerToken { get; set; }

        public void Initialize(WorldServerManager worldManager)
        {
            base.Initialize(worldManager);

            WorldManager = worldManager;
            WorldManager.SessionUpdated(ServerToken);
        }

        public new void Deinitialize()
        {
            WorldManager = null;

            base.Deinitialize();
        }

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (WorldManager.HasClientLogic)
                WorldManager.CreatePlayer();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            base.SceneLoadRemoteDone(connection);

            WorldManager.CreatePlayer(connection);
        }

        public override void SessionCreated(UdpSession session)
        {
            base.SessionCreated(session);

            ServerToken = (ServerRoomToken)session.GetProtocolToken();
            WorldManager?.SessionUpdated(ServerToken);
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

            WorldManager.SetScope(boltConnection, true);
        }

        public override void Disconnected(BoltConnection boltConnection)
        {
            base.Disconnected(boltConnection);

            WorldManager.SetNetworkState(boltConnection, PlayerNetworkState.Disconnected);
        }

        public override void EntityAttached(BoltEntity entity)
        {
            base.EntityAttached(entity);

            WorldManager.EntityAttached(entity);
        }

        public override void EntityDetached(BoltEntity entity)
        {
            base.EntityDetached(entity);

            WorldManager.EntityDetached(entity);
        }
    }
}
