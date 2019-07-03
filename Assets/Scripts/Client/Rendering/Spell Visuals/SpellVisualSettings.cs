using System;
using System.Collections.Generic;
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
        [SerializeField, UsedImplicitly] private List<EffectSpellSettings> visualEffects = new List<EffectSpellSettings>();

        private readonly Dictionary<EffectSpellSettings.UsageType, EffectSpellSettings> visualsByUsage = new Dictionary<EffectSpellSettings.UsageType, EffectSpellSettings>();

        public SpellInfo SpellInfo => spellInfo;
        public Sprite SpellIcon => spellIcon;
        public IReadOnlyDictionary<EffectSpellSettings.UsageType, EffectSpellSettings> VisualsByUsage => visualsByUsage;

        public void Initialize()
        {
            visualEffects.ForEach(effect => visualsByUsage.Add(effect.VisualUsageType, effect));
        }

        public void Deinitialize()
        {
            visualsByUsage.Clear();
        }
    }
}
