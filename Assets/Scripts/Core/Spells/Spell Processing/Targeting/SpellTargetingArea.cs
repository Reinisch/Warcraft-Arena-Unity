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

        public float MaxRadius => maxRadius;

        private Vector3 SelectSource(SpellExplicitTargets explicitTargets, Unit caster)
        {
            switch (referenceType)
            {
                case SpellTargetReferences.Destination:
                    return explicitTargets.Destination ?? caster.Position;
                case SpellTargetReferences.Source:
                case SpellTargetReferences.Caster:
                    return caster.Position;
                case SpellTargetReferences.Target:
                    return explicitTargets.Target.Position;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float CalculateRadius(Spell spell)
        {
            float radius = Mathf.Max(maxRadius, minRadius);
            if (spell.Caster != null)
            {
                radius = spell.Caster.Spells.ApplySpellModifier(spell, SpellModifierType.Radius, radius);
                radius = Mathf.Clamp(radius, minRadius, maxRadius);
            }

            return radius;
        }

        protected virtual bool IsValidTargetForSpell(Unit target, Spell spell)
        {
            if (target.IsDead && !spell.SpellInfo.HasAttribute(SpellAttributes.CanTargetDead))
                return false;

            return true;
        }

        internal sealed override void SelectTargets(Spell spell, int effectMask)
        {
            Vector3 center = SelectSource(spell.ExplicitTargets, spell.Caster);
            float radius = CalculateRadius(spell);
            List<Unit> targets = new List<Unit>();

            spell.Caster.Map.SearchAreaTargets(targets, radius, center, spell.Caster, targetChecks);

            foreach (var target in targets)
                if(IsValidTargetForSpell(target, spell))
                    spell.ImplicitTargets.AddTargetIfNotExists(target, effectMask);
        }
    }
}
