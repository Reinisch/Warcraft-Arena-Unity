namespace Core
{
    public struct MapHeightHeader
    {
        public uint Fourcc;
        public MapHeightFlags Flags;
        public float GridHeight;
        public float GridMaxHeight;
    }
}