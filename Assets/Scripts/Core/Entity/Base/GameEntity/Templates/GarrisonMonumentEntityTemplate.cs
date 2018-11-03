namespace Core
{
    public class GarrisonMonumentEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GarrisonMonument;

        public uint TrophyTypeID { get; set; }                            // 0 Trophy Type ID, References: TrophyType, NoValue = 0
        public uint TrophyInstanceID { get; set; }                        // 1 Trophy Instance ID, References: TrophyInstance, NoValue = 0
    }
}