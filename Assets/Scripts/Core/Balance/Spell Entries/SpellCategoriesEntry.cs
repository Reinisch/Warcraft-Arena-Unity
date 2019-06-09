namespace Core
{
    public class SpellCategoriesEntry
    {
        public uint ID;
        public uint SpellID;
        public uint Category;
        public uint StartRecoveryCategory;
        public uint ChargeCategory;
        public byte DifficultyID;
        public SpellDamageClass DefenseType;
        public SpellDispelType SpellDispelType;
        public SpellMechanics Mechanic;
        public SpellPreventionType PreventionType;
    }
}