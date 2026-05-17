using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Flying", menuName = "Game Data/Conditions/Unit/Target Unit Is Flying", order = 3)]
    public sealed class TargetUnitIsFlying: Condition
    {
        protected override bool IsApplicable => base.IsApplicable && TargetUnit != null;

        protected override bool IsValid => base.IsValid && TargetUnit.HasMovementFlag(MovementFlags.MaskAir);
    }
}