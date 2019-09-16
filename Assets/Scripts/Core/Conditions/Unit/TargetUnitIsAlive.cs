using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Alive", menuName = "Game Data/Conditions/Unit/Target Is Alive", order = 2)]
    public sealed class TargetUnitIsAlive : Condition
    {
        protected override bool IsApplicable => base.IsApplicable && TargetUnit != null;

        protected override bool IsValid => base.IsValid && TargetUnit.IsAlive;
    }
}
