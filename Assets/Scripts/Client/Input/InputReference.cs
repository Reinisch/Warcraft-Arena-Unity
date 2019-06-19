using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Input Reference", menuName = "Player Data/Input/Input Reference", order = 1)]
    public class InputReference : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private List<HotkeyInputItem> hotkeys;
        [SerializeField, UsedImplicitly] private List<InputActionGlobal> globalActions;

        public Player OriginalPlayer { get; private set; }

        public void Initialize()
        {
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            globalActions.ForEach(globalAction => globalAction.Register());
        }

        public void Deinitialize()
        {
            globalActions.ForEach(globalAction => globalAction.Unregister());

            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (var hotkey in hotkeys)
                if (hotkey.IsPressed())
                    EventHandler.ExecuteEvent(hotkey, GameEvents.HotkeyPressed);
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;
            }
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;
            }
        }

        private void OnEventEntityAttached(WorldEntity worldEntity)
        {
            if (worldEntity is Player player && player.IsOwner)
                OriginalPlayer = player;
        }

        private void OnEventEntityDetach(WorldEntity worldEntity)
        {
            if (worldEntity == OriginalPlayer)
                OriginalPlayer = null;
        }

        public void CastSpell(int spellId)
        {
            SpellCastRequestEvent spellCastRequest = SpellCastRequestEvent.Create(Bolt.GlobalTargets.OnlyServer);
            spellCastRequest.SpellId = spellId;
            spellCastRequest.Send();
        }

        public void StopCasting()
        {
            SpellCastCancelRequestEvent.Create(Bolt.GlobalTargets.OnlyServer).Send();
        }

#if UNITY_EDITOR
        [ContextMenu("Validate"), UsedImplicitly]
        private void OnValidate()
        {
            for (int i = 0; i < hotkeys.Count; i++)
            {
                for (int j = i + 1; j < hotkeys.Count; j++)
                {
                    if (hotkeys[i].HasSameInput(hotkeys[j]))
                        Debug.LogError($"{hotkeys[i].name} has the same input as {hotkeys[j].name}!");

                    if(hotkeys[i] == hotkeys[j])
                        Debug.LogError($"{hotkeys[i].name} assigned multiple times!");
                }
            }
        }

        [ContextMenu("Collect"), UsedImplicitly]
        private void CollectHotkeys()
        {
            hotkeys.Clear();
            globalActions.Clear();

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(HotkeyInputItem)}", null))
                hotkeys.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<HotkeyInputItem>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(InputActionGlobal)}", null))
                globalActions.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionGlobal>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));
        }
#endif
    }
}
