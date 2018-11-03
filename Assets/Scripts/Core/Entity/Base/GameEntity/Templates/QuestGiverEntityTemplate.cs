namespace Core
{
    public class QuestGiverEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.QuestGiver;

        public uint Open { get; set; }                                   // 0 open, References: Lock_, NoValue = 0
        public uint QuestGiver { get; set; }                              // 1 questGiver, References: QuestGiver, NoValue = 0
        public uint PageMaterial { get; set; }                            // 2 pageMaterial, References: PageTextMaterial, NoValue = 0
        public uint GossipID { get; set; }                                // 3 gossipID, References: Gossip, NoValue = 0
        public uint CustomAnim { get; set; }                              // 4 customAnim, int, Min value: 0, Max value: 4, Default value: 0
        public uint NoDamageImmune { get; set; }                          // 5 noDamageImmune, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                               // 6 openTextID, References: BroadcastText, NoValue = 0
        public uint RequireLos { get; set; }                              // 7 require LOS, enum { false, true, }; Default: false
        public uint AllowMounted { get; set; }                            // 8 allowMounted, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                              // 9 Gigantic AOI, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                            // 10 conditionID1, References: PlayerCondition, NoValue = 0
        public uint NeverUsableWhileMounted { get; set; }                // 11 Never Usable While Mounted, enum { false, true, }; Default: false

        public override bool IsUsableMounted => AllowMounted != 0;
        public override uint LockId => Open;
        public override uint GossipMenuId => GossipID;
        public override bool DespawnPossibility => NoDamageImmune != 0;
    }
}