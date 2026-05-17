using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Destination Target - Spell Targeting", menuName = "Game Data/Spells/Spell Targeting/Destination", order = 3)]
    public class SpellTargetingDestination: SpellTargeting
    {
        internal override void SelectTargets(Spell spell, int effectMask)
        {
            if (spell.ExplicitTargets.Destination != null)
            {
                spell.ImplicitTargets.AddDestination(spell.ExplicitTargets.Destination.Value, effectMask);
            }
        }
    }
}