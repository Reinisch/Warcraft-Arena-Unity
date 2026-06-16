using UnityEngine;

namespace Core
{
    public class SpellExplicitTargets
    {
        public Unit Target { get; set; }
        public Vector3? Source { get; set; }
        public Vector3? Destination { get; set; }

        public Vector3? HitPosition { get; set; }
        public Vector3? TargetingSource { get; set; }
        public Quaternion? TargetingRotation { get; set; }

        public SpellExplicitTargets()
        {
        }

        public SpellExplicitTargets(Unit target)
        {
            Target = target;
        }

        public void Dispose()
        {
            Target = null;
        }
    }
}