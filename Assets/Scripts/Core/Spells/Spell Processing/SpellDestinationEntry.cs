using UnityEngine;

namespace Core
{
    internal class SpellDestinationEntry
    {
        public Vector3 Destination { get; set; }
        public float Delay { get; set; }
        public bool Processed { get; set; }
        public int EffectMask { get; set; }
    }
}