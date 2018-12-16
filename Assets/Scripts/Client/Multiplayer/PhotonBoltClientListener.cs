using Core;

namespace Client
{
    public class PhotonBoltClientListener : PhotonBoltBaseListener
    {
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

            var localPlayer = worldManager.UnitManager.Find(entity.networkId.PackedValue) as Player;
            if (localPlayer != null)
                FindObjectOfType<WarcraftCamera>().Target = localPlayer.transform;
        }
    }
}
