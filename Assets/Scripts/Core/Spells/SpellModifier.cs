namespace Core
{
    public class SpellModifier
    {
        public Aura OwnerAura { get; private set; }
        public SpellModOp Mod { get; set; }
        public SpellModType Type { get; set; }

        public short Charges { get; set; }
        public int Value { get; set; }
        public Flag128 Mask { get; set; }
        public uint SpellId { get; set; }

        public SpellModifier(Aura ownerAura)
        {
            OwnerAura = ownerAura;
            Mod = SpellModOp.Damage;
            Type = SpellModType.Flat;
        }
    }
}