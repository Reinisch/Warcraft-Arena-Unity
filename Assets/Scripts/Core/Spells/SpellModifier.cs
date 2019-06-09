namespace Core
{
    public class SpellModifier
    {
        private Aura OwnerAura { get; set; }
        private SpellModifierType Mod { get; set; }
        private SpellModifierApplicationType Type { get; set; }

        public short Charges { get; set; }
        public int Value { get; set; }
        public uint SpellId { get; set; }

        public SpellModifier(Aura ownerAura)
        {
            OwnerAura = ownerAura;
            Mod = SpellModifierType.Damage;
            Type = SpellModifierApplicationType.Flat;
        }
    }
}