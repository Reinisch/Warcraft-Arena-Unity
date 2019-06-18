using System;
using Core;
using JetBrains.Annotations;

using Assert = Common.Assert;

namespace Client
{
    [UsedImplicitly]
    public partial class PhotonBoltClientListener : PhotonBoltBaseListener
    {
        public Player LocalPlayer { get; private set; }

        public event Action EventPlayerControlGained;
        public event Action EventPlayerControlLost;

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

                LocalPlayer = (Player)WorldManager.UnitManager.Find(entity.NetworkId.PackedValue);
                EventPlayerControlGained?.Invoke();

                FindObjectOfType<WarcraftCamera>().Target = LocalPlayer;
            }
        }

        public override void ControlOfEntityLost(BoltEntity entity)
        {
            base.ControlOfEntityLost(entity);

            if (entity.StateIs<IPlayerState>())
            {
                Assert.IsTrue(LocalPlayer.BoltEntity == entity, "Lost control of non-local player!");

                EventPlayerControlLost?.Invoke();
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
