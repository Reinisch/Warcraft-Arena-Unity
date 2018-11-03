using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Core;

[CustomEditor(typeof(BalanceDefinition))]
public class BalanceDefinitionEditor : Editor
{
    private BalanceDefinition balanceDefiniton;

    private bool spellInfoFoldout;
    private int spellInfoIndex = -1;
    private ReorderableList spellInfoList;

    private bool spellEffectFoldout;
    private int spellEffectIndex = -1;
    private ReorderableList spellEffectList;

    private Editor cachedSpellInfoEditor;

    private void OnEnable()
    {
        balanceDefiniton = (BalanceDefinition)target;
        spellInfoList = new ReorderableList(balanceDefiniton.SpellInfos, typeof(SpellInfo), true, true, true, true);
        spellInfoList.drawHeaderCallback += delegate(Rect rect) { GUI.Label(rect, "Spell Infos"); };

        spellInfoList.drawElementCallback += delegate(Rect rect, int index, bool active, bool focused)
        {
            SpellInfo drawnSpellInfo = balanceDefiniton.SpellInfos[index];
            EditorGUI.LabelField(rect, $"Id:{drawnSpellInfo.Id} {drawnSpellInfo.SpellName}");

            if (active)
            {
                spellInfoIndex = index;

                UpdateSpellEffectList(drawnSpellInfo);
            }
        };

        spellInfoList.onAddCallback += delegate
        {
            balanceDefiniton.SpellInfos.Add(new SpellInfo());
            EditorUtility.SetDirty(target);
        };

        spellInfoList.onRemoveCallback += delegate(ReorderableList list)
        {
            spellEffectIndex = -1;
            spellInfoIndex = -1;

            SpellInfo removedSpellInfo = balanceDefiniton.SpellInfos[list.index];
            balanceDefiniton.SpellInfos.RemoveAt(list.index);
            foreach (var effectInfo in removedSpellInfo.Effects)
                DestroyImmediate(effectInfo, true);

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(target);
        };
    }

    public override void OnInspectorGUI()
    {
        spellEffectIndex = -1;
        spellInfoIndex = -1;

        while (true)
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "spellInfos");

            spellInfoFoldout = EditorGUILayout.Foldout(spellInfoFoldout, "SpellInfos");
            if (!spellInfoFoldout)
                break;

            spellInfoList.DoLayoutList();
            if (spellInfoIndex == -1)
                break;

            var spellInfoProperty = serializedObject.FindProperty("spellInfos").GetArrayElementAtIndex(spellInfoIndex);
            foreach (var serializedProperty in EditorSerializationHelper.GetVisibleChildren(spellInfoProperty))
            {
                if (serializedProperty.name == "spellEffectInfos")
                {
                    spellEffectFoldout = EditorGUILayout.Foldout(spellEffectFoldout, "Spell Effects");
                    if (!spellEffectFoldout)
                        continue;

                    spellEffectList.DoLayoutList();
                    if (spellEffectIndex != -1)
                    {
                        var selectedEffectProperty = spellInfoProperty.FindPropertyRelative("spellEffectInfos").GetArrayElementAtIndex(spellEffectIndex);
                        CreateCachedEditor(selectedEffectProperty.objectReferenceValue, typeof(Editor), ref cachedSpellInfoEditor);
                        cachedSpellInfoEditor.OnInspectorGUI();
                    }
                }
                else
                    EditorGUILayout.PropertyField(serializedProperty, true);
            }

            break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateSpellEffectList(SpellInfo spellInfo)
    {
        if (spellEffectList != null && ReferenceEquals(spellEffectList.list, spellInfo.Effects))
            return;

        spellEffectList = new ReorderableList(spellInfo.Effects, typeof(SpellEffectInfo), true, true, true, true);
        spellEffectList.drawHeaderCallback += delegate (Rect spellEffectRect) { GUI.Label(spellEffectRect, "Spell Effects"); };

        spellEffectList.drawElementCallback += delegate (Rect rect, int index, bool active, bool focused)
        {
            SpellEffectInfo drawnSpellEffectInfo = balanceDefiniton.SpellInfos[spellInfoIndex].Effects[index];
            string spellEffectInfoTypeName = ObjectNames.NicifyVariableName(drawnSpellEffectInfo.EffectType.ToString());
            EditorGUI.LabelField(rect, $"Type: {spellEffectInfoTypeName} Value: {drawnSpellEffectInfo.BasePoints}");

            if (active)
                spellEffectIndex = index;
        };

        spellEffectList.onAddDropdownCallback += delegate
        {
            var menu = new GenericMenu();
            foreach (Type effectType in typeof(SpellEffectInfo).Assembly.GetTypes().Where(type => typeof(SpellEffectInfo).IsAssignableFrom(type)))
            {
                if (effectType == typeof(SpellEffectInfo))
                    continue;

                string displayedEffectName = ObjectNames.NicifyVariableName(effectType.Name);
                menu.AddItem(new GUIContent("Add " + displayedEffectName), false, () =>
                {
                    var newEffectInfo = (SpellEffectInfo)CreateInstance(effectType.Name);
                    balanceDefiniton.SpellInfos[spellInfoIndex].Effects.Add(newEffectInfo);
                    newEffectInfo.hideFlags = HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(newEffectInfo, balanceDefiniton);
                    AssetDatabase.SaveAssets();
                    EditorUtility.SetDirty(target);
                });
            }
            menu.ShowAsContext();
        };

        spellEffectList.onRemoveCallback += delegate (ReorderableList list)
        {
            spellEffectIndex = -1;
            cachedSpellInfoEditor = null;

            SpellEffectInfo removedSpellEffectInfo = balanceDefiniton.SpellInfos[spellInfoIndex].Effects[list.index];
            balanceDefiniton.SpellInfos[spellInfoIndex].Effects.Remove(removedSpellEffectInfo);
            DestroyImmediate(removedSpellEffectInfo, true);

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(target);
        };
    }
}
