using System;
using System.Collections.Generic;
using Zenject;

namespace Core
{
    public class ProjectileLauncher
    {
        [Inject]
        private UnitManager unitManager;

        private readonly List<ProjectileLaunch> launches = new();

        public IReadOnlyList<ProjectileLaunch> Launches => launches;

        public event Action<ProjectileLaunch, bool> EventProjectileLaunchChanged;

        [Inject]
        private void Setup()
        {
            unitManager.EventEntityDetach += OnEntityDetach;
        }

        internal void Dispose()
        {
            unitManager.EventEntityDetach -= OnEntityDetach;

            for (int i = launches.Count - 1; i >= 0; i--)
                Remove(launches[i]);
        }

        internal void DoUpdate(int deltaTime)
        {
            foreach (ProjectileLaunch launch in launches)
                launch.DoUpdate(deltaTime);

            for (int i = launches.Count - 1; i >= 0; i--)
                if (launches[i].Completed)
                    Remove(launches[i]);
        }

        internal void Add(ProjectileLaunch launch)
        {
            launches.Add(launch);

            EventProjectileLaunchChanged?.Invoke(launch, true);
        }

        internal void Remove(ProjectileLaunch launch)
        {
            launches.Remove(launch);

            EventProjectileLaunchChanged?.Invoke(launch, false);
        }

        private void OnEntityDetach(Unit unit)
        {
            for (int i = launches.Count - 1; i >= 0; i--)
                launches[i].HandleUnitDetach(unit);
        }
    }
}
