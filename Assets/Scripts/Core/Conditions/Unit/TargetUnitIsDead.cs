using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Dead", menuName = "Game Data/Conditions/Unit/Target Is Dead", order = 2)]
    public sealed class TargetUnitIsDead : Condition
    {
        public override bool IsApplicable => TargetUnit != null && base.IsApplicable;

        public override bool IsValid
        {
            get
            {
                bool isValid = TargetUnit.IsDead;
                return base.IsValid && isValid;
            }
        }
    }
}