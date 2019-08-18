namespace Core.AuraEffects
{
    public class AuraEffectPeriodicDamage : AuraEffectPeriodic
    {
        public new AuraEffectInfoPeriodicDamage EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectPeriodicDamage(Aura aura, AuraEffectInfoPeriodicDamage effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        protected override void HandlePeriodic(Unit target, Unit caster)
        {
            if (target.IsDead)
                return;

            if (target.HasState(UnitControlState.Isolated) || target.IsImmunedToDamage(Aura.AuraInfo))
                return;

            int originalDamage = EffectInfo.CalculateSpellDamage(caster);
            bool hasCrit = caster.Spells.IsSpellCrit(target, Aura.SpellInfo, EffectInfo.SpellSchoolMask);
            caster.Spells.DamageBySpell(new SpellDamageInfo(caster, target, Aura.SpellInfo, (uint)originalDamage, hasCrit, SpellDamageType.Dot));
        }
    }
}