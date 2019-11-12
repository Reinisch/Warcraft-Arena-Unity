namespace Core
{
    public class WorldClientManager : WorldManager
    {
        public WorldClientManager(bool hasServerLogic)
        {
            HasServerLogic = hasServerLogic;
            HasClientLogic = true;
        }
    }
}