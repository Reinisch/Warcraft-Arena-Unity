using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Single Target - Spell Targeting", menuName = "Game Data/Spells/Spell Targeting/Single", order = 1)]
    public class SpellTargetingSingle : SpellTargeting
    {
        [SerializeField, UsedImplicitly] private SpellSingleTargetType singleTargetType = SpellSingleTargetType.Target;

        internal override void SelectTargets(Spell spell, int effectMask)
        {
            // select implicit target from explicit effect target type
            switch (singleTargetType)
            {
                case SpellSingleTargetType.Target:
                    if (spell.ExplicitTargets.Target != null && spell.Caster.IsOnSameMap(spell.ExplicitTargets.Target))
                        spell.ImplicitTargets.AddTargetIfNotExists(spell.ExplicitTargets.Target, effectMask);
                    break;
                case SpellSingleTargetType.Caster:
                    spell.ImplicitTargets.AddTargetIfNotExists(spell.Caster, effectMask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
