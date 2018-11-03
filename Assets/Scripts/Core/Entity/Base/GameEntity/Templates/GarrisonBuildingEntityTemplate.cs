namespace Core
{
    public class GarrisonBuildingEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GarrisonBuilding;

        public int SpawnMap { get; set; }                   // 0 Spawn Map, References: Map, NoValue = -1
    }
}