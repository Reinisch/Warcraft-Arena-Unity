namespace Server
{
    internal abstract class BaseGameListener
    {
        protected readonly WorldServer World;

        internal BaseGameListener(WorldServer world)
        {
            World = world;
        }
    }
}
