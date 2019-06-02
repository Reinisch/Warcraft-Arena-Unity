using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Core;
using JetBrains.Annotations;
using Common;

[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    private Entity entity;
    private List<EntityField> entityFieldList;

    private bool entityFoldout;
    private int entityIndex = -1;
    private ReorderableList entityValuesList;

    [UsedImplicitly]
    private void OnEnable()
    {
        FieldInfo field = typeof(Entity).GetField("defaultFieldValues", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field);

        entity = (Entity)target;
        entityFieldList = (List<EntityField>)field.GetValue(entity);

        entityValuesList = new ReorderableList(entityFieldList, typeof(EntityField), true, true, true, true);
        entityValuesList.drawHeaderCallback += delegate(Rect rect)
        {
            GUI.Label(rect, "Default Values");
        };

        entityValuesList.drawElementCallback += delegate (Rect rect, int index, bool active, bool focused)
        {
            EntityField entityField = entityFieldList[index];

            var newRect = new Rect(rect);
            newRect.width *= 0.8f;
            EditorGUI.LabelField(newRect, $"{entityField.Field} - {entityField.FieldType}");
            newRect.position += new Vector2(newRect.width * 0.8f, 0.0f);
            EditorGUI.LabelField(newRect, $"{entityField.Value}");

            if (active)
            {
                entityIndex = index;
            }
        };

        entityValuesList.onAddCallback += delegate
        {
            entityFieldList.Add(new EntityField(EntityFields.Health, FieldTypes.Int));
            EditorUtility.SetDirty(target);
        };

        entityValuesList.onRemoveCallback += delegate (ReorderableList list)
        {
            entityIndex = -1;
            entityFieldList.RemoveAt(list.index);

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(target);
        };
    }

    public override void OnInspectorGUI()
    {
        entityIndex = -1;

        while (true)
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Entity", new GUIStyle { fontStyle = FontStyle.Bold, padding = new RectOffset(0, 0, 5, 2)});

            entityFoldout = EditorGUILayout.Foldout(entityFoldout, "Entity Fields");
            if (!entityFoldout)
                break;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Populate all default fields"))
            {
                entityIndex = -1;
                entityFieldList.Clear();

                EntityType targetEntityType = EntityType.Entity;
                if (target is Player)
                {
                    targetEntityType = EntityType.Player;
                }
                else if (target is Creature)
                {
                    targetEntityType = EntityType.Creature;
                }

                foreach (var entityField in EntityFieldHelper.GetEntityFields(targetEntityType))
                {
                    entityFieldList.Add(new EntityField(entityField, entityField.GetFieldType()));
                }
            }
            EditorGUILayout.EndHorizontal();

            entityValuesList.DoLayoutList();
            if (entityIndex == -1)
                break;

            EditorGUILayout.BeginVertical();

            var entityFieldsProperty = serializedObject.FindProperty("defaultFieldValues").GetArrayElementAtIndex(entityIndex);
            foreach (var serializedProperty in EditorSerializationHelper.GetVisibleChildren(entityFieldsProperty))
            {
                EntityField selectedEntity = entityFieldList[entityIndex];
                if (serializedProperty.name == "FieldValue")
                {
                    switch (selectedEntity.FieldType)
                    {
                        case FieldTypes.Int:
                            selectedEntity.FieldValue = (EntityFieldValue)EditorGUILayout.IntField("Value", selectedEntity.FieldValue.Int);
                            break;
                        case FieldTypes.Uint:
                            selectedEntity.FieldValue = (EntityFieldValue)(uint)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.FieldValue.UInt), uint.MinValue, uint.MaxValue);
                            break;
                        case FieldTypes.Ulong:
                            selectedEntity.FieldValue = (EntityFieldValue)(ulong)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.FieldValue.Long), ulong.MinValue, ulong.MaxValue);
                            break;
                        case FieldTypes.Long:
                            selectedEntity.FieldValue = (EntityFieldValue)EditorGUILayout.LongField("Value", selectedEntity.FieldValue.Long);
                            break;
                        case FieldTypes.Float:
                            selectedEntity.FieldValue = (EntityFieldValue)EditorGUILayout.FloatField("Value", selectedEntity.FieldValue.Float);
                            break;
                        case FieldTypes.Double:
                            selectedEntity.FieldValue = (EntityFieldValue)EditorGUILayout.DoubleField("Value", selectedEntity.FieldValue.Double);
                            break;
                        case FieldTypes.Short:
                            selectedEntity.FieldValue = (EntityFieldValue)(short)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.FieldValue.Short), short.MinValue, short.MaxValue);
                            break;
                        case FieldTypes.UShort:
                            selectedEntity.FieldValue = (EntityFieldValue)(ushort)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.FieldValue.UShort), ushort.MinValue, ushort.MaxValue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedProperty, true);
                }
            }

            EditorGUILayout.EndVertical();

            break;
        }

        DrawPropertiesExcluding(serializedObject, "defaultFieldValues", "m_Script");

        serializedObject.ApplyModifiedProperties();
    }
}
