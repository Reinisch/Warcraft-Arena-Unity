using Bolt;

namespace Core
{
    public abstract class PhotonBoltBaseListener : GlobalEventListener
    {
        protected WorldManager World;

        protected void Initialize(WorldManager worldManager)
        {
            World = worldManager;
        }

        protected void Deinitialize()
        {
            World = null;
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }
    }
}
