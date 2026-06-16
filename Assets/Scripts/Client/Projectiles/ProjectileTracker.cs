using Common;
using Core;
using Zenject;

namespace Client
{
    public class ProjectileTracker: GenericPoolItem<ProjectileLaunchTracker, ProjectileInfo>
    {
        public class Factory : GenericFactory<ProjectileTracker>{ }
        public class MemoryPool : GenericMemoryPool<ProjectileTracker, Factory, ProjectileLaunchTracker, ProjectileInfo> { }

        [Inject]
        private ProjectileModule projectileModule;

        public bool Activated { get; private set; }
        public bool Completed { get; private set; }

        public Projectile Projectile { get; private set;  }
        public ProjectileInfo ProjectileInfo { get; private set; }
        public ProjectileLaunch Launch { get; private set; }
        public ProjectileLaunchTracker LaunchTracker { get; private set; }

        public ProjectileLaunchInfo LaunchInfo => Launch.LaunchInfo;
        public SpellExplicitTargets ExplicitTargets => Launch.ExplicitTargets;
        public Unit Caster => Launch.Caster;

        public void DoUpdate(float deltaTime)
        {
            if (Completed)
                return;

            if (!Activated)
            {
                Projectile = projectileModule.CreateProjectile(this);
                Activated = true;
            }

            if (Projectile != null)
            {
                Projectile.DoUpdate(deltaTime);
            }
            else
            {
                Completed = true;
            }
        }

        public void OnCompletion()
        {
            if (Projectile != null)
            {
                GameObjectPool.Return(Projectile.gameObject, false);

                Projectile = null;
            }
            
            Completed = true;
        }

        protected override void OnReturnedToPool()
        {
            if (Projectile != null)
            {
                Projectile.Stop();
                Projectile = null;
            }

            Activated = Completed = false;
            ProjectileInfo = null;
            Launch = null;
            LaunchTracker = null;
        }

        protected override void OnTakenFromPool(
            ProjectileLaunchTracker launchTracker, 
            ProjectileInfo projectileInfo)
        {
            ProjectileInfo = projectileInfo;
            LaunchTracker = launchTracker;
            Launch = launchTracker.Launch;
        }
    }
}