namespace Core
{
    public struct SpellImmune
    {
        public uint Type;
        public uint SpellId;

        public SpellImmune(uint type, uint spellId)
        {
            Type = type;
            SpellId = spellId;
        }
    }
}