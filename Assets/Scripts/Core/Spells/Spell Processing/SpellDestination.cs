namespace Core
{
    public struct SpellDestination
    {
        public WorldLocation Position { get; private set; }
        public Position TransportOffset { get; private set; }
        public ulong TransportId { get; set; }


        public SpellDestination(float x, float y, float z, float orientation = 0.0f, int mapId = GridHelper.MapIdInvalid) : this()
        {
            Position = new WorldLocation(x, y, z, orientation, mapId);
            TransportId = 0;
            TransportOffset = new Position();
        }

        public SpellDestination(Position pos) : this()
        {
            Position = new WorldLocation();
            Position.Relocate(pos);
            TransportId = 0;
            TransportOffset = new Position();
        }

        public SpellDestination(WorldEntity entity) : this()
        {
            Position = new WorldLocation();
            Position.Relocate(entity.Position);
            TransportId = 0;
            TransportOffset = new Position();
        }

        public void Relocate(Position pos)
        {
            if (TransportId != 0)
            {
                Position offset = new Position();
                Position.GetPositionOffsetTo(pos, offset);
                TransportOffset.RelocateOffset(offset);
            }
            Position.Relocate(pos);
        }

        public void RelocateOffset(Position offset)
        {
            if (TransportId != 0)
                TransportOffset.RelocateOffset(offset);

            Position.RelocateOffset(offset);
        }
    }
}