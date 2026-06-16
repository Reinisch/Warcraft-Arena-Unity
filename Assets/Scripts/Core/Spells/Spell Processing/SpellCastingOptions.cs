using UnityEngine;

namespace Core
{
    public struct SpellCastingOptions
    {
        public SpellExplicitTargets Targets { get; internal set; }
        public SpellCastFlags SpellFlags  { get; internal set; }
        public MovementFlags? MovementFlags  { get; internal set; }

        public Vector3? HitPosition { get; internal set; }
        public Vector3? TargetingSource { get; internal set; }
        public Quaternion? TargetingRotation { get; internal set; }
        public float HitBoxMultiplier { get; internal set; }

        public SpellCastingOptions(
            SpellExplicitTargets targets = null, 
            SpellCastFlags castFlags = 0,
            MovementFlags? movementFlags = null,
            Vector3? hitPosition = null,
            Vector3? targetingSource = null,
            Quaternion? targetingRotation = null,
            float hitBoxMultiplider = 1)
        {
            Targets = targets;
            SpellFlags = castFlags;
            MovementFlags = movementFlags;
            HitPosition = hitPosition;
            TargetingSource = targetingSource;
            TargetingRotation = targetingRotation;
            HitBoxMultiplier = hitBoxMultiplider;
        }
    }
}
