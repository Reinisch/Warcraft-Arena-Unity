using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Single Target - Spell Targeting", menuName = "Game Data/Spells/Spell Targeting/Single", order = 1)]
    public class SpellTargetingSingle : SpellTargeting
    {
        [SerializeField, UsedImplicitly] private SpellExplicitTargetType explicitTargetType = SpellExplicitTargetType.Target;

        internal override void SelectTargets(Spell spell)
        {
            // select implicit target from explicit effect target type
            switch (explicitTargetType)
            {
                case SpellExplicitTargetType.Target when spell.ExplicitTargets.Target != null:
                    spell.ImplicitTargets.AddTargetIfNotExists(spell.ExplicitTargets.Target);
                    break;
                case SpellExplicitTargetType.Caster when spell.Caster != null:
                    spell.ImplicitTargets.AddTargetIfNotExists(spell.Caster);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
