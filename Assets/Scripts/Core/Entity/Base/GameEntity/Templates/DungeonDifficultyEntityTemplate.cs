namespace Core
{
    public class DungeonDifficultyEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.DungeonDifficulty;

        public uint InstanceType { get; set; }                          // 0 Instance Type, enum { Not Instanced, Party Dungeon, Raid Dungeon, PVP Battlefield, Arena Battlefield, Scenario, }; Default: Party Dungeon
        public uint DifficultyNormal { get; set; }                      // 1 Difficulty Normal, References: animationdata, NoValue = 0
        public uint DifficultyHeroic { get; set; }                      // 2 Difficulty Heroic, References: animationdata, NoValue = 0
        public uint DifficultyEpic { get; set; }                        // 3 Difficulty Epic, References: animationdata, NoValue = 0
        public uint DifficultyLegendary { get; set; }                   // 4 Difficulty Legendary, References: animationdata, NoValue = 0
        public uint HeroicAttachment { get; set; }                      // 5 Heroic Attachment, References: gameobjectdisplayinfo, NoValue = 0
        public uint ChallengeAttachment { get; set; }                   // 6 Challenge Attachment, References: gameobjectdisplayinfo, NoValue = 0
        public uint DifficultyAnimations { get; set; }                  // 7 Difficulty Animations, References: GameObjectDiffAnim, NoValue = 0
        public uint LargeAoi { get; set; }                              // 8 Large AOI, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                           // 9 Gigantic AOI, enum { false, true, }; Default: false
        public uint Legacy { get; set; }                                // 10 Legacy, enum { false, true, }; Default: false
    }
}