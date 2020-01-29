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

        public override void Initialize(World world)
        {
            base.Initialize(world);

            World.UnitManager.EventEntityDetach += OnEntityDetach;
        }

        public override void Deinitialize()
        {
            World.UnitManager.EventEntityDetach -= OnEntityDetach;

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
                EventHandler.ExecuteEvent(GameEvents.ClientControlStateChanged, LocalPlayer, true);
            }
        }

        public override void ControlOfEntityLost(BoltEntity entity)
        {
            base.ControlOfEntityLost(entity);

            if (entity.StateIs<IPlayerState>())
            {
                Assert.IsTrue(LocalPlayer.BoltEntity == entity, "Lost control of non-local player!");

                EventHandler.ExecuteEvent(GameEvents.ClientControlStateChanged, LocalPlayer, false);
                LocalPlayer = null;
            }
        }

        private void OnEntityDetach(Unit unit)
        {
            if (LocalPlayer == unit)
                ControlOfEntityLost(LocalPlayer.BoltEntity);
        }
    }
}
