using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Common
{
    public abstract class ScriptableUniqueInfo<TUnique> : ScriptableObject
    {
        [SerializeField, UsedImplicitly, HideInInspector] private int id;

        protected int Id => id;

#if UNITY_EDITOR
        [UsedImplicitly]
        protected void Awake()
        {
            OnValidate();
        }

        [UsedImplicitly]
        protected void OnValidate()
        {
            if (id <= 0)
            {
                var takenIds = new HashSet<int>();
                foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{GetType()}", null))
                {
                    string infoAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableUniqueInfo<TUnique> anotherInfo = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableUniqueInfo<TUnique>>(infoAssetPath);
                    takenIds.Add(anotherInfo.Id);
                }

                for (int i = 1; i < int.MaxValue; i++)
                {
                    if (!takenIds.Contains(i))
                    {
                        id = i;
                        Debug.Log($"Assigned id:{i} to {GetType().Name}: {name}");
                        UnityEditor.EditorUtility.SetDirty(this);
                        UnityEditor.AssetDatabase.SaveAssets();
                        break;
                    }
                }
            }
        }
#endif
    }
}
