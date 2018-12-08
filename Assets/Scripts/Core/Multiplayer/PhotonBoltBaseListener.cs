using Bolt;

namespace Core
{
    public class PhotonBoltBaseListener : GlobalEventListener
    {
        protected WorldManager worldManager;

        public virtual void Initialize(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public virtual void Deinitialize()
        {
            worldManager = null;
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }
    }
}
