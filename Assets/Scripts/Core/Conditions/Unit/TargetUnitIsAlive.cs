using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Alive", menuName = "Game Data/Conditions/Unit/Target Is Alive", order = 2)]
    public sealed class TargetUnitIsAlive : Condition
    {
        public override bool IsApplicable => TargetUnit != null && base.IsApplicable;

        public override bool IsValid
        {
            get
            {
                bool isValid = TargetUnit.IsAlive;
                return base.IsValid && isValid;
            }
        }
    }
}
