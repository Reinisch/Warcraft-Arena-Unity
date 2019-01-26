using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Core;
using JetBrains.Annotations;

[CustomEditor(typeof(BalanceDefinition)), UsedImplicitly]
public class BalanceDefinitionEditor : Editor
{
    private const string SpellInfoPath = "Assets/Balance/Balance Definitions/";

    private BalanceDefinition balanceDefinition;

    private bool spellInfoFoldout;
    private bool spellInfoReorderableFoldout;
    private bool spellInfoSelectionFoldout;
    
    private int spellInfoIndex = -1;
    private ReorderableList spellInfoList;

    private bool spellEffectReorderableFoldout;
    private bool spellEffectSelectionFoldout;

    private int spellEffectIndex = -1;
    private ReorderableList spellEffectList;

    private Editor cachedSpellEditor;
    private Editor cachedSpellInfoEditor;

    private void OnEnable()
    {
        balanceDefinition = (BalanceDefinition)target;
        spellInfoList = new ReorderableList(balanceDefinition.SpellInfos, typeof(SpellInfo), true, true, true, true);
        spellInfoList.drawHeaderCallback += delegate(Rect rect) { GUI.Label(rect, "Spell Infos"); };

        spellInfoList.drawElementCallback += delegate(Rect rect, int index, bool active, bool focused)
        {
            SpellInfo drawnSpellInfo = balanceDefinition.SpellInfos[index];
            EditorGUI.LabelField(rect, $"Id:{drawnSpellInfo.Id} {drawnSpellInfo.SpellName}");

            if (active)
            {
                spellInfoIndex = index;

                UpdateSpellEffectList(drawnSpellInfo);
            }
        };

        spellInfoList.onAddCallback += delegate
        {
            string spellDirectory = SpellInfoPath + balanceDefinition.name + "/Spells/";
            if (!Directory.Exists(spellDirectory))
                Directory.CreateDirectory(spellDirectory);

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(spellDirectory + "SpellInfo.asset");

            var newSpellInfo = CreateInstance<SpellInfo>();
            balanceDefinition.SpellInfos.Add(newSpellInfo);
            AssetDatabase.CreateAsset(newSpellInfo, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(target);
        };

        spellInfoList.onRemoveCallback += delegate(ReorderableList list)
        {
            spellEffectIndex = -1;
            spellInfoIndex = -1;

            SpellInfo removedSpellInfo = balanceDefinition.SpellInfos[list.index];
            balanceDefinition.SpellInfos.RemoveAt(list.index);
            foreach (var effectInfo in removedSpellInfo.Effects)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(effectInfo));

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(removedSpellInfo));
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
            spellInfoFoldout = EditorGUILayout.Foldout(spellInfoFoldout, "Spell Entries");
            if (!spellInfoFoldout)
                break;

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spellInfos"), true);

            spellInfoReorderableFoldout = EditorGUILayout.Foldout(spellInfoReorderableFoldout, "Reorderable Spell List");
            if (spellInfoReorderableFoldout)
                spellInfoList.DoLayoutList();

            if (spellInfoIndex == -1)
                break;

            spellInfoSelectionFoldout = EditorGUILayout.Foldout(spellInfoSelectionFoldout, "Selected Spell");
            if (!spellInfoSelectionFoldout)
                break;

            var spellInfoProperty = serializedObject.FindProperty("spellInfos").GetArrayElementAtIndex(spellInfoIndex);
            CreateCachedEditor(spellInfoProperty.objectReferenceValue, typeof(Editor), ref cachedSpellEditor);
            cachedSpellEditor.OnInspectorGUI();

            spellEffectReorderableFoldout = EditorGUILayout.Foldout(spellEffectReorderableFoldout, "Reorderable Effect List");
            if (spellEffectReorderableFoldout)
            {
                spellEffectList.DoLayoutList();

                if (spellEffectIndex != -1)
                {
                    spellEffectSelectionFoldout = EditorGUILayout.Foldout(spellEffectSelectionFoldout, "Selected Effect");
                    if (!spellEffectSelectionFoldout)
                        break;

                    CreateCachedEditor(balanceDefinition.SpellInfos[spellInfoIndex].Effects[spellEffectIndex], typeof(Editor), ref cachedSpellInfoEditor);
                    cachedSpellInfoEditor.OnInspectorGUI();
                }
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
            SpellEffectInfo drawnSpellEffectInfo = balanceDefinition.SpellInfos[spellInfoIndex].Effects[index];
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
                    string spellDirectory = SpellInfoPath + balanceDefinition.name + "/Spells/Effects/" + spellInfo.name + " Effects/";

                    if (!Directory.Exists(spellDirectory))
                        Directory.CreateDirectory(spellDirectory);

                    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(spellDirectory + spellInfo.name + " Effect.asset");
                    var newEffectInfo = (SpellEffectInfo)CreateInstance(effectType.Name);
                    AssetDatabase.CreateAsset(newEffectInfo, assetPathAndName);

                    balanceDefinition.SpellInfos[spellInfoIndex].Effects.Add(newEffectInfo);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.SetDirty(target);
                });
            }
            menu.ShowAsContext();
        };

        spellEffectList.onRemoveCallback += delegate (ReorderableList list)
        {
            spellEffectIndex = -1;
            cachedSpellInfoEditor = null;

            SpellEffectInfo removedSpellEffectInfo = balanceDefinition.SpellInfos[spellInfoIndex].Effects[list.index];
            balanceDefinition.SpellInfos[spellInfoIndex].Effects.Remove(removedSpellEffectInfo);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(removedSpellEffectInfo));
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(target);
        };
    }
}
