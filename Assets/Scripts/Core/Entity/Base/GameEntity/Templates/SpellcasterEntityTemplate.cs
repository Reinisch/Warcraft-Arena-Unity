namespace Core
{
    public class SpellcasterEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Spellcaster;

        public uint Spell { get; set; }                             // 0 spell, References: Spell, NoValue = 0
        public uint Charges { get; set; }                           // 1 charges, int, Min value: -1, Max value: 65535, Default value: 1
        public uint PartyOnly { get; set; }                         // 2 partyOnly, enum { false, true, }; Default: false
        public uint AllowMounted { get; set; }                      // 3 allowMounted, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                       // 4 Gigantic AOI, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }                      // 5 conditionID1, References: PlayerCondition, NoValue = 0
        public uint PlayerCast { get; set; }                        // 6 playerCast, enum { false, true, }; Default: false
        public uint NeverUsableWhileMounted { get; set; }           // 7 Never Usable While Mounted, enum { false, true, }; Default: false

        public override bool IsUsableMounted => AllowMounted != 0;
        public override uint UseCharges => Charges;
    }
}