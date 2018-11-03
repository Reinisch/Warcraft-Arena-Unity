namespace Core
{
    public struct MapLiquidHeader
    {
        public uint Fourcc;
        public LiquidHeaderFlags Flags;
        public ushort LiquidType;
        public byte OffsetX;
        public byte OffsetY;
        public byte Width;
        public byte Height;
        public float LiquidLevel;
    }
}