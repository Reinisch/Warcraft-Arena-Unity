namespace Core
{
    public class GenericEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Generic;

        public uint FloatingTooltip { get; set; }                       // 0 floatingTooltip, enum { false, true, }; Default: false
        public uint Highlight { get; set; }                             // 1 highlight, enum { false, true, }; Default: true
        public uint ServerOnly { get; set; }                            // 2 serverOnly, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                           // 3 Gigantic AOI, enum { false, true, }; Default: false
        public uint FloatOnWater { get; set; }                          // 4 floatOnWater, enum { false, true, }; Default: false
        public uint QuestID { get; set; }                               // 5 questID, References: QuestV2, NoValue = 0
        public uint ConditionID1 { get; set; }                          // 6 conditionID1, References: PlayerCondition, NoValue = 0
        public uint LargeAoi { get; set; }                              // 7 Large AOI, enum { false, true, }; Default: false
        public uint UseGarrisonOwnerGuildColors { get; set; }           // 8 Use Garrison Owner Guild Colors, enum { false, true, }; Default: false
    }
}