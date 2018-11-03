namespace Core
{
    public class DoorEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Door;

        public uint StartOpen { get; set; }                         // 0 startOpen, enum { false, true, }; Default: false
        public uint Open { get; set; }                              // 1 open, References: Lock_, NoValue = 0
        public uint AutoClose { get; set; }                         // 2 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 3000
        public uint NoDamageImmune { get; set; }                    // 3 noDamageImmune, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                        // 4 openTextID, References: BroadcastText, NoValue = 0
        public uint CloseTextID { get; set; }                       // 5 closeTextID, References: BroadcastText, NoValue = 0
        public uint IgnoredByPathing { get; set; }                  // 6 Ignored By Pathing, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                      // 7 conditionID1, References: PlayerCondition, NoValue = 0
        public uint DoorisOpaque { get; set; }                      // 8 Door is Opaque (Disable portal on close), enum { false, true, }; Default: true
        public uint GiganticAoi { get; set; }                       // 9 Gigantic AOI, enum { false, true, }; Default: false
        public uint InfiniteAoi { get; set; }                       // 10 Infinite AOI, enum { false, true, }; Default: false

        public override uint LockId => Open;
        public override bool DespawnPossibility => NoDamageImmune != 0;
        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
    }
}