using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info", menuName = "Game Data/Spells/Auras/Aura Info", order = 1)]
    public sealed class AuraInfo : ScriptableUniqueInfo<AuraInfo>
    {
        [SerializeField, UsedImplicitly] private AuraInfoContainer container;

        [Header("Aura Info")]
        [SerializeField, UsedImplicitly] private int duration;
        [SerializeField, UsedImplicitly] private int maxDuration;
        [SerializeField, UsedImplicitly] private int maxStack;
        [SerializeField, UsedImplicitly] private AuraStateType stateType;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraTargetingMode targetingMode;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraInterruptFlags interruptFlags;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraAttributes attributes;
        [SerializeField, UsedImplicitly] private List<AuraEffectInfo> auraEffects;
        [SerializeField, UsedImplicitly] private List<AuraScriptable> auraScriptables;

        [Header("Charges")]
        [SerializeField, UsedImplicitly] private bool usesCharges;
        [SerializeField, UsedImplicitly] private int maxCharges;
        [SerializeField, UsedImplicitly] private int baseCharges;

        [Header("Damage Interrupt Info")]
        [SerializeField, UsedImplicitly] private int damageInterruptValue;
        [SerializeField, UsedImplicitly] private int damageInterruptDelay;
        [SerializeField, UsedImplicitly] private AuraInterruptValueCalculationType interruptValueType;

        protected override ScriptableUniqueInfoContainer<AuraInfo> Container => container;
        protected override AuraInfo Data => this;

        public new int Id => base.Id;
        public int Charges => baseCharges;
        public int MaxCharges => maxCharges;
        public int Duration => duration;
        public int MaxDuration => maxDuration;
        public bool UsesCharges => usesCharges;
        public bool HasInterruptFlags => interruptFlags != 0;
        public AuraStateType StateType => stateType;
        public AuraTargetingMode TargetingMode => targetingMode;
        public AuraInterruptFlags InterruptFlags => interruptFlags;
        public IReadOnlyList<AuraEffectInfo> AuraEffects => auraEffects;

        internal IReadOnlyList<AuraScriptable> AuraScriptables => auraScriptables;

        public bool IsPositive => !HasAttribute(AuraAttributes.Negative);

        public bool HasAttribute(AuraAttributes attribute)
        {
            return (attributes & attribute) != 0;
        }

        public bool HasMechanics(SpellMechanics mechanics)
        {
            foreach (AuraEffectInfo auraEffectInfo in auraEffects)
                if (auraEffectInfo.Mechanics == mechanics)
                    return true;

            return false;
        }

        public bool HasAnyMechanics(SpellMechanicsFlags mechanicsFlags)
        {
            foreach (AuraEffectInfo auraEffectInfo in auraEffects)
                if (auraEffectInfo.Mechanics != SpellMechanics.None && mechanicsFlags.HasTargetFlag(auraEffectInfo.Mechanics.AsFlag()))
                    return true;

            return false;
        }

        public void CalculateDamageInterruptValue(Unit caster, Unit target, out int delay, out int interruptValue)
        {
            if (!interruptFlags.HasTargetFlag(AuraInterruptFlags.CombinedDamageTaken))
            {
                delay = 0;
                interruptValue = 0;
                return;
            }

            switch (interruptValueType)
            {
                case AuraInterruptValueCalculationType.Direct:
                    delay = damageInterruptDelay;
                    interruptValue = damageInterruptValue;
                    break;
                case AuraInterruptValueCalculationType.MaxHealthCasterPercent:
                    if (caster != null)
                    {
                        delay = damageInterruptDelay;
                        interruptValue = caster.MaxHealth.CalculatePercentage(damageInterruptValue);
                    }
                    else
                    {
                        delay = 0;
                        interruptValue = 0;
                    }
                    break;
                case AuraInterruptValueCalculationType.MaxHealthTargetPercent:
                    if (target != null)
                    {
                        delay = damageInterruptDelay;
                        interruptValue = target.MaxHealth.CalculatePercentage(damageInterruptValue);
                    }
                    else
                    {
                        delay = 0;
                        interruptValue = 0;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsStackableOnOneSlotWithDifferentCasters()
        {
            return maxStack > 1 && !HasAttribute(AuraAttributes.StackForAnyCasters);
        }
    }
}
