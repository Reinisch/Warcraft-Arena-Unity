namespace Server
{
    internal abstract class BaseGameListener
    {
        protected readonly WorldServerManager World;

        internal BaseGameListener(WorldServerManager world)
        {
            World = world;
        }

        internal abstract void Dispose();
    }
}
