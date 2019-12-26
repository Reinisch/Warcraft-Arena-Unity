using System.Collections.Generic;
using Client.Localization;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Arena.Editor
{
    [CustomEditor(typeof(LocalizedString), true), UsedImplicitly]
    internal class LocalizedStringEditor : UnityEditor.Editor
    {
        private LocalizedString localizedString;
        private List<(LocalizedLanguage, LocalizedLanguage.LocalizationEntry)> languageEntries;

        [UsedImplicitly]
        private void OnEnable()
        {
            localizedString = (LocalizedString)serializedObject.targetObject;
            languageEntries = new List<(LocalizedLanguage, LocalizedLanguage.LocalizationEntry)>();

            foreach (LocalizedLanguage language in Resources.LoadAll<LocalizedLanguage>("Languages/"))
            {
                LocalizedLanguage.LocalizationEntry entry = language.Entries.Find(item => item.StringReference == localizedString);
                if (entry == null)
                {
                    entry = new LocalizedLanguage.LocalizationEntry {StringReference = localizedString, Value = string.Empty};
                    language.Entries.Add(entry);

                    EditorUtility.SetDirty(language);
                }

                languageEntries.Add((language, entry));
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach ((LocalizedLanguage, LocalizedLanguage.LocalizationEntry) entry in languageEntries)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField($"Language: {entry.Item1.LanguageType}", new GUIStyle { fontStyle = FontStyle.Bold });

                EditorGUILayout.BeginHorizontal();
                
                entry.Item2.Value = EditorGUILayout.TextArea(entry.Item2.Value, GUI.skin.GetStyle("TextArea"));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Populate English Everywhere"))
            {
                var englishEntry = languageEntries.Find(entry => entry.Item1.LanguageType == LocalizedLanguageType.English);
                if (englishEntry != default)
                    foreach ((LocalizedLanguage, LocalizedLanguage.LocalizationEntry) entry in languageEntries)
                        entry.Item2.Value = englishEntry.Item2.Value;
            }

            if (GUILayout.Button("Save Changes"))
            {
                foreach (LocalizedLanguage language in Resources.LoadAll<LocalizedLanguage>("Languages/"))
                    EditorUtility.SetDirty(language.gameObject);

                AssetDatabase.SaveAssets();
            }
        }
    }
}
