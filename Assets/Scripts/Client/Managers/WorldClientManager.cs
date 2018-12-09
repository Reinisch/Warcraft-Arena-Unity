namespace Core
{
    public class WorldClientManager : WorldManager
    {
        public override bool HasServerLogic => false;
        public override bool HasClientLogic => true;
    }
}