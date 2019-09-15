using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Input Reference", menuName = "Game Data/Scriptable/Input", order = 1)]
    public class InputReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private TargetingSpellReference spellTargeting;
        [SerializeField, UsedImplicitly] private List<HotkeyInputItem> hotkeys;
        [SerializeField, UsedImplicitly] private List<InputActionGlobal> globalActions;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            globalActions.ForEach(globalAction => globalAction.Register());
            hotkeys.ForEach(hotkey => hotkey.Register());
        }

        protected override void OnUnregister()
        {
            hotkeys.ForEach(hotkey => hotkey.Unregister());
            globalActions.ForEach(globalAction => globalAction.Unregister());

            base.OnUnregister();
        }

        protected override void OnUpdate(float deltaTime)
        {
            hotkeys.ForEach(hotkey => hotkey.DoUpdate());
        }

        protected override void OnPlayerControlGained(Player player)
        {
            base.OnPlayerControlGained(player);

            Player.InputProvider = new ClientControllerMouseKeyboardInput(player, cameraReference);
        }

        protected override void OnPlayerControlLost(Player player)
        {
            Player.InputProvider = null;

            base.OnPlayerControlLost(player);
        }

        public void SelectTarget(Unit target)
        {
            if (!Player.ExistsIn(World))
                return;

            Player.SetTarget(target);

            TargetSelectionRequestEvent targetSelectionRequest = TargetSelectionRequestEvent.Create(Bolt.GlobalTargets.OnlyServer);
            targetSelectionRequest.TargetId = target?.BoltEntity.NetworkId ?? default;
            targetSelectionRequest.Send();
        }

        public void CastSpell(int spellId)
        {
            if (!Player.ExistsIn(World))
                return;

            if (balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo) && spellInfo.ExplicitTargetType == SpellExplicitTargetType.Destination)
                spellTargeting.SelectSpellDestination(spellInfo);
            else
            {
                SpellCastRequestEvent spellCastRequest = SpellCastRequestEvent.Create(Bolt.GlobalTargets.OnlyServer);
                spellCastRequest.SpellId = spellId;
                spellCastRequest.MovementFlags = (int)Player.MovementInfo.Flags;
                spellCastRequest.Send();
            }
        }

        public void CastSpellWithDestination(int spellId, Vector3 destination)
        {
            if (!Player.ExistsIn(World))
                return;

            SpellCastRequestDestinationEvent spellCastRequest = SpellCastRequestDestinationEvent.Create(Bolt.GlobalTargets.OnlyServer);
            spellCastRequest.SpellId = spellId;
            spellCastRequest.Destination = destination;
            spellCastRequest.MovementFlags = (int)Player.MovementInfo.Flags;
            spellCastRequest.Send();
        }

        public void StopCasting()
        {
            if (!Player.ExistsIn(World))
                return;

            SpellCastCancelRequestEvent.Create(Bolt.GlobalTargets.OnlyServer).Send();
        }

#if UNITY_EDITOR
        [ContextMenu("Validate"), UsedImplicitly]
        private void Validate()
        {
            for (int i = 0; i < hotkeys.Count; i++)
            {
                for (int j = i + 1; j < hotkeys.Count; j++)
                {
                    if (hotkeys[i].HasSameInput(hotkeys[j]))
                        Debug.LogWarning($"{hotkeys[i].name} has the same input as {hotkeys[j].name}, this combination should be properly prioritized!");

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
