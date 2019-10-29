using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Common
{
    public abstract class ScriptableUniqueInfoContainer<TUnique> : ScriptableObject where TUnique : ScriptableUniqueInfo<TUnique>
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

        [UsedImplicitly]
        public void OnValidate()
        {
            if (Items == null)
                return;

            for(int i = Items.Count - 1; i >= 0; i--)
               if (Items[i] == null)
                   Items.RemoveAt(i);
        }
#endif
    }
}
