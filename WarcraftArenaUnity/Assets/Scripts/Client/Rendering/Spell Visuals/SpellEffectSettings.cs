using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Effect Settings", menuName = "Game Data/Visuals/Spell Effect Settings", order = 2)]
    public class SpellEffectSettings : ScriptableObject
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

