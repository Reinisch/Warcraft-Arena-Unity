using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Projectile Info", menuName = "Game Data/Projectiles/Projectile Info", order = 1)]
    public class ProjectileInfo : ScriptableObject
    {
        [field: SerializeField]
        public SpellInfo OnHitInfo { get; private set; }

        [field: SerializeField]
        public GameObject Projectile { get; private set; }

        [field: SerializeField]
        public bool StopOnHit { get; private set; } = true;

        [field: SerializeField]
        public bool HitsGround { get; private set; } = true;

        [field: SerializeField]
        public float MaxTotalEntityHits { get; private set; } = 1;

        [field: SerializeField]
        public float MaxScanEntityHits { get; private set; } = 1;

        [field: SerializeField]
        public float HitScanDelay { get; private set; }

        [field: SerializeField]
        public float LifeTimeLimit { get; private set; } = 10;

        [field: SerializeField]
        public float Speed { get; private set; } = 100;

        [field: SerializeField]
        public float HitScanDistance { get; private set; } = 0.1f;
    }
}