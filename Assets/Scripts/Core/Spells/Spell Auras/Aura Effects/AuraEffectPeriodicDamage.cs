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
            if (target.IsDead)
                return;

            if (target.HasState(UnitControlState.Isolated) || target.IsImmunedToDamage(Aura.AuraInfo))
                return;

            int damage = EffectInfo.CalculateSpellDamage(caster);
            damage = caster.Spells.SpellDamageBonusDone(target, Aura.SpellInfo, damage, SpellDamageType.Dot);
            damage = target.Spells.SpellDamageBonusTaken(caster, Aura.SpellInfo, damage, SpellDamageType.Dot);

            bool isCrit = caster.Spells.IsSpellCrit(target, Aura.SpellInfo, EffectInfo.SpellSchoolMask);
            if (isCrit)
                damage = caster.Spells.CalculateSpellCriticalDamage(Aura.SpellInfo, damage);

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, caster, target, damage, isCrit);

            caster.DealDamage(target, damage);
        }
    }
}