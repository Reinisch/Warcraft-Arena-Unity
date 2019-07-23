using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class EffectSchoolDamage : SpellEffectInfo
    {
        [Header("School Damage")]
        [SerializeField, UsedImplicitly] private int baseValue;
        [SerializeField, UsedImplicitly] private uint baseVariance;
        [SerializeField, UsedImplicitly] private uint additionalValue;
        [SerializeField, UsedImplicitly] private SpellDamageCalculationType calculationType;

        public override float Value => baseValue;
        public override SpellEffectType EffectType => SpellEffectType.SchoolDamage;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectSchoolDamage(this, effectIndex, target, mode);
        }

        internal int CalculateSpellDamage(SpellInfo spellInfo, int effectIndex, Unit caster = null, Unit target = null)
        {
            int rolledValue = RandomUtils.Next(baseValue, (int) (baseValue + baseVariance));
            int baseDamage = 0;

            switch (calculationType)
            {
                case SpellDamageCalculationType.Direct:
                    baseDamage = (int) (rolledValue + additionalValue);
                    break;
                case SpellDamageCalculationType.SpellPowerPercent:
                    if (caster == null)
                        break;

                    baseDamage = (int) (additionalValue + caster.SpellPower.ApplyPercentage(rolledValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationType), $"Unknown damage calculation type: {calculationType}");
            }

            if (caster != null)
                baseDamage = Mathf.CeilToInt(caster.Spells.ApplyEffectModifiers(spellInfo, baseDamage));

            return baseDamage;
        }
    }

    public partial class Spell
    {
        internal void EffectSchoolDamage(EffectSchoolDamage effect, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget || target == null || !target.IsAlive)
                return;

            int spellDamage = effect.CalculateSpellDamage(SpellInfo, effectIndex, Caster, target);
            if (SpellInfo.HasAttribute(SpellCustomAttributes.ShareDamage))
                spellDamage /= Mathf.Min(1, ImplicitTargets.TargetCountForEffect(effectIndex));

            if (OriginalCaster != null)
            {
                spellDamage += OriginalCaster.Spells.SpellDamageBonusDone(target, SpellInfo, spellDamage, SpellDamageType.Direct, effect);
                spellDamage = target.Spells.SpellDamageBonusTaken(OriginalCaster, SpellInfo, spellDamage, SpellDamageType.Direct, effect);
            }

            EffectDamage += spellDamage;
        }
    }
}
