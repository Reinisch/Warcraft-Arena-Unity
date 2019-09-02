using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Area Target - Spell Targeting", menuName = "Game Data/Spells/Spell Targeting/Area", order = 1)]
    public class SpellTargetingArea : SpellTargeting
    {
        [SerializeField, UsedImplicitly] private SpellTargetReferences referenceType = SpellTargetReferences.Caster;
        [SerializeField, UsedImplicitly] private SpellTargetChecks targetChecks = SpellTargetChecks.Enemy;
        [SerializeField, UsedImplicitly] private float minRadius;
        [SerializeField, UsedImplicitly] private float maxRadius = 10.0f;

        private Unit SelectReferer(SpellExplicitTargets explicitTargets, Unit caster)
        {
            switch (referenceType)
            {
                case SpellTargetReferences.Source:
                case SpellTargetReferences.Dest:
                case SpellTargetReferences.Caster:
                    return caster;
                case SpellTargetReferences.Target:
                    return explicitTargets.Target;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float CalculateRadius(Spell spell)
        {
            float radius = Mathf.Max(maxRadius, minRadius);
            if (spell.Caster != null)
            {
                spell.Caster.Spells.ApplySpellModifier(spell.SpellInfo, SpellModifierType.Radius, ref radius);
                radius = Mathf.Clamp(radius, minRadius, maxRadius);
            }

            return radius;
        }

        protected bool IsValidTargetForSpell(Unit target, Spell spell)
        {
            if (target.IsDead && !spell.SpellInfo.HasAttribute(SpellAttributes.CanTargetDead))
                return false;

            return true;
        }

        internal sealed override void SelectTargets(Spell spell)
        {
            Unit referer = SelectReferer(spell.ExplicitTargets, spell.Caster);
            float radius = CalculateRadius(spell);
            List<Unit> targets = new List<Unit>();

            spell.Caster.Map.SearchAreaTargets(targets, radius, referer.Position, spell.Caster, targetChecks);

            foreach (var target in targets)
                if(IsValidTargetForSpell(target, spell))
                    spell.ImplicitTargets.AddTargetIfNotExists(target);
        }
    }
}
