using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Creature", menuName = "Game Data/Conditions/Unit/Target Is Creature", order = 3)]
    public sealed class TargetUnitIsCreature : Condition
    {
        protected override bool IsApplicable => base.IsApplicable && TargetUnit != null;

        protected override bool IsValid => base.IsValid && TargetUnit is Creature;
    }
}
