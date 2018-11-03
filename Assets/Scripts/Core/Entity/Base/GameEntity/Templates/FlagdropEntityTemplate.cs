namespace Core
{
    public class FlagdropEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Flagdrop;

        public uint Open { get; set; }                      // 0 open, References: Lock_, NoValue = 0
        public uint EventID { get; set; }                   // 1 eventID, References: GameEvents, NoValue = 0
        public uint PickupSpell { get; set; }               // 2 pickupSpell, References: Spell, NoValue = 0
        public uint NoDamageImmune { get; set; }            // 3 noDamageImmune, enum { false, true, }; Default: false
        public uint OpenTextID { get; set; }                // 4 openTextID, References: BroadcastText, NoValue = 0
        public uint PlayerCast { get; set; }                // 5 playerCast, enum { false, true, }; Default: false
        public uint ExpireDuration { get; set; }            // 6 Expire Duration, int, Min value: 0, Max value: 60000, Default value: 10000
        public uint GiganticAoi { get; set; }               // 7 Gigantic AOI, enum { false, true, }; Default: false
        public uint InfiniteAoi { get; set; }               // 8 Infinite AOI, enum { false, true, }; Default: false
        public uint Cooldown { get; set; }                  // 9 cooldown, int, Min value: 0, Max value: 2147483647, Default value: 3000

        public override uint LockId => Open;
        public override bool DespawnPossibility => NoDamageImmune != 0;
    }
}