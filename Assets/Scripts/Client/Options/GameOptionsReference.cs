using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Game Options Reference", menuName = "Game Data/Scriptable/Game Options", order = 1)]
    public class GameOptionsReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] private List<GameOptionItem> options;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            options.ForEach(option => option.Load());
        }

        protected override void OnUnregister()
        {
            options.ForEach(option => option.Save());

            base.OnUnregister();
        }

#if UNITY_EDITOR
        [ContextMenu("Validate"), UsedImplicitly]
        private void Validate()
        {
            var optionsNames = new HashSet<string>();
            foreach (GameOptionItem option in options)
            {
                if (optionsNames.Contains(option.name))
                    Debug.LogError($"Option {option.name} is duplicated!");
                else
                    optionsNames.Add(option.name);
            }
        }

        [ContextMenu("Collect"), UsedImplicitly]
        private void CollectOptions()
        {
            options.Clear();

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(GameOptionItem)}", null))
                options.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<GameOptionItem>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));
        }
#endif
    }
}
