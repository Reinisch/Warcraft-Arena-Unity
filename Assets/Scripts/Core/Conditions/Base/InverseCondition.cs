using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Inverse Condition", menuName = "Game Data/Conditions/Inverse Condition", order = 1)]
    public sealed class InverseCondition : Condition
    {
        [SerializeField, UsedImplicitly] private Condition condition;

        public override bool IsValid
        {
            get
            {
                Condition targetCondition = condition.From(this);
                bool isValid = targetCondition.IsApplicable && !targetCondition.IsValid;
                return base.IsValid && isValid;
            }
        }
    }
}
