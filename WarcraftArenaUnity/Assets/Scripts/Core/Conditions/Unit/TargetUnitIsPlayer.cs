using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Player", menuName = "Game Data/Conditions/Unit/Target Is Player", order = 4)]
    public sealed class TargetUnitIsPlayer : Condition
    {
        protected override bool IsApplicable => TargetUnit != null && base.IsApplicable;

        protected override bool IsValid => base.IsValid && TargetUnit is Player;
    }
}
