using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Projectile Launch Info", menuName = "Game Data/Projectiles/Projectile Launch Info", order = 1)]
    public class ProjectileLaunchInfo: ScriptableObject
    {
        [field: SerializeField]
        public ProjectileInfo Projectile { get; private set; }

        [field: SerializeField]
        public ProjectileTagInfo LaunchTag { get; private set; }

        [field: SerializeField]
        public bool UseViewPointAsLaunchSource { get; private set; }
    }
}
