namespace Core
{
    public struct SpellTriggerActivationInfo
    {
        public Unit Actor { get; private set; }
        public Unit ActionTarget { get; private set; }
        public Unit TriggerTarget { get; private set; }

        public SpellTriggerFlags SpellTriggerFlags { get; private set; }
        public SpellDamageInfo SpellDamageInfo { get; private set; }
        public SpellHealInfo SpellHealInfo { get; private set; }

        public Spell Spell { get; }

        public SpellTriggerActivationInfo(Unit actor, Unit actionTarget, Unit triggerTarget, Spell spell, 
            SpellTriggerFlags spellTriggerFlags, SpellDamageInfo spellDamageInfo, SpellHealInfo spellHealInfo)
        {
            Actor = actor;
            ActionTarget = actionTarget;
            TriggerTarget = triggerTarget;

            Spell = spell;
            SpellDamageInfo = spellDamageInfo;
            SpellHealInfo = spellHealInfo;
            SpellTriggerFlags = spellTriggerFlags;
        }
    }
}