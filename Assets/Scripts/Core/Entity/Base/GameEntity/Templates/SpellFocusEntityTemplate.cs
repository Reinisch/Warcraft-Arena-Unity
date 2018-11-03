namespace Core
{
    public class SpellFocusEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.SpellFocus;

        public uint SpellFocusType { get; set; }                // 0 spellFocusType, References: SpellFocusObject, NoValue = 0
        public uint Radius { get; set; }                        // 1 radius, int, Min value: 0, Max value: 50, Default value: 10
        public uint LinkedTrap { get; set; }                    // 2 linkedTrap, References: GameObjects, NoValue = 0
        public uint ServerOnly { get; set; }                    // 3 serverOnly, enum { false, true, }; Default: false
        public uint QuestID { get; set; }                       // 4 questID, References: QuestV2, NoValue = 0
        public uint GiganticAoi { get; set; }                   // 5 Gigantic AOI, enum { false, true, }; Default: false
        public uint FloatingTooltip { get; set; }               // 6 floatingTooltip, enum { false, true, }; Default: false
        public uint FloatOnWater { get; set; }                  // 7 floatOnWater, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                  // 8 conditionID1, References: PlayerCondition, NoValue = 0

        public override uint LinkedEntityEntry => LinkedTrap;
    }
}