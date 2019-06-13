using System.Collections.Generic;
using Client.Spells;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Rendering Reference", menuName = "Game Data/Rendering/Rendering Reference", order = 1)]
    public class RenderingReference : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private List<SpellVisualSettings> spellVisualSettings;
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;

        private readonly Dictionary<int, SpellVisualSettings> spellVisualSettingsById = new Dictionary<int, SpellVisualSettings>();

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public IReadOnlyDictionary<int, SpellVisualSettings> SpellVisualSettingsById => spellVisualSettingsById;

        public void Initialize()
        {
            spellVisualSettings.ForEach(visual => spellVisualSettingsById.Add(visual.SpellInfo.Id, visual));
            spellVisualSettings.ForEach(visual => visual.Initialize());
        }

        public void Deinitialize()
        {
            spellVisualSettings.ForEach(visual => visual.Deinitialize());
            spellVisualSettingsById.Clear();
        }
    }
}
