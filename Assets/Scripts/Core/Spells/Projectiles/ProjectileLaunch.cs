namespace Core
{
    public class ProjectileLaunch
    {
        public int ProjectileAmount { get; }
        public Unit Caster { get; }
        public SpellExplicitTargets ExplicitTargets { get; }

        public ProjectileLaunchInfo LaunchInfo { get; }

        public bool Completed { get; private set; }

        public ProjectileLaunch(
            int projectileAmount,
            Unit caster,
            SpellExplicitTargets explicitTargets,
            ProjectileLaunchInfo launchInfo)
        {
            ProjectileAmount = projectileAmount;
            Caster = caster;
            LaunchInfo = launchInfo;
            ExplicitTargets = explicitTargets;
        }

        internal void DoUpdate(int deltaTime)
        {
            
        }

        internal void HandleUnitDetach(Unit detachedUnit)
        {
            if (detachedUnit == Caster)
            {
                Completed = true;
            }
        }

        public void Complete()
        {
            Completed = true;
        }
    }
}