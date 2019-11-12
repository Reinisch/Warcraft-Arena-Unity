using System;
using Common;
using Core.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class ConditionalModifier
    {
        [SerializeField, UsedImplicitly] private float value;
        [SerializeField, UsedImplicitly] private Condition condition;
        [SerializeField, UsedImplicitly] private ConditionalModiferValue conditionalModifierValue;
        [SerializeField, UsedImplicitly] private SpellModifierApplicationType applicationType;

        public Condition Condition => condition;

        public void Modify(Unit caster, Unit target, ref float baseValue)
        {
            float finalValue = value;
            conditionalModifierValue?.Modify(caster, target, ref finalValue);

            switch (applicationType)
            {
                case SpellModifierApplicationType.Flat:
                    baseValue += finalValue;
                    break;
                case SpellModifierApplicationType.Percent:
                    baseValue += baseValue.ApplyPercentage(finalValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(applicationType), applicationType, "Unknown modifier application type!");
            }
        }
    }
}
