namespace Core
{
    public class GarrisonMonumentPlaqueEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GarrisonMonumentPlaque;

        public uint TrophyInstanceID { get; set; }                  // 0 Trophy Instance ID, References: TrophyInstance, NoValue = 0
    }
}