using System;

namespace Core
{
    public struct SpellDamageInfo
    {
        public HitType HitType { get; private set; }
        public SpellDamageType SpellDamageType { get; }
        public SpellInfo SpellInfo { get; }
        public Unit Target { get; }
        public Unit Caster { get; }
        public bool HasCrit { get; }

        public uint Damage { get; private set; }
        public uint Absorb { get; private set; }
        public uint Resist { get; private set; }

        public SpellDamageInfo(Unit caster, Unit target, SpellInfo spellInfo, uint originalDamage, bool hasCrit, SpellDamageType spellDamageType)
        {
            Caster = caster;
            Target = target;
            SpellInfo = spellInfo;

            SpellDamageType = spellDamageType;

            Damage = originalDamage;
            HasCrit = hasCrit;

            HitType = 0;
            Absorb = 0;
            Resist = 0;

            if (HasCrit)
                HitType |= HitType.CriticalHit;
        }

        public SpellDamageInfo(Unit caster, Unit target, SpellInfo spellInfo, SpellDamageType spellDamageType, HitType hitType)
        {
            Caster = caster;
            Target = target;
            SpellInfo = spellInfo;

            SpellDamageType = spellDamageType;
            HitType = hitType;

            Damage = 0;
            Absorb = 0;
            Resist = 0;
            HasCrit = false;
        }

        public void UpdateDamage(uint amount)
        {
            Damage = amount;
        }

        public void AbsorbDamage(uint amount)
        {
            if (Damage > 0)
            {
                amount = Math.Min(amount, Damage);
                Absorb += amount;
                Damage -= amount;

                if (Damage == 0)
                    HitType |= HitType.FullAbsorb;
            }
        }

        public void ResistDamage(uint amount)
        {
            amount = Math.Min(amount, Damage);
            Resist += amount;
            Damage -= amount;
        }
    }
}