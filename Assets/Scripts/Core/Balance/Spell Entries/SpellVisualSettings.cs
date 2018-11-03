using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class SpellVisualSettings
    {
        [SerializeField, UsedImplicitly] private Sprite spellIcon;
        [SerializeField, UsedImplicitly] private Sprite activeIcon;
        [SerializeField, UsedImplicitly] private List<SpellVisualEntry> visualEntries = new List<SpellVisualEntry>();

        public Sprite SpellIcon => spellIcon;
        public Sprite ActiveIcon => activeIcon;

        public GameObject FindEffect(SpellVisualEntry.UsageType usageType)
        {
            return visualEntries.Find(entry => entry.VisualUsageType == usageType)?.EffectPrototype;
        }
    }
}

