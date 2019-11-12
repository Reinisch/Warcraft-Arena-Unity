using System;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TargetingOptions
    {
        [SerializeField, UsedImplicitly, EnumFlag] private TargetingDeathState deathState;
        [SerializeField, UsedImplicitly, EnumFlag] private TargetingEntityType entityTypes;
        [SerializeField, UsedImplicitly] private TargetingDistance distance;
        [SerializeField, UsedImplicitly] private TargetingMode mode;
        [SerializeField, UsedImplicitly] private float maxReferringAngle = 180.0f;

        public TargetingDeathState DeathState => deathState;
        public TargetingEntityType EntityTypes => entityTypes;
        public TargetingDistance Distance => distance;
        public TargetingMode Mode => mode;
        public float MaxReferringAngle => maxReferringAngle;
    }
}
