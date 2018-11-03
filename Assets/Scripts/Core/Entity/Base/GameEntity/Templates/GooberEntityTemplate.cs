namespace Core
{
    public class GooberEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Goober;

        public uint Open { get; set; }                                  // 0 open, References: Lock_, NoValue = 0
        public uint QuestID { get; set; }                               // 1 questID, References: QuestV2, NoValue = 0
        public uint EventID { get; set; }                               // 2 eventID, References: GameEvents, NoValue = 0
        public uint AutoClose { get; set; }                             // 3 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 3000
        public uint CustomAnim { get; set; }                            // 4 customAnim, int, Min value: 0, Max value: 4, Default value: 0
        public uint Consumable { get; set; }                            // 5 consumable, enum { false, true, }; Default: false
        public uint Cooldown { get; set; }                              // 6 cooldown, int, Min value: 0, Max value: 65535, Default value: 0
        public uint PageID { get; set; }                                // 7 pageID, References: PageText, NoValue = 0
        public uint Language { get; set; }                              // 8 language, References: Languages, NoValue = 0
        public uint PageMaterial { get; set; }                          // 9 pageMaterial, References: PageTextMaterial, NoValue = 0
        public uint Spell { get; set; }                                 // 10 spell, References: Spell, NoValue = 0
        public uint NoDamageImmune { get; set; }                        // 11 noDamageImmune, enum { false, true, }; Default: false
        public uint LinkedTrap { get; set; }                            // 12 linkedTrap, References: GameObjects, NoValue = 0
        public uint GiganticAoi { get; set; }                           // 13 Gigantic AOI, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                            // 14 openTextID, References: BroadcastText, NoValue = 0
        public uint CloseTextID { get; set; }                           // 15 closeTextID, References: BroadcastText, NoValue = 0
        public uint RequireLos { get; set; }                            // 16 require LOS, enum { false, true, }; Default: false
        public uint AllowMounted { get; set; }                          // 17 allowMounted, enum { false, true, }; Default: false
        public uint FloatingTooltip { get; set; }                       // 18 floatingTooltip, enum { false, true, }; Default: false
        public uint GossipID { get; set; }                              // 19 gossipID, References: Gossip, NoValue = 0
        public uint AllowMultiInteract;                                 // 20 Allow Multi-Interact, enum { false, true, }; Default: false
        public uint FloatOnWater { get; set; }                          // 21 floatOnWater, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                          // 22 conditionID1, References: PlayerCondition, NoValue = 0
        public uint PlayerCast { get; set; }                            // 23 playerCast, enum { false, true, }; Default: false
        public uint SpawnVignette { get; set; }                         // 24 Spawn Vignette, References: vignette, NoValue = 0
        public uint StartOpen { get; set; }                             // 25 startOpen, enum { false, true, }; Default: false
        public uint DontPlayOpenAnim { get; set; }                      // 26 Dont Play Open Anim, enum { false, true, }; Default: false
        public uint IgnoreBoundingBox { get; set; }                     // 27 Ignore Bounding Box, enum { false, true, }; Default: false
        public uint NeverUsableWhileMounted { get; set; }               // 28 Never Usable While Mounted, enum { false, true, }; Default: false
        public uint SortFarZ { get; set; }                              // 29 Sort Far Z, enum { false, true, }; Default: false
        public uint SyncAnimationtoObjectLifetime { get; set; }         // 30 Sync Animation to Object Lifetime (global track only), enum { false, true, }; Default: false
        public uint NoFuzzyHit { get; set; }                            // 31 No Fuzzy Hit, enum { false, true, }; Default: false

        public override bool IsDespawnAtAction => Consumable != 0;
        public override bool IsUsableMounted => AllowMounted != 0;
        public override uint LockId => Open;
        public override uint GossipMenuId => GossipID;
        public override uint EventScriptId => EventID;

        public override bool DespawnPossibility => NoDamageImmune != 0;
        public override uint LinkedEntityEntry => LinkedTrap;
        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
        public override uint CastCooldown => Cooldown;
    }
}