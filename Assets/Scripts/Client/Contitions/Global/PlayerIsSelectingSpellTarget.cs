using Client;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Player Is Selecting Spell Target", menuName = "Game Data/Conditions/Player/Player Is Selecting Spell Target", order = 1)]
    public sealed class PlayerIsSelectingSpellTarget : Condition
    {
        [SerializeField, UsedImplicitly] private TargetingSpellReference spellTargeting;

        public override bool IsValid
        {
            get
            {
                bool isValid = IsApplicable && spellTargeting.IsTargeting;
                return base.IsValid && isValid;
            }
        }
    }
}
