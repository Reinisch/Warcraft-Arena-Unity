using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System.Linq;

namespace Client.Localization
{
    internal class LocalizedLanguage : MonoBehaviour
    {
        [Serializable]
        internal class LocalizationEntry
        {
            public LocalizedString StringReference;
            [TextArea] public string Value;
        }

        [SerializeField, UsedImplicitly] private LocalizedLanguageType languageType;
        [SerializeField, UsedImplicitly] private List<LocalizationEntry> entries;

        internal LocalizedLanguageType LanguageType => languageType;
        internal List<LocalizationEntry> Entries => entries;

        internal void Localize()
        {
            foreach (var entry in entries)
                entry.StringReference.Value = entry.Value;
        }

#if UNITY_EDITOR
        [ContextMenu("Add missing strings"), UsedImplicitly]
        private void AddMissingStrings()
        {
            HashSet<LocalizedString> addedStrings = new HashSet<LocalizedString>(entries.Select(item => item.StringReference));

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(LocalizedString)}", null))
            {
                LocalizedString otherString = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizedString>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                if (!addedStrings.Contains(otherString))
                    entries.Add(new LocalizationEntry {StringReference = otherString });
            }
        }

        [ContextMenu("Populate empty from English"), UsedImplicitly]
        private void PopulateEmptyFromEnglish()
        {
            AddMissingStrings();

            var englishEntries = Resources.Load<LocalizedLanguage>($"Languages/{LocalizedLanguageType.English}").entries;
            var englishStrings = new Dictionary<LocalizedString, string>();

            englishEntries.ForEach(entry => englishStrings.Add(entry.StringReference, entry.Value));

            foreach (LocalizationEntry entry in entries)
                if (string.IsNullOrEmpty(entry.Value) && englishStrings.ContainsKey(entry.StringReference))
                    entry.Value = englishStrings[entry.StringReference];
        }
#endif
    }
}
