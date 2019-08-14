using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info", menuName = "Game Data/Spells/Auras/Aura Info", order = 1)]
    public sealed class AuraInfo : ScriptableUniqueInfo<AuraInfo>
    {
        [Header("Aura Info")]
        [SerializeField, UsedImplicitly] private int duration;
        [SerializeField, UsedImplicitly] private int maxDuration;
        [SerializeField, UsedImplicitly] private int maxStack;
        [SerializeField, UsedImplicitly] private int maxCharges;
        [SerializeField, UsedImplicitly] private int baseCharges;

        [SerializeField, UsedImplicitly] private AuraStateType stateType;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraTargetingMode targetingMode;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraInterruptFlags interruptFlags;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraAttributes attributes;
        [SerializeField, UsedImplicitly] private List<AuraEffectInfo> auraEffects;
        [SerializeField, UsedImplicitly] private List<AuraScriptable> auraScriptables;

        public new int Id => base.Id;
        public int Duration => duration;
        public int MaxDuration => maxDuration;
        public bool HasInterruptFlags => interruptFlags != 0;
        public AuraStateType StateType => stateType;
        public AuraTargetingMode TargetingMode => targetingMode;
        public AuraInterruptFlags InterruptFlags => interruptFlags;
        public IReadOnlyList<AuraEffectInfo> AuraEffects => auraEffects;

        internal IReadOnlyList<AuraScriptable> AuraScriptables => auraScriptables;

        public bool HasAttribute(AuraAttributes attribute)
        {
            return (attributes & attribute) != 0;
        }

        public bool IsStackableOnOneSlotWithDifferentCasters()
        {
            return maxStack > 1 && !HasAttribute(AuraAttributes.StackForAnyCasters);
        }
    }
}
