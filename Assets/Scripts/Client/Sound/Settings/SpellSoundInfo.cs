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
        [SerializeField, UsedImplicitly] private SpellSoundInfoContainer container;
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private List<SpellSoundEntry> soundEntries = new List<SpellSoundEntry>();

        protected override SpellSoundInfo Data => this;
        protected override ScriptableUniqueInfoContainer<SpellSoundInfo> Container => container;

        public SpellInfo SpellInfo => spellInfo;

        public void PlayAtPoint(Vector3 point, SpellSoundEntry.UsageType usageType)
        {
            soundEntries.Find(entry => entry.SoundUsageType == usageType)?.PlayAtPoint(point);
        }
    }
}