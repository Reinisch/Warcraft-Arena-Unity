namespace Core
{
    public class MeetingStoneEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.MeetingStone;

        public uint MinLevel { get; set; }                                // 0 minLevel, int, Min value: 0, Max value: 65535, Default value: 1
        public uint MaxLevel { get; set; }                                // 1 maxLevel, int, Min value: 1, Max value: 65535, Default value: 60
        public uint AreaID { get; set; }                                  // 2 areaID, References: AreaTable, NoValue = 0
    }
}