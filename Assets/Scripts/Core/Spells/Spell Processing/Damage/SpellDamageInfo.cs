using System;

namespace Core
{
    public class SpellDamageInfo
    {
        public HitType HitInfo { get; set; }

        public SpellDamageType SpellDamageType { get; }
        public SpellSchoolMask SchoolMask { get; }
        public SpellInfo SpellInfo { get; }
        public Unit Target { get; }
        public Unit Caster { get; }
        public bool HasCrit { get; }

        public uint UnmitigatedDamage { get; private set; }
        public uint Damage { get; private set; }
        public uint Absorb { get; private set; }
        public uint Resist { get; private set; }

        public SpellDamageInfo(Unit caster, Unit target, SpellInfo spellInfo, uint originalDamage, bool hasCrit, SpellSchoolMask schoolMask, SpellDamageType spellDamageType)
        {
            Caster = caster;
            Target = target;
            SpellInfo = spellInfo;

            SchoolMask = schoolMask;
            SpellDamageType = spellDamageType;

            Damage = originalDamage;
            UnmitigatedDamage = originalDamage;
            HasCrit = hasCrit;

            if (HasCrit)
                HitInfo |= HitType.CriticalHit;
        }

        public void UpdateOriginalDamage(uint amount)
        {
            UnmitigatedDamage = Damage = amount;
        }

        public void UpdateDamage(uint amount)
        {
            Damage = amount;
        }

        public void AbsorbDamage(uint amount)
        {
            amount = Math.Min(amount, Damage);
            Absorb += amount;
            Damage -= amount;

            if (UnmitigatedDamage == Absorb)
                HitInfo |= HitType.FullAbsorb;
        }

        public void ResistDamage(uint amount)
        {
            amount = Math.Min(amount, Damage);
            Resist += amount;
            Damage -= amount;
        }
    }
}