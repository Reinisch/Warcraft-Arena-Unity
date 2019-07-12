using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info", menuName = "Game Data/Spells/Auras/Aura Info", order = 1)]
    public sealed class AuraInfo : ScriptableObject
    {
        [Header("Aura Info")]
        [SerializeField, UsedImplicitly] private int id;
        [SerializeField, UsedImplicitly] private int maxCharges;
        [SerializeField, UsedImplicitly] private int baseCharges;

        [SerializeField, UsedImplicitly] private AuraTargetingMode targetingMode;
        [SerializeField, UsedImplicitly] private AuraStateType stateType;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraInterruptFlags interruptFlags;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraAttributes attributes;

        [SerializeField, UsedImplicitly] private List<AuraEffectInfo> auraEffects;

        public int Id => id;
        public bool HasInterruptFlags => interruptFlags != 0;
        public AuraStateType StateType => stateType;
        public AuraTargetingMode TargetingMode => targetingMode;
        public AuraInterruptFlags InterruptFlags => interruptFlags;
        public IReadOnlyList<AuraEffectInfo> AuraEffects => auraEffects;

        public bool HasAttribute(AuraAttributes attribute)
        {
            return (attributes & attribute) != 0;
        }
    }
}
