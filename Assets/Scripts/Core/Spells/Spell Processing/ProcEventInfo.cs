namespace Core
{
    public class ProcEventInfo
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
        public DamageInfo DamageInfo { get; private set; }
        public HealInfo HealInfo { get; private set; }

        private Spell Spell { get; set; }

        public ProcEventInfo(Unit actor, Unit actionTarget, Unit procTarget, uint typeMask, uint spellTypeMask,
            uint spellPhaseMask, uint hitMask, Spell spell, DamageInfo damageInfo, HealInfo healInfo)
        {
            Actor = actor;
            ActionTarget = actionTarget;
            ProcTarget = procTarget;

            TypeMask = typeMask;
            SpellTypeMask = spellTypeMask;
            SpellPhaseMask = spellPhaseMask;
            HitMask = hitMask;

            Spell = spell;
            DamageInfo = damageInfo;
            HealInfo = healInfo;
        }
    }
}