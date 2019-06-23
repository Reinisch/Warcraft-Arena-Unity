using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Alive", menuName = "Game Data/Conditions/Unit/Target Is Alive", order = 2)]
    public sealed class TargetUnitIsAlive : Condition
    {
        protected internal override bool IsApplicable => TargetUnit != null && base.IsApplicable;

        protected internal override bool IsValid
        {
            get
            {
                bool isValid = TargetUnit.IsAlive;
                return base.IsValid && isValid;
            }
        }
    }
}
