namespace Core
{
    public class SpellAuraOptionsEntry
    {
        public uint ID;
        public uint SpellID;
        public uint ProcCharges;
        public ProcFlags ProcTypeMask;
        public uint ProcCategoryRecovery;
        public uint CumulativeAura;
        public byte DifficultyID;
        public uint ProcChance;
        public byte SpellProcsPerMinuteID;
    }
}