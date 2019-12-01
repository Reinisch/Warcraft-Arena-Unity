using Bolt;

namespace Core
{
    public class PhotonBoltBaseListener : GlobalEventListener
    {
        protected World World;

        public virtual void Initialize(World world)
        {
            World = world;
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
