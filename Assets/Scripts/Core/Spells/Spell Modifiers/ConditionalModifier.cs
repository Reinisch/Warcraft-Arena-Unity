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
        [SerializeField, UsedImplicitly] private SpellModifierApplicationType applicationType;

        public Condition Condition => condition;

        public void Modify(ref float baseValue)
        {
            switch (applicationType)
            {
                case SpellModifierApplicationType.Flat:
                    baseValue += value;
                    break;
                case SpellModifierApplicationType.Percent:
                    baseValue = baseValue.ApplyPercentage(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(applicationType), applicationType, "Unknown modifier application type!");
            }
        }
    }
}
