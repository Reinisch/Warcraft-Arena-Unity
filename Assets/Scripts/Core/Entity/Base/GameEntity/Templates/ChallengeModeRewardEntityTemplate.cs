namespace Core
{
    public class ChallengeModeRewardEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.ChallengeModeReward;

        public uint ChestLoot { get; set; }                          // 0 chestLoot, References: Treasure, NoValue = 0
        public uint WhenAvailable { get; set; }                      // 1 When Available, References: GameObjectDisplayInfo, NoValue = 0
    }
}