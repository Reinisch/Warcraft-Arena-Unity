namespace Core
{
    public class ChestEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Chest;

        public uint Open { get; set; }                                  // 0 open, References: Lock_, NoValue = 0
        public uint ChestLoot { get; set; }                             // 1 chestLoot, References: Treasure, NoValue = 0
        public uint ChestRestockTime { get; set; }                      // 2 chestRestockTime, int, Min value: 0, Max value: 1800000, Default value: 0
        public uint Consumable { get; set; }                            // 3 consumable, enum { false, true, }; Default: false
        public uint MinRestock { get; set; }                            // 4 minRestock, int, Min value: 0, Max value: 65535, Default value: 0
        public uint MaxRestock { get; set; }                            // 5 maxRestock, int, Min value: 0, Max value: 65535, Default value: 0
        public uint TriggeredEvent { get; set; }                        // 6 triggeredEvent, References: GameEvents, NoValue = 0
        public uint LinkedTrap { get; set; }                            // 7 linkedTrap, References: GameObjects, NoValue = 0
        public uint QuestID { get; set; }                               // 8 questID, References: QuestV2, NoValue = 0
        public uint Level { get; set; }                                 // 9 level, int, Min value: 0, Max value: 65535, Default value: 0
        public uint RequireLos { get; set; }                            // 10 require LOS, enum { false, true, }; Default: false
        public uint LeaveLoot { get; set; }                             // 11 leaveLoot, enum { false, true, }; Default: false
        public uint NotInCombat { get; set; }                           // 12 notInCombat, enum { false, true, }; Default: false
        public uint Logloot { get; set; }                               // 13 log loot, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                            // 14 openTextID, References: BroadcastText, NoValue = 0
        public uint Usegrouplootrules { get; set; }                     // 15 use group loot rules, enum { false, true, }; Default: false
        public uint FloatingTooltip { get; set; }                       // 16 floatingTooltip, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                          // 17 conditionID1, References: PlayerCondition, NoValue = 0
        public int XpLevel { get; set; }                                // 18 xpLevel, int, Min value: -1, Max value: 123, Default value: 0
        public uint XpDifficulty { get; set; }                          // 19 xpDifficulty, enum { No Exp, Trivial, Very Small, Small, Substandard, Standard, High, Epic, Dungeon, 5, }; Default: No Exp
        public uint LootLevel { get; set; }                             // 20 lootLevel, int, Min value: 0, Max value: 123, Default value: 0
        public uint GroupXp { get; set; }                               // 21 Group XP, enum { false, true, }; Default: false
        public uint DamageImmuneOk { get; set; }                        // 22 Damage Immune OK, enum { false, true, }; Default: false
        public uint TrivialSkillLow { get; set; }                       // 23 trivialSkillLow, int, Min value: 0, Max value: 65535, Default value: 0
        public uint TrivialSkillHigh { get; set; }                      // 24 trivialSkillHigh, int, Min value: 0, Max value: 65535, Default value: 0
        public uint DungeonEncounter { get; set; }                      // 25 Dungeon Encounter, References: DungeonEncounter, NoValue = 0
        public uint Spell { get; set; }                                 // 26 spell, References: Spell, NoValue = 0
        public uint GiganticAoi { get; set; }                           // 27 Gigantic AOI, enum { false, true, }; Default: false
        public uint LargeAoi { get; set; }                              // 28 Large AOI, enum { false, true, }; Default: false
        public uint SpawnVignette { get; set; }                         // 29 Spawn Vignette, References: vignette, NoValue = 0
        public uint ChestPersonalLoot { get; set; }                     // 30 chest Personal Loot, References: Treasure, NoValue = 0
        public uint Turnpersonallootsecurityoff { get; set; }           // 31 turn personal loot security off, enum { false, true, }; Default: false
        public uint ChestProperties { get; set; }                       // 32 Chest Properties, References: ChestProperties, NoValue = 0

        public override bool IsDespawnAtAction => Consumable != 0;
        public override uint LockId => Open;
        public override uint LinkedEntityEntry => LinkedTrap;
        public override uint LootId => ChestLoot;
        public override uint EventScriptId => TriggeredEvent;
    }
}