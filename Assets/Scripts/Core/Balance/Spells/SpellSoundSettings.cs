using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpellSoundSettings
    {
        [SerializeField, UsedImplicitly] private List<SpellSoundEntry> soundEntries = new List<SpellSoundEntry>();

        public AudioClip FindSound(SpellSoundEntry.UsageType usageType)
        {
            foreach (var entry in soundEntries)
                if (entry.SoundUsageType == usageType)
                    return entry.AudioClip;

            return null;
        }
    }
}

