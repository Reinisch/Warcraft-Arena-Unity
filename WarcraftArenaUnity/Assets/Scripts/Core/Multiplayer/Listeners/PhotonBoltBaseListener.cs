using Bolt;

namespace Core
{
    public class PhotonBoltBaseListener : GlobalEventListener
    {
        protected WorldManager World;

        public virtual void Initialize(WorldManager worldManager)
        {
            World = worldManager;
        }

        public virtual void Deinitialize()
        {
            World = null;
        }

        public override bool PersistBetweenStartupAndShutdown()
        {
            return true;
        }
    }
}
