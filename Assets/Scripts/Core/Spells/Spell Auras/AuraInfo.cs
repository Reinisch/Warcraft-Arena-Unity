using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Info", menuName = "Game Data/Spells/Auras/Aura Info", order = 1)]
    public sealed class AuraInfo : ScriptableObject
    {
        [Header("Aura Info")]
        [SerializeField, UsedImplicitly, HideInInspector] private int id;
        [SerializeField, UsedImplicitly] private int duration;
        [SerializeField, UsedImplicitly] private int maxDuration;
        [SerializeField, UsedImplicitly] private int maxCharges;
        [SerializeField, UsedImplicitly] private int baseCharges;

        [SerializeField, UsedImplicitly] private AuraStateType stateType;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraTargetingMode targetingMode;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraInterruptFlags interruptFlags;
        [SerializeField, UsedImplicitly, EnumFlag] private AuraAttributes attributes;
        [SerializeField, UsedImplicitly] private List<AuraEffectInfo> auraEffects;

        public int Id => id;
        public int Duration => duration;
        public int MaxDuration => maxDuration;
        public bool HasInterruptFlags => interruptFlags != 0;
        public AuraStateType StateType => stateType;
        public AuraTargetingMode TargetingMode => targetingMode;
        public AuraInterruptFlags InterruptFlags => interruptFlags;
        public IReadOnlyList<AuraEffectInfo> AuraEffects => auraEffects;

        [UsedImplicitly]
        private void OnValidate()
        {
            id = GetInstanceID();
        }

        public bool HasAttribute(AuraAttributes attribute)
        {
            return (attributes & attribute) != 0;
        }
    }
}
