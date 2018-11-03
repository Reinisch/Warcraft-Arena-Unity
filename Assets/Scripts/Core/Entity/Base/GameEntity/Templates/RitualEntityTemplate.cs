namespace Core
{
    public class RitualEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Ritual;

        public uint Casters { get; set; }                               // 0 casters, int, Min value: 1, Max value: 10, Default value: 1
        public uint Spell { get; set; }                                 // 1 spell, References: Spell, NoValue = 0
        public uint AnimSpell { get; set; }                             // 2 animSpell, References: Spell, NoValue = 0
        public uint RitualPersistent { get; set; }                      // 3 ritualPersistent, enum { false, true, }; Default: false
        public uint CasterTargetSpell { get; set; }                     // 4 casterTargetSpell, References: Spell, NoValue = 0
        public uint CasterTargetSpellTargets { get; set; }              // 5 casterTargetSpellTargets, int, Min value: 1, Max value: 10, Default value: 1
        public uint CastersGrouped { get; set; }                        // 6 castersGrouped, enum { false, true, }; Default: true
        public uint RitualNoTargetCheck { get; set; }                   // 7 ritualNoTargetCheck, enum { false, true, }; Default: true
        public uint ConditionID1 { get; set; }                          // 8 conditionID1, References: PlayerCondition, NoValue = 0
    }
}