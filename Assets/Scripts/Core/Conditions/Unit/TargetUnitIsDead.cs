using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Dead", menuName = "Game Data/Conditions/Unit/Target Is Dead", order = 2)]
    public sealed class TargetUnitIsDead : Condition
    {
        protected override bool IsApplicable => base.IsApplicable && TargetUnit != null;

        protected override bool IsValid => base.IsValid && TargetUnit.IsDead;
    }
}