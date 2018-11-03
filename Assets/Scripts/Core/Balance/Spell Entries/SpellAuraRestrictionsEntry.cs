namespace Core
{
    public class SpellAuraRestrictionsEntry
    {
        public uint ID;
        public uint SpellID;
        public uint CasterAuraSpell;
        public uint TargetAuraSpell;
        public uint ExcludeCasterAuraSpell;
        public uint ExcludeTargetAuraSpell;
        public byte DifficultyID;
        public AuraStateType CasterAuraState;
        public AuraStateType TargetAuraState;
        public AuraStateType ExcludeCasterAuraState;
        public AuraStateType ExcludeTargetAuraState;
    }
}