namespace Core
{
    public class WorldLocation : Position
    {
        public int MapId { get; set; }

        public WorldLocation(float x = 0.0f, float y = 0.0f, float z = 0.0f, float o = 0.0f, int mapId = GridHelper.MapIdInvalid) : base(x, y, z, o)
        {
            MapId = mapId;
        }

        public WorldLocation(WorldLocation loc) : base(loc)
        {
            MapId = loc.MapId;
        }


        public void WorldRelocate(WorldLocation loc)
        {
            MapId = loc.MapId;
            Relocate(loc);
        }

        public void WorldRelocate(int mapId = GridHelper.MapIdInvalid, float x = 0.0f, float y = 0.0f, float z = 0.0f, float o = 0.0f)
        {
            MapId = mapId;
            Relocate(x, y, z, o);
        }
    }
}