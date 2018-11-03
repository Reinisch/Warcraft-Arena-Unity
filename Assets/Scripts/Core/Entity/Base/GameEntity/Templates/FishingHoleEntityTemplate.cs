namespace Core
{
    public class FishingHoleEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.FishingHole;

        public uint Radius { get; set; }                    // 0 radius, int, Min value: 0, Max value: 50, Default value: 0
        public uint ChestLoot { get; set; }                 // 1 chestLoot, References: Treasure, NoValue = 0
        public uint MinRestock { get; set; }                // 2 minRestock, int, Min value: 0, Max value: 65535, Default value: 0
        public uint MaxRestock { get; set; }                // 3 maxRestock, int, Min value: 0, Max value: 65535, Default value: 0
        public uint Open { get; set; }                      // 4 open, References: Lock_, NoValue = 0

        public override uint LockId => Open;
        public override uint LootId => ChestLoot;
    }
}