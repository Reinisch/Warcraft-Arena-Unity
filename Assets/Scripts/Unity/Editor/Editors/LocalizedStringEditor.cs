using Client.Localization;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Game.Editor
{
    [CustomEditor(typeof(LocalizedString), true)]
    internal class LocalizedStringEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var localizedString = (LocalizedString)target;
            
            EditorGUILayout.LabelField("Localization Key", localizedString.name, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This asset's name is used as the key in the 'GameStrings' table collection.", MessageType.Info);
            EditorGUILayout.Space();

            var collection = LocalizationEditorSettings.GetStringTableCollection("GameStrings");
            if (collection == null)
            {
                EditorGUILayout.HelpBox("String Table Collection 'GameStrings' not found. Please ensure the migration was successful and the collection exists.", MessageType.Error);
                return;
            }

            EditorGUI.BeginChangeCheck();
            
            var locales = LocalizationEditorSettings.GetLocales();
            foreach (var locale in locales)
            {
                var table = collection.GetTable(locale.Identifier) as StringTable;
                if (table == null)
                {
                    EditorGUILayout.LabelField($"Locale: {locale.Identifier.Code}", EditorStyles.miniBoldLabel);
                    EditorGUILayout.HelpBox($"Table for {locale.name} not found in collection.", MessageType.Warning);
                    continue;
                }

                var entry = table.GetEntry(localizedString.name);
                string value = entry != null ? entry.Value : string.Empty;

                EditorGUILayout.LabelField($"Language: {locale.name} ({locale.Identifier.Code})", EditorStyles.miniBoldLabel);
                
                GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true
                };

                string newValue = EditorGUILayout.TextArea(value, textAreaStyle, GUILayout.MinHeight(40));

                if (newValue != value)
                {
                    Undo.RecordObject(table, "Update Localization Entry");
                    if (entry == null)
                    {
                        table.AddEntry(localizedString.name, newValue);
                    }
                    else
                    {
                        entry.Value = newValue;
                    }
                    EditorUtility.SetDirty(table);
                    EditorUtility.SetDirty(table.SharedData);
                }
                
                EditorGUILayout.Space(2);
            }

            if (EditorGUI.EndChangeCheck())
            {
                // Raise modified event to update other windows
                LocalizationEditorSettings.EditorEvents.RaiseCollectionModified(this, collection);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Localization Tables Window"))
            {
                EditorApplication.ExecuteMenuItem("Window/Asset Management/Localization Tables");
            }
        }
    }
}