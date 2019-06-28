using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Target Unit Is Creature", menuName = "Game Data/Conditions/Unit/Target Is Creature", order = 3)]
    public sealed class TargetUnitIsCreature : Condition
    {
        protected internal override bool IsApplicable => TargetUnit != null && base.IsApplicable;

        protected internal override bool IsValid
        {
            get
            {
                bool isValid = TargetUnit is Creature;
                return base.IsValid && isValid;
            }
        }
    }
}
