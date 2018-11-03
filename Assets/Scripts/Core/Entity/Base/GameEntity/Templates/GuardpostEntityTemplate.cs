namespace Core
{
    public class GuardpostEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Guardpost;

        public uint CreatureID { get; set; }                // 0 creatureID, References: Creature, NoValue = 0
        public uint Charges { get; set; }                   // 1 charges, int, Min value: 0, Max value: 65535, Default value: 1

        public override uint UseCharges => Charges;
    }
}