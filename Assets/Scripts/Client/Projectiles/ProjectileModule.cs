using Common;
using Core;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Client
{
    public class ProjectileModule : ScriptableReferenceClient
    {
        [Inject] private BalanceReference balance;
        [Inject] private RenderingReference rendering;
        [Inject] private ProjectileLauncher projectileLauncher;
        [Inject] private ProjectileLaunchTracker.MemoryPool launchTrackerPool;

        [field: SerializeField]
        public LayerMask ProjectileTotalHitMask { get; private set; }

        [field: SerializeField]
        public LayerMask ProjectileOnlyHitMask { get; private set; }

        [field: SerializeField]
        public Transform ProjectileContainer { get; private set; }

        private readonly Dictionary<ProjectileLaunch, ProjectileLaunchTracker> activeProjectileEvents = new();
        private readonly Dictionary<Collider, Projectile> projectilesByCollider = new();

        protected override void OnRegistered()
        {
            base.OnRegistered();

            projectileLauncher.EventProjectileLaunchChanged += OnProjectileLaunchChanged;
        }

        protected override void OnUnregister()
        {
            projectileLauncher.EventProjectileLaunchChanged -= OnProjectileLaunchChanged;

            base.OnUnregister();
        }

        public Projectile CreateProjectile(ProjectileTracker projectileTracker)
        {
            if (!projectileTracker.ExplicitTargets.TargetingSource.HasValue ||
                !projectileTracker.ExplicitTargets.TargetingRotation.HasValue)
            {
                return null;
            }

            var projectile = GameObjectPool.Take(
                projectileTracker.ProjectileInfo.Projectile,
                projectileTracker.ExplicitTargets.TargetingSource.Value,
                projectileTracker.ExplicitTargets.TargetingRotation.Value,
                ProjectileContainer).GetComponent<Projectile>();

            projectile.Launch(
                projectileTracker,
                projectileTracker.ExplicitTargets.TargetingSource.Value,
                projectileTracker.ExplicitTargets.TargetingRotation.Value);
            return projectile;
        }

        public void ProjectileLaunched(Projectile projectile)
        {
            projectilesByCollider.Add(projectile.Trigger, projectile);
        }

        public void ProjectileStopped(Projectile projectile)
        {
            projectilesByCollider.Remove(projectile.Trigger);
        }

        public bool TryFindProjectile(Collider trigger, out Projectile projectile)
        {
            return projectilesByCollider.TryGetValue(trigger, out projectile);
        }

        protected override void OnUpdate(float deltaTime)
        {
            foreach(var pair in activeProjectileEvents)
                pair.Value.DoUpdate(deltaTime);
        }

        private void OnProjectileLaunchChanged(ProjectileLaunch launch, bool active)
        {
            if (active)
                activeProjectileEvents.Add(launch, launchTrackerPool.Take(launch));
            else if (activeProjectileEvents.Remove(launch, out ProjectileLaunchTracker removedTracker))
                removedTracker.Dispose();
        }
    }
}
