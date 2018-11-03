namespace Core
{
    public class ChairEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Chair;

        public uint Chairslots { get; set; }                              // 0 chairslots, int, Min value: 1, Max value: 5, Default value: 1
        public uint Chairheight { get; set; }                             // 1 chairheight, int, Min value: 0, Max value: 2, Default value: 1
        public uint OnlyCreatorUse { get; set; }                          // 2 onlyCreatorUse, enum { false, true, }; Default: false
        public uint TriggeredEvent { get; set; }                          // 3 triggeredEvent, References: GameEvents, NoValue = 0
        public uint ConditionID1 { get; set; }                            // 4 conditionID1, References: PlayerCondition, NoValue = 0
    }
}