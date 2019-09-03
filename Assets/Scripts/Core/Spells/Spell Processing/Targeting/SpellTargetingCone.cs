using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Cone Target - Spell Targeting", menuName = "Game Data/Spells/Spell Targeting/Cone", order = 1)]
    public class SpellTargetingCone : SpellTargetingArea
    {
        [SerializeField, UsedImplicitly] private SpellTargetDirections targetDirection = SpellTargetDirections.Front;
        [SerializeField, UsedImplicitly] private float coneAngle;

        protected override bool IsValidTargetForSpell(Unit target, Spell spell)
        {
            if (!base.IsValidTargetForSpell(target, spell))
                return false;

            return spell.Caster.IsFacing(target, targetDirection, coneAngle / 2);
        }
    }
}
