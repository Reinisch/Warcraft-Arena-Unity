using System;
using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Periodic Healing", menuName = "Game Data/Spells/Auras/Effects/Periodic Healing", order = 2)]
    public class AuraEffectInfoPeriodicHealing : AuraEffectInfoPeriodic
    {
        [SerializeField, UsedImplicitly] private int baseValue;
        [SerializeField, UsedImplicitly] private uint additionalValue;
        [SerializeField, UsedImplicitly] private SpellDamageCalculationType calculationType;
        [SerializeField, UsedImplicitly] private List<ConditionalModifier> conditionalModifiers;

        public override float Value => baseValue;
        public override AuraEffectType AuraEffectType => AuraEffectType.PeriodicHealing;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectPeriodicHealing(aura, this, index, Value);
        }

        internal int CalculateSpellHeal(Unit caster)
        {
            float baseHeal = 0;

            switch (calculationType)
            {
                case SpellDamageCalculationType.Direct:
                    baseHeal = (baseValue + additionalValue);
                    break;
                case SpellDamageCalculationType.SpellPowerPercent:
                    if (caster == null)
                        break;

                    baseHeal = additionalValue + caster.SpellPower.ApplyPercentage(baseValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationType), $"Unknown healing calculation type: {calculationType}");
            }

            return (int)baseHeal;
        }
    }
}