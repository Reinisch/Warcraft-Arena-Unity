using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Common
{
    public abstract class ScriptableUniqueInfo<TUnique> : ScriptableObject where TUnique : ScriptableUniqueInfo<TUnique>
    {
        [SerializeField, UsedImplicitly, HideInInspector] private int id;

        protected int Id => id;
        protected abstract TUnique Data { get; }
        protected abstract ScriptableUniqueInfoContainer<TUnique> Container { get; }

#if UNITY_EDITOR
        [UsedImplicitly]
        protected virtual void OnValidate()
        {
            var takenIds = new HashSet<int>();

            if (Container != null)
            {
                Container.OnValidate();

                if (Container.EditorList.Contains(Data))
                    return;

                foreach (var item in Container.ItemList)
                    takenIds.Add(item.Id);

                if (id != 0)
                {
                    if (!takenIds.Contains(Id))
                    {
                        Container.EditorList.Add(Data);
                        UnityEditor.EditorUtility.SetDirty(Container);
                        UnityEditor.AssetDatabase.SaveAssets();

                        Debug.Log($"Added existing item: {GetType().Name}: {name} id:{id} to container {Container.GetType()}");
                    }
                    else
                    {
                        Debug.Log($"Resetting id for existing item : {GetType().Name}: {name} id:{id} in container {Container.GetType()}");
                        UnityEditor.EditorUtility.SetDirty(this);
                        UnityEditor.AssetDatabase.SaveAssets();
                        id = 0;
                    }
                }
            }

            if (id <= 0)
            {
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

                        if (Container != null && !Container.EditorList.Contains(Data))
                        {
                            Debug.Log($"Added new item: {GetType().Name}: {name} id:{id} to container {Container.GetType()}");
                            Container.EditorList.Add(Data);
                            UnityEditor.EditorUtility.SetDirty(Container);
                        }

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
