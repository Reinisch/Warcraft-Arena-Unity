namespace Core
{
    public class WorldClient : World
    {
        public WorldClient(bool hasServerLogic)
        {
            HasServerLogic = hasServerLogic;
            HasClientLogic = true;
        }
    }
}