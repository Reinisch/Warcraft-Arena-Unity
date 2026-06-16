using JetBrains.Annotations;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Effect Launch Projectile", menuName = "Game Data/Spells/Effects/Launch Projectile")]
    public class EffectLaunchProjectile : SpellEffectInfo
    {
        [Header("Launch Projectile")]
        [SerializeField, UsedImplicitly] private int baseAmount;
        [SerializeField, UsedImplicitly] private ProjectileLaunchInfo launchInfo;
        [SerializeField, UsedImplicitly] private List<ConditionalModifier> projectileAmountModifiers;

        public override float Value => baseAmount;
        public override SpellEffectType EffectType => SpellEffectType.Projectile;
        public IReadOnlyList<ConditionalModifier> ProjectileAmountModifiers => projectileAmountModifiers;

        internal override void Handle(Spell spell, int effectIndex, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.Launch || target == null || !target.IsAlive)
                return;

            if (!spell.ExplicitTargets.TargetingSource.HasValue || !spell.ExplicitTargets.TargetingRotation.HasValue)
                return;

            float baseProjectileAmount = Value;
            for (var i = 0; i < ProjectileAmountModifiers.Count; i++)
            {
                ConditionalModifier modifier = ProjectileAmountModifiers[i];
                if (modifier.Condition.IsApplicableAndValid(spell.Caster, target, spell))
                    modifier.Modify(spell.Caster, target, ref baseProjectileAmount);
            }

            int totalProjectiles = Mathf.FloorToInt(baseProjectileAmount);
            target.World.ProjectileLauncher.Add(new ProjectileLaunch(totalProjectiles, spell.Caster, spell.ExplicitTargets, launchInfo));
        }
    }
}
