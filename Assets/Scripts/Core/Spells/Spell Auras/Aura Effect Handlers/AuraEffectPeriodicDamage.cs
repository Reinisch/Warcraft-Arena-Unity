using Common;

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
            if (target.IsDead || caster == null)
                return;

            if (target.HasState(UnitControlState.Isolated) || target.IsImmunedToDamage(Aura.SpellInfo, EffectInfo.SpellSchoolMask, Aura.Caster))
            {
                SpellDamageInfo damageInfo = new SpellDamageInfo(caster, target, Aura.SpellInfo, SpellDamageType.Dot, HitType.Immune);
                EventHandler.ExecuteEvent(GameEvents.ServerDamageDone, damageInfo);
            }
            else
            {
                int originalDamage = EffectInfo.CalculateSpellDamage(caster);
                bool hasCrit = caster.Spells.IsSpellCrit(target, Aura.SpellInfo, EffectInfo.SpellSchoolMask);
                caster.Spells.DamageBySpell(new SpellDamageInfo(caster, target, Aura.SpellInfo, (uint)originalDamage, hasCrit, SpellDamageType.Dot));
            }
        }
    }
}