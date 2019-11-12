namespace Core.AuraEffects
{
    public class AuraEffectPeriodicHealing : AuraEffectPeriodic
    {
        public new AuraEffectInfoPeriodicHealing EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectPeriodicHealing(Aura aura, AuraEffectInfoPeriodicHealing effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        protected override void HandlePeriodic(Unit target, Unit caster)
        {
            if (target.IsDead || caster == null)
                return;

            if (target.HasState(UnitControlState.Isolated))
                return;

            int originalHeal = EffectInfo.CalculateSpellHeal(caster);
            bool hasCrit = caster.Spells.IsSpellCrit(target, Aura.SpellInfo, Aura.SpellInfo.SchoolMask);
            caster.Spells.HealBySpell(new SpellHealInfo(caster, target, Aura.SpellInfo, (uint)originalHeal, hasCrit));
        }
    }
}