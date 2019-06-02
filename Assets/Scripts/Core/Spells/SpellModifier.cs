namespace Core
{
    public class SpellModifier
    {
        private Aura OwnerAura { get; set; }
        private SpellModOp Mod { get; set; }
        private SpellModType Type { get; set; }

        public short Charges { get; set; }
        public int Value { get; set; }
        public uint SpellId { get; set; }

        public SpellModifier(Aura ownerAura)
        {
            OwnerAura = ownerAura;
            Mod = SpellModOp.Damage;
            Type = SpellModType.Flat;
        }
    }
}