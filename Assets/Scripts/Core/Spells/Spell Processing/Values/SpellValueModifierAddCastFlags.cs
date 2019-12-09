using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Value Modifier - Add Cast Flags", menuName = "Game Data/Spells/Value Modifier/Add Cast Flags", order = 1)]
    public class SpellValueModifierAddCastFlags : SpellValueModifier
    {
        [SerializeField, UsedImplicitly] private SpellCastFlags spellCastFlags;

        internal override void Modify(ref SpellValue spellValue)
        {
            spellValue.CastFlags |= spellCastFlags;
        }
    }
}