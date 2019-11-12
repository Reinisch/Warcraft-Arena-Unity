using System.IO;
using Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Arena.Editor
{
    [CustomEditor(typeof(AuraInfo), true), CanEditMultipleObjects, UsedImplicitly]
    internal class AuraInfoEditor : ScriptableObjectEditor { }

    [CustomEditor(typeof(AuraEffectInfo), true), CanEditMultipleObjects, UsedImplicitly]
    internal class AuraEffectInfoEditor : ScriptableObjectEditor { }

    [CustomEditor(typeof(SpellEffectInfo), true), CanEditMultipleObjects, UsedImplicitly]
    internal class SpellEffectInfoEditor : ScriptableObjectEditor { }

    [CustomEditor(typeof(AuraScriptable), true), CanEditMultipleObjects, UsedImplicitly]
    internal class AuraScriptableEditor : ScriptableObjectEditor { }

    [CanEditMultipleObjects, UsedImplicitly]
    internal class ScriptableObjectEditor : UnityEditor.Editor
    {
        private ScriptableObject spellInfo;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Set as child to parent asset: ", new GUIStyle() { fontStyle = FontStyle.Bold });

            EditorGUILayout.BeginHorizontal();
            spellInfo = (ScriptableObject)EditorGUILayout.ObjectField(spellInfo, typeof(SpellInfo), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add to Parent") && spellInfo != null)
            {
                if (AssetDatabase.IsSubAsset(target))
                    AssetDatabase.RemoveObjectFromAsset(target);
                else
                {
                    string assetPath = AssetDatabase.GetAssetPath(target);
                    AssetDatabase.RemoveObjectFromAsset(target);
                    AssetDatabase.DeleteAsset(assetPath);
                }

                AssetDatabase.AddObjectToAsset(target, spellInfo);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                spellInfo = null;
            }
            if (GUILayout.Button("Remove From Parent") && AssetDatabase.IsSubAsset(target))
            {
                string assetPath = $"{Path.GetDirectoryName(AssetDatabase.GetAssetPath(target))}{'\\'}{target.name}.asset";
                AssetDatabase.RemoveObjectFromAsset(target);
                AssetDatabase.CreateAsset(target, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}
