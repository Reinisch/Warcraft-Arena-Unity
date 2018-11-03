namespace Core
{
    public struct MapFileHeader
    {
        public MapMagic MapMagic;
        public MapMagic VersionMagic;
        public MapMagic BuildMagic;
        public uint AreaMapOffset;
        public uint AreaMapSize;
        public uint HeightMapOffset;
        public uint HeightMapSize;
        public uint LiquidMapOffset;
        public uint LiquidMapSize;
        public uint HolesOffset;
        public uint HolesSize;
    }
}