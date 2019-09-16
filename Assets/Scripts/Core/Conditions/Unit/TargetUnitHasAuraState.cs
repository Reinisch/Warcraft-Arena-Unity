using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Has Aura State", menuName = "Game Data/Conditions/Unit/Target Has Aura State", order = 3)]
    public sealed class TargetUnitHasAuraState : Condition
    {
        [SerializeField, UsedImplicitly] private AuraStateType auraStateType;

        public override bool IsApplicable => TargetUnit != null && base.IsApplicable;

        public override bool IsValid
        {
            get
            {
                bool isValid = TargetUnit.HasAuraState(auraStateType);
                return base.IsValid && isValid;
            }
        }
    }
}