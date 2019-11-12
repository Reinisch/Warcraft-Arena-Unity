using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Spell Sound Settings", menuName = "Game Data/Sound/Spell Sound Settings", order = 1)]
    public class SpellSoundSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private List<SpellSoundEntry> soundEntries = new List<SpellSoundEntry>();

        public SpellInfo SpellInfo => spellInfo;

        public void PlayAtPoint(Vector3 point, SpellSoundEntry.UsageType usageType)
        {
            soundEntries.Find(entry => entry.SoundUsageType == usageType)?.PlayAtPoint(point);
        }
    }
}