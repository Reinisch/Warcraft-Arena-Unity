using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Core;

[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    private Entity entity;
    private List<EntityField> entityFieldList;

    private bool entityFoldout;
    private int entityIndex = -1;
    private ReorderableList entityValuesList;

    private void OnEnable()
    {
        FieldInfo field = typeof(Entity).GetField("entityFieldList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        entity = (Entity)target;
        entityFieldList = (List<EntityField>)field.GetValue(entity);

        entityValuesList = new ReorderableList(entityFieldList, typeof(EntityField), true, true, true, true);
        entityValuesList.drawHeaderCallback += delegate(Rect rect)
        {
            GUI.Label(rect, "Entity Fields");
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
            if (GUILayout.Button("Populate default fields"))
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

            var entityFieldsProperty = serializedObject.FindProperty("entityFieldList").GetArrayElementAtIndex(entityIndex);
            foreach (var serializedProperty in EditorSerializationHelper.GetVisibleChildren(entityFieldsProperty))
            {
                EntityField selectedEntity = entityFieldList[entityIndex];
                if (serializedProperty.name == "Long")
                {
                    switch (selectedEntity.FieldType)
                    {
                        case FieldTypes.Int:
                            selectedEntity.Int = EditorGUILayout.IntField("Value", selectedEntity.Int);
                            break;
                        case FieldTypes.Uint:
                            selectedEntity.UInt = (uint)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.UInt), uint.MinValue, uint.MaxValue);
                            break;
                        case FieldTypes.Ulong:
                            selectedEntity.ULong = (ulong)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.Long), ulong.MinValue, ulong.MaxValue);
                            break;
                        case FieldTypes.Long:
                            selectedEntity.Long = EditorGUILayout.LongField("Value", selectedEntity.Long);
                            break;
                        case FieldTypes.Float:
                            selectedEntity.Float = EditorGUILayout.FloatField("Value", selectedEntity.Float);
                            break;
                        case FieldTypes.Double:
                            selectedEntity.Double = EditorGUILayout.DoubleField("Value", selectedEntity.Double);
                            break;
                        case FieldTypes.Short:
                            selectedEntity.Short = (short)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.Short), short.MinValue, short.MaxValue);
                            break;
                        case FieldTypes.UShort:
                            selectedEntity.UShort = (ushort)Mathf.Clamp(EditorGUILayout.LongField("Value", selectedEntity.UShort), ushort.MinValue, ushort.MaxValue);
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

        DrawPropertiesExcluding(serializedObject, "entityFieldList", "m_Script");

        serializedObject.ApplyModifiedProperties();
    }
}
