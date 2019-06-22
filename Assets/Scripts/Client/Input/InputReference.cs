using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Input Reference", menuName = "Game Data/Scriptable/Input", order = 1)]
    public class InputReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private List<HotkeyInputItem> hotkeys;
        [SerializeField, UsedImplicitly] private List<InputActionGlobal> globalActions;

        private Player Player { get; set; }

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            globalActions.ForEach(globalAction => globalAction.Register());
        }

        protected override void OnUnregister()
        {
            globalActions.ForEach(globalAction => globalAction.Unregister());

            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        protected override void OnUpdate(float deltaTime)
        {
            foreach (var hotkey in hotkeys)
                if (hotkey.IsPressed())
                    EventHandler.ExecuteEvent(hotkey, GameEvents.HotkeyPressed);
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        private void OnPlayerControlGained(Player player)
        {
            Player = player;
        }

        private void OnPlayerControlLost(Player player)
        {
            Player = null;
        }

        public void SelectTarget(Unit target)
        {
            if (Player == null)
                return;

            Player.SetTarget(target);

            TargetSelectionRequestEvent targetSelectionRequest = TargetSelectionRequestEvent.Create(Bolt.GlobalTargets.OnlyServer);
            targetSelectionRequest.TargetId = target?.BoltEntity.NetworkId ?? default;
            targetSelectionRequest.Send();
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
