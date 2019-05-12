using System;
using Core;
using JetBrains.Annotations;

using Assert = UnityEngine.Assertions.Assert;

namespace Client
{
    [UsedImplicitly]
    public class PhotonBoltClientListener : PhotonBoltBaseListener
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
            base.Deinitialize();
        }

        public override void ControlOfEntityGained(BoltEntity entity)
        {
            base.ControlOfEntityGained(entity);

            if (entity.StateIs<IPlayerState>())
            {
                Assert.IsNull(LocalPlayer, "Gained control of another player while already controlling one!");

                LocalPlayer = (Player)WorldManager.UnitManager.Find(entity.networkId.PackedValue);
                EventPlayerControlGained?.Invoke();

                FindObjectOfType<WarcraftCamera>().Target = LocalPlayer.transform;
            }
        }

        public override void ControlOfEntityLost(BoltEntity entity)
        {
            base.ControlOfEntityGained(entity);

            if (entity.StateIs<IPlayerState>())
            {
                Assert.IsTrue(LocalPlayer.BoltEntity == entity, "Lost control of non-local player!");

                EventPlayerControlLost?.Invoke();
                LocalPlayer = null;
            }
        }
    }
}
