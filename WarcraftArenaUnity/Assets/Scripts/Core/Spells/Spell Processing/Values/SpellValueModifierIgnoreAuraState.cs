using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Value Modifier - Ignore Aura State", menuName = "Game Data/Spells/Value Modifier/Ignore Aura State", order = 1)]
    public class SpellValueModifierIgnoreAuraState : SpellValueModifier
    {
        [SerializeField, UsedImplicitly] private AuraStateType ignoredAuraState;

        internal override void Modify(ref SpellValue spellValue)
        {
            spellValue.IgnoredAuraStates |= ignoredAuraState.AsFlag();
        }
    }
}
