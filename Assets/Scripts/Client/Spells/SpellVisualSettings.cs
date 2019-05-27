using System;
using System.Collections.Generic;
using Client.Effects;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [Serializable]
    public class SpellVisualSettings
    {
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private Sprite spellIcon;
        [SerializeField, UsedImplicitly] private Sprite activeIcon;
        [SerializeField, UsedImplicitly] private List<SpellVisualEffect> visualEffects = new List<SpellVisualEffect>();

        public SpellInfo SpellInfo => spellInfo;
        public Sprite SpellIcon => spellIcon;
        public Sprite ActiveIcon => activeIcon;

        public EffectSettings FindEffect(SpellVisualEffect.UsageType usageType)
        {
            return visualEffects.Find(entry => entry.VisualUsageType == usageType)?.EffectSettings;
        }
    }
}
