using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Common
{
    public abstract class ScriptableUniqueInfoContainer<TUnique> : ScriptableObject, IScriptablePostProcess where TUnique : ScriptableUniqueInfo<TUnique>
    {
        [SerializeField]
        private List<TUnique> items = new();

        protected List<TUnique> Items => items;

        public IReadOnlyList<TUnique> ItemList => items;

        public virtual void Register()
        {
            items.ForEach(item => item.Register());
        }

        public virtual void Unregister()
        {
            items.ForEach(item => item.Unregister());
        }

        public void QueueForInject(DiContainer container)
        {
            container.QueueForInject(this);

            items.ForEach(container.QueueForInject);
        }

#if UNITY_EDITOR
        internal List<TUnique> EditorList => items;
#endif

        bool IScriptablePostProcess.OnPostProcess(bool isDeleted)
        {
            if (items == null)
                return false;

            bool hasChanges = false;
#if UNITY_EDITOR
            if (isDeleted)
                return false;

            for (int i = items.Count - 1; i >= 0; i--)
                if (items[i] == null)
                {
                    hasChanges = true;
                    items.RemoveAt(i);
                }
#endif
            return hasChanges;
        }
    }
}
