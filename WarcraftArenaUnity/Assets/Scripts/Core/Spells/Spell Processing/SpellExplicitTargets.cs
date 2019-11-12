using UnityEngine;

namespace Core
{
    public class SpellExplicitTargets
    {
        public Unit Target { get; internal set; }
        public Vector3 Source { get; internal set; }
        public Vector3? Destination { get; internal set; }

        public void Dispose()
        {
            Target = null;
        }
    }
}