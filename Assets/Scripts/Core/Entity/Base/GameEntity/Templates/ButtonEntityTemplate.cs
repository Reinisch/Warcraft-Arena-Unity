namespace Core
{
    public class ButtonEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Button;

        public uint StartOpen { get; set; }                             // 0 startOpen, enum { false, true, }; Default: false
        public uint Open { get; set; }                                  // 1 open, References: Lock_, NoValue = 0
        public uint AutoClose { get; set; }                             // 2 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 3000
        public uint LinkedTrap { get; set; }                            // 3 linkedTrap, References: GameObjects, NoValue = 0
        public uint NoDamageImmune { get; set; }                        // 4 noDamageImmune, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                           // 5 Gigantic AOI, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                            // 6 openTextID, References: BroadcastText, NoValue = 0
        public uint CloseTextID { get; set; }                           // 7 closeTextID, References: BroadcastText, NoValue = 0
        public uint RequireLos { get; set; }                            // 8 require LOS, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                          // 9 conditionID1, References: PlayerCondition, NoValue = 0

        public override uint LockId => Open;
        public override bool DespawnPossibility => NoDamageImmune != 0;
        public override uint LinkedEntityEntry => LinkedTrap;
        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
    }
}