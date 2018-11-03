using System;

namespace Core
{
    public class DamageInfo
    {
        public Unit Attacker { get; private set; }
        public Unit Victim { get; private set; }
        public uint Damage { get; private set; }
        public SpellInfo SpellInfo { get; private set; }
        public SpellSchoolMask SchoolMask { get; private set; }
        public DamageEffectType DamageType { get; private set; }
        public uint Absorb { get; private set; }
        public uint Resist { get; private set; }
        public uint Block { get; private set; }

        public DamageInfo(Unit attacker, Unit victim, uint damage, SpellInfo spellInfo, SpellSchoolMask schoolMask, DamageEffectType damageType)
        {
            Attacker = attacker;
            Victim = victim;
            Damage = damage;
            SpellInfo = spellInfo;
            SchoolMask = schoolMask;
            DamageType = damageType;
        }

        public void IncreaseDamage(uint amount)
        {
            Damage += amount;
        }

        public void ReduceDamage(uint amount)
        {
            Damage -= Math.Min(amount, Damage);
        }

        public void AbsorbDamage(uint amount)
        {
            amount = Math.Min(amount, Damage);
            Absorb += amount;
            Damage -= amount;
        }

        public void ResistDamage(uint amount)
        {
            amount = Math.Min(amount, Damage);
            Resist += amount;
            Damage -= amount;
        }

        public void BlockDamage(uint amount)
        {
            amount = Math.Min(amount, Damage);
            Block += amount;
            Damage -= amount;
        }
    }
}