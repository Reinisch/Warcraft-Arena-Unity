namespace Core
{
    public class SpellProcInfo
    {
        public Unit Actor { get; private set; }
        public Unit ActionTarget { get; private set; }
        public Unit ProcTarget { get; private set; }

        public uint TypeMask { get; private set; }
        public uint SpellTypeMask { get; private set; }
        public uint SpellPhaseMask { get; private set; }
        public uint HitMask { get; private set; }

        public SpellInfo SpellInfo => Spell.SpellInfo;
        public SpellSchoolMask SchoolMask { get; private set; }
        public SpellDamageInfo SpellDamageInfo { get; private set; }
        public SpellHealInfo SpellHealInfo { get; private set; }

        private Spell Spell { get; set; }

        public SpellProcInfo(Unit actor, Unit actionTarget, Unit procTarget, uint typeMask, uint spellTypeMask,
            uint spellPhaseMask, uint hitMask, Spell spell, SpellDamageInfo spellDamageInfo, SpellHealInfo spellHealInfo)
        {
            Actor = actor;
            ActionTarget = actionTarget;
            ProcTarget = procTarget;

            TypeMask = typeMask;
            SpellTypeMask = spellTypeMask;
            SpellPhaseMask = spellPhaseMask;
            HitMask = hitMask;

            Spell = spell;
            SpellDamageInfo = spellDamageInfo;
            SpellHealInfo = spellHealInfo;
        }
    }
}