namespace Server
{
    internal abstract class BaseGameListener
    {
        protected readonly WorldServerManager WorldServerManager;

        internal BaseGameListener(WorldServerManager worldServerManager)
        {
            WorldServerManager = worldServerManager;
        }

        internal abstract void Dispose();
    }
}
