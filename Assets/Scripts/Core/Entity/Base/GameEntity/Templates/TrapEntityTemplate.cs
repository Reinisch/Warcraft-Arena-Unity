namespace Core
{
    public class TrapEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Trap;

        public uint Open { get; set; }                                  // 0 open, References: Lock_, NoValue = 0
        public uint Level { get; set; }                                 // 1 level, int, Min value: 0, Max value: 65535, Default value: 0
        public uint Radius { get; set; }                                // 2 radius, int, Min value: 0, Max value: 100, Default value: 0
        public uint Spell { get; set; }                                 // 3 spell, References: Spell, NoValue = 0
        public uint Charges { get; set; }                               // 4 charges, int, Min value: 0, Max value: 65535, Default value: 1
        public uint Cooldown { get; set; }                              // 5 cooldown, int, Min value: 0, Max value: 65535, Default value: 0
        public uint AutoClose { get; set; }                             // 6 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint StartDelay { get; set; }                            // 7 startDelay, int, Min value: 0, Max value: 65535, Default value: 0
        public uint ServerOnly { get; set; }                            // 8 serverOnly, enum { false, true, }; Default: false
        public uint Stealthed { get; set; }                             // 9 stealthed, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                           // 10 Gigantic AOI, enum { false, true, }; Default: false
        public uint StealthAffected { get; set; }                       // 11 stealthAffected, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                            // 12 openTextID, References: BroadcastText, NoValue = 0
        public uint CloseTextID { get; set; }                           // 13 closeTextID, References: BroadcastText, NoValue = 0
        public uint IgnoreTotems { get; set; }                          // 14 Ignore Totems, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                          // 15 conditionID1, References: PlayerCondition, NoValue = 0
        public uint PlayerCast { get; set; }                            // 16 playerCast, enum { false, true, }; Default: false
        public uint SummonerTriggered { get; set; }                     // 17 Summoner Triggered, enum { false, true, }; Default: false
        public uint RequireLos { get; set; }                            // 18 require LOS, enum { false, true, }; Default: false

        public override uint LockId => Open;
        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
        public override uint CastCooldown => Cooldown;
    }
}