using UnityEngine;

namespace Net
{
    /// <summary>
    /// Client → server: cast a spell using camera-based targeting — an explicit source position and
    /// rotation rather than a unit/destination target. (Was InputReference.CastSpellWithTargetingOptions.)
    /// </summary>
    public readonly struct SpellCastTargetingRequest : INetMessage
    {
        public int SpellId { get; }
        public Vector3 TargetingSource { get; }
        public Quaternion TargetingRotation { get; }

        public SpellCastTargetingRequest(int spellId, Vector3 targetingSource, Quaternion targetingRotation)
        {
            SpellId = spellId;
            TargetingSource = targetingSource;
            TargetingRotation = targetingRotation;
        }
    }
}
