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

        protected override bool IsApplicable => base.IsApplicable && IsOtherApplicable(condition);

        protected override bool IsValid => base.IsValid && IsOtherValid(condition);
    }
}
