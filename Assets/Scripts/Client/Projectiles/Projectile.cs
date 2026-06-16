using Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Client
{
    public class Projectile: MonoBehaviour
    {
        private static readonly ProjectileUtils.RaycastHitDistanceComparer ProjectileHitComparer = new();

        [SerializeField]
        private EffectSettings projectileEffect;

        [SerializeField]
        private SphereCollider trigger;

        [Inject]
        private ProjectileModule projectileModule;

        [Inject]
        private RenderingReference rendering;

        [Inject]
        private SoundReference sound;

        private readonly Dictionary<Unit, int> hitsPerUnitTotal = new();
        private readonly Dictionary<Unit, int> hitsPerUnitInScan = new();
        private readonly RaycastHit[] results = new RaycastHit[20];
        private ProjectileTracker projectileTracker;
        private EffectHandle projectileVisuals;

        private float lifeTimeLeft;
        private float delayTimeLeft;
        private bool launched;

        public ProjectileLaunchInfo ProjectileLaunchInfo => projectileTracker.LaunchInfo;
        public ProjectileInfo ProjectileInfo => projectileTracker.ProjectileInfo;
        public SpellInfo HitSpell => ProjectileInfo.OnHitInfo;
        public Unit Caster => projectileTracker.Caster;
        public Transform Transform => transform;
        public Collider Trigger => trigger;

        public Vector3 TargetingSource { get; private set; }
        public Quaternion TargetingRotation { get; private set; }

        private void OnDisable() => Stop();

        public void Launch(
            ProjectileTracker projectileTracker,
            Vector3 targetingSource,
            Quaternion targetingRotation)
        {
            this.projectileTracker = projectileTracker;

            TargetingSource = targetingSource;
            TargetingRotation = targetingRotation;
            launched = true;

            DeterminePositioning();

            Transform.SetPositionAndRotation(TargetingSource, TargetingRotation);
            lifeTimeLeft = ProjectileInfo.LifeTimeLimit;
            delayTimeLeft = ProjectileInfo.HitScanDelay;

            projectileVisuals = projectileEffect.PlayEffect(Transform.position, Transform.rotation, Transform);
            projectileModule.ProjectileLaunched(this);
        }

        public void DoUpdate(float deltaTime)
        {
            if (lifeTimeLeft > 0 && (lifeTimeLeft -= deltaTime) <= 0)
            {
                Stop();
                return;
            }

            if (delayTimeLeft > 0 && (delayTimeLeft -= deltaTime) > 0)
            {
                return;
            }

            Vector3 movementDelta = deltaTime * ProjectileInfo.Speed * transform.forward;

            DoHitScan(extraDistance: movementDelta.magnitude);

            if (isActiveAndEnabled)
                transform.position += movementDelta;
        }

        public void Stop()
        {
            projectileVisuals.Stop();

            if (launched)
            {
                launched = false;

                hitsPerUnitTotal.Clear();
                hitsPerUnitInScan.Clear();

                projectileModule.ProjectileStopped(this);
                projectileTracker.OnCompletion();
            }
        }

        private void DeterminePositioning()
        {
            if (ProjectileLaunchInfo.UseViewPointAsLaunchSource || ProjectileLaunchInfo.LaunchTag == null || Caster == null)
                return;

            if (!rendering.UnitRenderers.TryFind(Caster, out UnitRenderer casterRenderer))
                return;

            Vector3 delta = ProjectileInfo.LifeTimeLimit * ProjectileInfo.Speed * transform.forward;
            int hits = Physics.SphereCastNonAlloc(
                TargetingSource,
                trigger.radius,
                TargetingRotation * Vector3.forward,
                results,
                ProjectileInfo.HitScanDistance + delta.magnitude,
                projectileModule.ProjectileTotalHitMask,
                QueryTriggerInteraction.Collide);

            Array.Sort(results, 0, hits, ProjectileHitComparer);

            TargetingSource = casterRenderer.TagContainer.FindProjectileTag(ProjectileLaunchInfo.LaunchTag);

            for (int i = 0; i < hits; i++)
                if (HasHitTarget(results[i]))
                {
                    Vector3 direction = (results[i].point - TargetingSource).normalized;
                    if (Vector3.Dot(direction, Caster.transform.forward) > 0)
                        TargetingRotation = Quaternion.LookRotation(results[i].point - TargetingSource);

                    break;
                }    
        }

        private void DoHitScan(float extraDistance)
        {
            hitsPerUnitInScan.Clear();

            int hits = Physics.SphereCastNonAlloc(
                transform.position,
                trigger.radius,
                transform.forward,
                results,
                ProjectileInfo.HitScanDistance + extraDistance,
                projectileModule.ProjectileTotalHitMask,
                QueryTriggerInteraction.Collide);

            Array.Sort(results, 0, hits, ProjectileHitComparer);

            for (int i = 0; i < hits; i++)
                if (ConfirmHit(results[i]) && !isActiveAndEnabled)
                    break;
        }

        private bool HasHitTarget(RaycastHit hit)
        {
            if (rendering.TryFindRendererByProjectileHitBox(hit.collider, out UnitRenderer unitRenderer, out _))
            {
                if (unitRenderer.Unit == Caster)
                    return false;
            }

            if (projectileModule.TryFindProjectile(hit.collider, out Projectile otherProjectile))
            {
                if (otherProjectile.Caster == Caster)
                    return false;
            }

            return true;
        }

        private bool ConfirmHit(RaycastHit hit)
        {
            if (!isActiveAndEnabled)
                return false;

            if (rendering.TryFindRendererByProjectileHitBox(hit.collider, out UnitRenderer unitRenderer, out UnitProjectileHitBox hitBox))
            {
                if (unitRenderer.Unit == Caster)
                    return false;
            }

            if (projectileModule.TryFindProjectile(hit.collider, out Projectile otherProjectile))
            {
                if (otherProjectile.Caster == Caster)
                    return false;
            }

            if (unitRenderer != null && !Caster.IsHostileTo(unitRenderer.Unit))
                return false;

            if (unitRenderer != null && !HandleHitCount(unitRenderer.Unit))
                return false;

            float hitBoxMultiplider = 1;
            if (hitBox != null)
                hitBoxMultiplider *= hitBox.DamageMultiplier;

            if (unitRenderer != null)
            {
                 var result = Caster.Spells.CastSpell(ProjectileInfo.OnHitInfo,
                    new SpellCastingOptions(
                        new SpellExplicitTargets
                        { Target = unitRenderer.Unit },
                        SpellCastFlags.TriggeredByAura,
                        hitPosition: hit.point,
                        hitBoxMultiplider: hitBoxMultiplider),
                    out Spell spell);

                if (result == SpellCastResult.Success && hitBox != null)
                    hitBox.ReceiveDamage(spell.TotalDamage);
            }
            else if (ProjectileInfo.HitsGround)
            {
                rendering.OnProjectileImpact(this);
                sound.OnProjectileImpact(this);
            }

            if (ProjectileInfo.StopOnHit)
                Stop();

            return true;
        }

        private bool HandleHitCount(Unit unit)
        {
            if (hitsPerUnitTotal.TryGetValue(unit, out int confirmedHitsTotal))
                if (confirmedHitsTotal >= ProjectileInfo.MaxTotalEntityHits)
                    return false;

            if (hitsPerUnitInScan.TryGetValue(unit, out int confirmedHitsInScan))
                if (confirmedHitsInScan >= ProjectileInfo.MaxScanEntityHits)
                    return false;

            hitsPerUnitInScan[unit] = ++confirmedHitsInScan;
            hitsPerUnitTotal[unit] = ++confirmedHitsTotal;

            return true;
        }
    }
}