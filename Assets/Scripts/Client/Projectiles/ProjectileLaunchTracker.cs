using Common;
using Core;
using System.Collections.Generic;
using Zenject;

namespace Client
{
    public class ProjectileLaunchTracker: GenericPoolItem<ProjectileLaunch>
    {
        public class Factory : GenericFactory<ProjectileLaunchTracker> { }
        public class MemoryPool : GenericMemoryPool<ProjectileLaunchTracker, Factory, ProjectileLaunch> { }

        [Inject]
        private ProjectileTracker.MemoryPool projectileTrackerPool;

        private int remainingProjectiles;

        public ProjectileLaunch Launch { get; private set; }
        public List<ProjectileTracker> Projectiles { get; } = new();

        public void DoUpdate(float deltaTime)
        {
            if (Launch.Completed)
                return;

            while(remainingProjectiles > 0)
            {
                Projectiles.Add(projectileTrackerPool.Take(this, Launch.LaunchInfo.Projectile));
                remainingProjectiles--;
            }

            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = Projectiles[i];
                projectile.DoUpdate(deltaTime);

                if (projectile.Completed)
                {
                    Projectiles.RemoveAt(i);
                    projectile.Dispose();
                }
            }

            if (remainingProjectiles <= 0 && Projectiles.Count == 0)
                Launch.Complete();
        }

        protected override void OnReturnedToPool()
        {
            Projectiles.ForEach(item => item.Dispose());
            Projectiles.Clear();
        }

        protected override void OnTakenFromPool(ProjectileLaunch launch)
        {
            remainingProjectiles = launch.ProjectileAmount;
            Launch = launch;
        }
    }
}