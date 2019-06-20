using Bolt;

namespace Core
{
    public abstract class PhotonBoltBaseListener : GlobalEventListener
    {
        protected WorldManager WorldManager;

        protected void Initialize(WorldManager worldManager)
        {
            WorldManager = worldManager;
        }

        protected void Deinitialize()
        {
            WorldManager = null;
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }
    }
}
