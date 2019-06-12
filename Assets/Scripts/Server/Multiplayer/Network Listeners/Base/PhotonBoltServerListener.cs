using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Server
{
    public partial class PhotonBoltServerListener : PhotonBoltBaseListener
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;

        private new WorldServerManager WorldManager { get; set; }

        public void Initialize(WorldServerManager worldManager)
        {
            base.Initialize(worldManager);

            WorldManager = worldManager;
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
