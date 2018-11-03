namespace Core
{
    public class AuraGeneratorEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.AuraGenerator;

        public uint StartOpen { get; set; }                               // 0 startOpen, enum { false, true, }; Default: true
        public uint Radius { get; set; }                                  // 1 radius, int, Min value: 0, Max value: 100, Default value: 10
        public uint AuraID1 { get; set; }                                 // 2 auraID1, References: Spell, NoValue = 0
        public uint ConditionID1 { get; set; }                            // 3 conditionID1, References: PlayerCondition, NoValue = 0
        public uint AuraID2 { get; set; }                                 // 4 auraID2, References: Spell, NoValue = 0
        public uint ConditionID2 { get; set; }                            // 5 conditionID2, References: PlayerCondition, NoValue = 0
        public uint ServerOnly { get; set; }                              // 6 serverOnly, enum { false, true, }; Default: false
    }
}