using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Inverse Condition", menuName = "Game Data/Conditions/Base/Inverse Condition", order = 1)]
    public sealed class InverseCondition : Condition
    {
        [SerializeField, UsedImplicitly] private Condition condition;

        protected internal override bool IsValid
        {
            get
            {
                Condition targetCondition = condition.From(this);
                bool isApplicable = targetCondition.IsApplicable;
                bool isValid = !targetCondition.IsValid && isApplicable;
                return base.IsValid && isValid;
            }
        }
    }
}
