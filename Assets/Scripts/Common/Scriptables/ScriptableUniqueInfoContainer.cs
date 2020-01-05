using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public abstract class ScriptableUniqueInfoContainer<TUnique> : ScriptableObject, IScriptablePostProcess where TUnique : ScriptableUniqueInfo<TUnique>
    {
        protected abstract List<TUnique> Items { get; }

        public IReadOnlyList<TUnique> ItemList => Items;

        public virtual void Register()
        {
            Items.ForEach(item => item.Register());
        }

        public virtual void Unregister()
        {
            Items.ForEach(item => item.Unregister());
        }

#if UNITY_EDITOR
        public List<TUnique> EditorList => Items;
#endif

        bool IScriptablePostProcess.OnPostProcess(bool isDeleted)
        {
            if (Items == null)
                return false;

            bool hasChanges = false;
#if UNITY_EDITOR
            if (isDeleted)
                return false;

            for(int i = Items.Count - 1; i >= 0; i--)
                if (Items[i] == null)
                {
                    hasChanges = true;
                    Items.RemoveAt(i);
                }
#endif
            return hasChanges;
        }
    }
}
