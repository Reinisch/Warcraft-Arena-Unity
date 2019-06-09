using System;

namespace Core
{
    public class SpellHealInfo
    {
        public Unit Healer { get; private set; }
        public Unit Target { get; private set; }
        public uint Heal { get; private set; }
        public uint Absorb { get; private set; }
        public SpellInfo SpellInfo { get; private set; }
        public SpellSchoolMask SchoolMask { get; private set; }

        public SpellHealInfo(Unit healer, Unit target, uint heal, SpellInfo spellInfo, SpellSchoolMask schoolMask)
        {
            Healer = healer;
            Target = target;
            Heal = heal;
            SpellInfo = spellInfo;
            SchoolMask = schoolMask;
        }

        public void AbsorbHeal(uint amount)
        {
            amount = Math.Min(amount, Heal);

            Absorb += amount;
            Heal -= amount;
        }
    }
}