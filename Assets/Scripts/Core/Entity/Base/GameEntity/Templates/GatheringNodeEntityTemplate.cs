namespace Core
{
    public class GatheringNodeEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GatheringNode;

        public uint Open { get; set; }                                    // 0 open, References: Lock_, NoValue = 0
        public uint ChestLoot { get; set; }                               // 1 chestLoot, References: Treasure, NoValue = 0
        public uint Level { get; set; }                                   // 2 level, int, Min value: 0, Max value: 65535, Default value: 0
        public uint NotInCombat { get; set; }                             // 3 notInCombat, enum { false, true, }; Default: false
        public uint TrivialSkillLow { get; set; }                         // 4 trivialSkillLow, int, Min value: 0, Max value: 65535, Default value: 0
        public uint TrivialSkillHigh { get; set; }                        // 5 trivialSkillHigh, int, Min value: 0, Max value: 65535, Default value: 0
        public uint ObjectDespawnDelay { get; set; }                      // 6 Object Despawn Delay, int, Min value: 0, Max value: 600, Default value: 15
        public uint TriggeredEvent { get; set; }                          // 7 triggeredEvent, References: GameEvents, NoValue = 0
        public uint RequireLos { get; set; }                              // 8 require LOS, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                              // 9 openTextID, References: BroadcastText, NoValue = 0
        public uint FloatingTooltip { get; set; }                         // 10 floatingTooltip, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                            // 11 conditionID1, References: PlayerCondition, NoValue = 0
        public uint XpLevel { get; set; }                                 // 12 xpLevel, int, Min value: -1, Max value: 123, Default value: 0
        public uint XpDifficulty { get; set; }                            // 13 xpDifficulty, enum { No Exp, Trivial, Very Small, Small, Substandard, Standard, High, Epic, Dungeon, 5, }; Default: No Exp
        public uint Spell { get; set; }                                   // 14 spell, References: Spell, NoValue = 0
        public uint GiganticAoi { get; set; }                             // 15 Gigantic AOI, enum { false, true, }; Default: false
        public uint LargeAoi { get; set; }                                // 16 Large AOI, enum { false, true, }; Default: false
        public uint SpawnVignette { get; set; }                           // 17 Spawn Vignette, References: vignette, NoValue = 0
        public uint MaxNumberofLoots { get; set; }                        // 18 Max Number of Loots, int, Min value: 1, Max value: 40, Default value: 10
        public uint Logloot { get; set; }                                 // 19 log loot, enum { false, true, }; Default: false
        public uint LinkedTrap { get; set; }                              // 20 linkedTrap, References: GameObjects, NoValue = 0
    }
}