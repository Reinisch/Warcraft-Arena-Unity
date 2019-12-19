using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Spells
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Visual Info", menuName = "Game Data/Visuals/Spell Visual Info", order = 2)]
    public class SpellVisualsInfo : ScriptableUniqueInfo<SpellVisualsInfo>
    {
        [SerializeField, UsedImplicitly] private SpellVisualsInfoContainer container;
        [SerializeField, UsedImplicitly] private SpellInfo spellInfo;
        [SerializeField, UsedImplicitly] private Sprite spellIcon;
        [SerializeField, UsedImplicitly] private List<EffectSpellSettings> visualEffects = new List<EffectSpellSettings>();

        private readonly Dictionary<EffectSpellSettings.UsageType, EffectSpellSettings> visualsByUsage = new Dictionary<EffectSpellSettings.UsageType, EffectSpellSettings>();

        protected override SpellVisualsInfo Data => this;
        protected override ScriptableUniqueInfoContainer<SpellVisualsInfo> Container => container;

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

