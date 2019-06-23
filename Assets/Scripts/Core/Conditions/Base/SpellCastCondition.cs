using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Cast Condition", menuName = "Game Data/Conditions/Base/Spell Cast Condition", order = 1)]
    public sealed class SpellCastCondition : Condition
    {
        [SerializeField, UsedImplicitly] private SpellCastResult failedResult;
        [SerializeField, UsedImplicitly] private Condition condition;

        public SpellCastResult FailedResult => failedResult;

        protected internal override bool IsValid
        {
            get
            {
                Condition targetCondition = condition.From(this);
                bool isApplicable = targetCondition.IsApplicable;
                bool isValid = targetCondition.IsValid && isApplicable;
                return base.IsValid && isValid;
            }
        }

        protected override Condition SetResources(Unit source = null, Unit target = null, Spell spell = null)
        {
            base.SetResources(source, target, spell);

            condition.From(this);

            return this;
        }

        protected override bool FreeResources(Condition baseCondition)
        {
            base.FreeResources(baseCondition);

            base.FreeResources(condition);

            return true;
        }
    }
}
