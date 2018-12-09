using Core;

namespace Server
{
    public class WorldServerManager : WorldManager
    {
        public override bool HasServerLogic => true;
        public override bool HasClientLogic => true;
    }
}