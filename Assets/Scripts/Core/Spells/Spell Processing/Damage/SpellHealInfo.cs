using System;

namespace Core
{
    public struct SpellHealInfo
    {
        public HitType HitInfo { get; private set; }
        public SpellInfo SpellInfo { get; }
        public Unit Healer { get; }
        public Unit Target { get; }
        public bool HasCrit { get; }

        public uint UnmitigatedHeal { get; private set; }
        public uint Heal { get; private set; }
        public uint Absorb { get; private set; }

        public SpellHealInfo(Unit healer, Unit target, SpellInfo spellInfo, uint heal, bool crit)
        {
            Healer = healer;
            Target = target;
            SpellInfo = spellInfo;
            HasCrit = crit;

            UnmitigatedHeal = heal;
            Heal = heal;

            HitInfo = 0;
            Absorb = 0;

            if (HasCrit)
                HitInfo |= HitType.CriticalHit;
        }

        public void UpdateBase(uint amount)
        {
            UnmitigatedHeal = Heal = amount;
        }

        public void AbsorbHeal(uint amount)
        {
            amount = Math.Min(amount, Heal);

            Absorb += amount;
            Heal -= amount;

            if (UnmitigatedHeal == Absorb)
                HitInfo |= HitType.FullAbsorb;
        }
    }
}