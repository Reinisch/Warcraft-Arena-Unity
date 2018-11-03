using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
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
        }

        [SerializeField, UsedImplicitly] private UsageType soundUsageType;
        [SerializeField, UsedImplicitly] private AudioClip audioClip;

        public UsageType SoundUsageType => soundUsageType;
        public AudioClip AudioClip => audioClip;
    }
}