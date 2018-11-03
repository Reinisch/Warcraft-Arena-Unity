namespace Core
{
    public class NewFlagEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.NewFlag;

        public uint Open { get; set; }                                    // 0 open, References: Lock_, NoValue = 0
        public uint PickupSpell { get; set; }                             // 1 pickupSpell, References: Spell, NoValue = 0
        public uint OpenTextID { get; set; }                              // 2 openTextID, References: BroadcastText, NoValue = 0
        public uint RequireLos { get; set; }                              // 3 require LOS, enum { false, true, }; Default: true
        public uint ConditionID1 { get; set; }                            // 4 conditionID1, References: PlayerCondition, NoValue = 0
        public uint GiganticAoi { get; set; }                             // 5 Gigantic AOI, enum { false, true, }; Default: false
        public uint InfiniteAoi { get; set; }                             // 6 Infinite AOI, enum { false, true, }; Default: false
        public uint ExpireDuration { get; set; }                          // 7 Expire Duration, int, Min value: 0, Max value: 3600000, Default value: 10000
        public uint RespawnTime { get; set; }                             // 8 Respawn Time, int, Min value: 0, Max value: 3600000, Default value: 20000
        public uint FlagDrop { get; set; }                                // 9 Flag Drop, References: GameObjects, NoValue = 0
        public int ExclusiveCategory { get; set; }                        // 10 Exclusive Category (BGs Only), int, Min value: -2147483648, Max value: 2147483647, Default value: 0
        public uint WorldState1 { get; set; }                             // 11 worldState1, References: WorldState, NoValue = 0
        public uint ReturnonDefenderInteract { get; set; }                // 12 Return on Defender Interact, enum { false, true, }; Default: false

        public override uint LockId => Open;
    }
}