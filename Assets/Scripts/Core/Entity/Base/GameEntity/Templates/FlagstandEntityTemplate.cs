namespace Core
{
    public class FlagstandEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Flagstand;

        public uint Open { get; set; }                      // 0 open, References: Lock_, NoValue = 0
        public uint PickupSpell { get; set; }               // 1 pickupSpell, References: Spell, NoValue = 0
        public uint Radius { get; set; }                    // 2 radius, int, Min value: 0, Max value: 50, Default value: 0
        public uint ReturnAura { get; set; }                // 3 returnAura, References: Spell, NoValue = 0
        public uint ReturnSpell { get; set; }               // 4 returnSpell, References: Spell, NoValue = 0
        public uint NoDamageImmune { get; set; }            // 5 noDamageImmune, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                // 6 openTextID, References: BroadcastText, NoValue = 0
        public uint RequireLos { get; set; }                // 7 require LOS, enum { false, true, }; Default: true
        public uint ConditionID1 { get; set; }              // 8 conditionID1, References: PlayerCondition, NoValue = 0
        public uint PlayerCast { get; set; }                // 9 playerCast, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }               // 10 Gigantic AOI, enum { false, true, }; Default: false
        public uint InfiniteAoi { get; set; }               // 11 Infinite AOI, enum { false, true, }; Default: false
        public uint Cooldown { get; set; }                  // 12 cooldown, int, Min value: 0, Max value: 2147483647, Default value: 3000

        public override uint LockId => Open;
        public override bool DespawnPossibility => NoDamageImmune != 0;
    }
}