using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Absorb Damage", menuName = "Game Data/Spells/Auras/Effects/Absorb Damage", order = 5)]
    public class AuraEffectInfoAbsorbDamage : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly] private int baseValue;
        [SerializeField, UsedImplicitly] private uint additionalValue;
        [SerializeField, UsedImplicitly] private SpellAbsorbCalculationType calculationType;
        [SerializeField, UsedImplicitly, EnumFlag] private SpellSchoolMask schoolMask;

        public override float Value => baseValue;
        public override AuraEffectType AuraEffectType => AuraEffectType.AbsorbDamage;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectAbsorbDamage(aura, this, index, Value);
        }

        internal int CalculateAbsorbAmount(Unit caster)
        {
            float baseAbsorb = 0;

            switch (calculationType)
            {
                case SpellAbsorbCalculationType.Direct:
                    baseAbsorb = baseValue + additionalValue;
                    break;
                case SpellAbsorbCalculationType.MaxHealthPercent:
                    baseAbsorb = caster.MaxHealth.ApplyPercentage(baseValue) + additionalValue;
                    break;
                case SpellAbsorbCalculationType.SpellPowerPercent:
                    if (caster == null)
                        break;

                    baseAbsorb = additionalValue + caster.SpellPower.ApplyPercentage(baseValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationType), $"Unknown damage calculation type: {calculationType}");
            }

            return (int)baseAbsorb;
        }
    }
}