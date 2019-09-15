using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class SpellSoundEntry
    {
        public enum UsageType
        {
            Cast,
            Projectile,
            Impact,
            Aura,
            Destination
        }

        [SerializeField, UsedImplicitly] private UsageType soundUsageType;
        [SerializeField, UsedImplicitly] private SoundEntry soundEntry;

        public UsageType SoundUsageType => soundUsageType;

        public void PlayAtPoint(Vector3 point) => soundEntry?.PlayAtPoint(point);
    }
}