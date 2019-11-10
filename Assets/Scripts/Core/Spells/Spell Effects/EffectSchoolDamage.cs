using System;
using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect School Damage", menuName = "Game Data/Spells/Effects/School Damage", order = 4)]
    public class EffectSchoolDamage : SpellEffectInfo
    {
        [Header("School Damage")]
        [SerializeField, UsedImplicitly] private int baseValue;
        [SerializeField, UsedImplicitly] private uint baseVariance;
        [SerializeField, UsedImplicitly] private uint additionalValue;
        [SerializeField, UsedImplicitly] private bool usesComboPoints;
        [SerializeField, UsedImplicitly] private SpellDamageCalculationType calculationType;
        [SerializeField, UsedImplicitly] private List<ConditionalModifier> conditionalModifiers;

        public override float Value => baseValue;
        public override SpellEffectType EffectType => SpellEffectType.SchoolDamage;
        public IReadOnlyList<ConditionalModifier> ConditionalModifiers => conditionalModifiers;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            spell.EffectSchoolDamage(this, effectIndex, target, mode);
        }

        internal int CalculateSpellDamage(SpellInfo spellInfo, int effectIndex, Unit caster = null, Spell spell = null)
        {
            int rolledValue = RandomUtils.Next(baseValue, (int) (baseValue + baseVariance));

            if (usesComboPoints && spell != null && spell.ConsumedComboPoints > 0)
                rolledValue *= spell.ConsumedComboPoints;

            float baseDamage = 0;

            switch (calculationType)
            {
                case SpellDamageCalculationType.Direct:
                    baseDamage = (rolledValue + additionalValue);
                    break;
                case SpellDamageCalculationType.SpellPowerPercent:
                    if (caster == null)
                        break;

                    baseDamage = (additionalValue + caster.SpellPower.ApplyPercentage(rolledValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationType), $"Unknown damage calculation type: {calculationType}");
            }

            if (caster != null)
                baseDamage = caster.Spells.ApplyEffectModifiers(spellInfo, baseDamage);

            return (int) baseDamage;
        }
    }

    public partial class Spell
    {
        internal void EffectSchoolDamage(EffectSchoolDamage effect, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitStart || target == null || !target.IsAlive)
                return;

            float spellDamage = effect.CalculateSpellDamage(SpellInfo, effectIndex, Caster, this);
            if (SpellInfo.HasAttribute(SpellCustomAttributes.ShareDamage))
                spellDamage /= Mathf.Min(1, ImplicitTargets.TargetCountForEffect(effectIndex));

            for (var i = 0; i < effect.ConditionalModifiers.Count; i++)
            {
                ConditionalModifier modifier = effect.ConditionalModifiers[i];
                if (modifier.Condition.IsApplicableAndValid(Caster, target, this))
                    modifier.Modify(Caster, target, ref spellDamage);
            }

            EffectDamage += (int) spellDamage;
        }
    }
}
