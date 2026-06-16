using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Client.Localization
{
    public abstract class LocalizationReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private LocalizedLanguageType defaultLanguage = LocalizedLanguageType.English;

        private static readonly List<LocalizedBehaviour> UsedBehaviours = new List<LocalizedBehaviour>();

        protected override void OnRegistered()
        {
            LoadLanguage(defaultLanguage);
        }

        protected override void OnUnregister()
        {
            UsedBehaviours.Clear();
        }

        internal static void AddBehaviour(LocalizedBehaviour behaviour)
        {
            behaviour.Localize();

            UsedBehaviours.Add(behaviour);
        }

        internal static void RemoveBehaviour(LocalizedBehaviour behaviour)
        {
            UsedBehaviours.Remove(behaviour);
        }

        private void LoadLanguage(LocalizedLanguageType languageType)
        {
            string localeCode = languageType switch
            {
                LocalizedLanguageType.English => "en",
                LocalizedLanguageType.Spanish => "es",
                LocalizedLanguageType.German => "de",
                LocalizedLanguageType.Italian => "it",
                _ => "en"
            };

            var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }

            foreach (var behaviour in UsedBehaviours)
                behaviour.Localize();
        }

        [ContextMenu("Set to English"), UsedImplicitly]
        private void SetEnglishLanguage()
        {
            LoadLanguage(LocalizedLanguageType.English);
        }

        [ContextMenu("Set to Spanish"), UsedImplicitly]
        private void SetSpanishLanguage()
        {
            LoadLanguage(LocalizedLanguageType.Spanish);
        }

        [ContextMenu("Set to German"), UsedImplicitly]
        private void SetGermanLanguage()
        {
            LoadLanguage(LocalizedLanguageType.German);
        }

        [ContextMenu("Set to Italian"), UsedImplicitly]
        private void SetItalianLanguage()
        {
            LoadLanguage(LocalizedLanguageType.Italian);
        }
    }
}
