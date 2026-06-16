using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public class GameOptionsReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] private GameOptionItemContainer options;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            IReadOnlyList<GameOptionItem> items = options.ItemList;
            for (int i = 0; i < items.Count; i++)
                items[i].Load();
        }

        protected override void OnUnregister()
        {
            IReadOnlyList<GameOptionItem> items = options.ItemList;
            for (int i = 0; i < items.Count; i++)
                items[i].Save();

            base.OnUnregister();
        }

        protected override void QueueForInject(DiContainer container)
        {
            base.QueueForInject(container);

            options.QueueForInject(container);
        }

#if UNITY_EDITOR
        [ContextMenu("Validate"), UsedImplicitly]
        private void Validate()
        {
            var optionsNames = new HashSet<string>();
            foreach (GameOptionItem option in options.ItemList)
            {
                if (!optionsNames.Add(option.name))
                    Debug.LogError($"Option {option.name} is duplicated!");
            }
        }
#endif
    }
}
