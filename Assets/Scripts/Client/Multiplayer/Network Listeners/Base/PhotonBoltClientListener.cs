using Common;
using Core;
using JetBrains.Annotations;

using Assert = Common.Assert;
using EventHandler = Common.EventHandler;

namespace Client
{
    [UsedImplicitly]
    public partial class PhotonBoltClientListener : PhotonBoltBaseListener
    {
        private Player LocalPlayer { get; set; }

        public new void Initialize(WorldManager worldManager)
        {
            base.Initialize(worldManager);
        }

        public new void Deinitialize()
        {
            if (LocalPlayer != null)
                ControlOfEntityLost(LocalPlayer.BoltEntity);

            base.Deinitialize();
        }

        public override void ControlOfEntityGained(BoltEntity entity)
        {
            base.ControlOfEntityGained(entity);

            if (entity.StateIs<IPlayerState>())
            {
                Assert.IsNull(LocalPlayer, "Gained control of another player while already controlling one!");

                LocalPlayer = (Player)World.UnitManager.Find(entity.NetworkId.PackedValue);
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.PlayerControlGained, LocalPlayer);
            }
        }

        public override void ControlOfEntityLost(BoltEntity entity)
        {
            base.ControlOfEntityLost(entity);

            if (entity.StateIs<IPlayerState>())
            {
                Assert.IsTrue(LocalPlayer.BoltEntity == entity, "Lost control of non-local player!");

                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.PlayerControlLost, LocalPlayer);
                LocalPlayer = null;
            }
        }

        public override void EntityDetached(BoltEntity entity)
        {
            base.EntityDetached(entity);

            if (LocalPlayer != null && LocalPlayer.BoltEntity == entity)
            {
                ControlOfEntityLost(LocalPlayer.BoltEntity);
            }
        }
    }
}
