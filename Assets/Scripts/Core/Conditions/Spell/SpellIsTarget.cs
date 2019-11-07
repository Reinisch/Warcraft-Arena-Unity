using JetBrains.Annotations;
using UnityEngine;

namespace Core.Conditions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Is Target", menuName = "Game Data/Conditions/Spell/Spell Is Target", order = 2)]
    public sealed class SpellIsTarget : Condition
    {
        [SerializeField, UsedImplicitly] private SpellInfo targetSpell;

        protected override bool IsApplicable => base.IsApplicable && SpellInfo != null;

        protected override bool IsValid => base.IsValid && SpellInfo == targetSpell;
    }
}