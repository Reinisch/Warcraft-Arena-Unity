using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Spell Sound Info", menuName = "Game Data/Sound/Spell Sound Info", order = 1)]
    public class SpellSoundInfo : ScriptableUniqueInfo<SpellSoundInfo>
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private List<SpellSoundEntry> soundEntries = new();

        public SpellInfo SpellInfo => spellInfo;

        public void PlayAtPoint(Vector3 point, SpellSoundEntry.UsageType usageType)
        {
            foreach (SpellSoundEntry entry in soundEntries)
                if (entry.SoundUsageType == usageType)
                    entry.PlayAtPoint(point);
        }
    }
}