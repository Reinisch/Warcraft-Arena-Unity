using Bolt;

namespace Core
{
    public abstract class PhotonBoltBaseListener : GlobalEventListener
    {
        protected WorldManager worldManager;

        protected void Initialize(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        protected void Deinitialize()
        {
            worldManager = null;
        }

        public virtual void DoUpdate(int deltaTime)
        {
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }
    }
}
