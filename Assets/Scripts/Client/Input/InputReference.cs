using Core;
using Core.Conditions;
using Net;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using UnityInputAciton = UnityEngine.InputSystem.InputAction;

namespace Client
{
    public class InputReference : ScriptableReferenceClient
    {
        [Inject] private BalanceReference balance;
        [Inject] private CameraReference cameraModule;
        [Inject] private TargetingSpellReference spellTargeting;
        [Inject] private InterfaceReference interfaceModule;
        [Inject] private INetworkMessageBus messageBus;
        [Inject] private INetEntityRegistry entityRegistry;

        [SerializeField] private ControllerInputContainer controllerInputs;
        [SerializeField] private HotkeyInputItemContainer hotkeyInputItemContainer;
        [SerializeField] private WarcraftCameraMovementModeContainer cameraModeContainer;
        [SerializeField] private ActionBarSettingsContainer actionBarSettingsContainer;
        [SerializeField] private InputActionContainer inputActionContainer;
        [SerializeField] private InputActionGlobalContainer inputActionGlobalContainer;

        [SerializeField] private List<Condition> inputDisabledWhen;

        private UnityInputAciton altAction;
        private UnityInputAciton moveAction;
        private UnityInputAciton lookAction;
        private UnityInputAciton zoomAction;
        private UnityInputAciton leftClickAction;
        private UnityInputAciton rightClickAction;
        private UnityInputAciton jumpAction;

        public bool IsPlayerInputAllowed { get; private set; }
        public bool IsAlternativeActionActive => altAction != null && altAction.IsPressed();
        public bool IsAlternativeMode => IsAlternativeActionActive || interfaceModule.IsPanelShown<LobbyScreen, LobbyPanel>();

        public Vector2 MoveInput => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public Vector2 LookInput => lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public float ZoomInput => zoomAction?.ReadValue<float>() ?? 0f;
        public bool LeftClickPressed => leftClickAction != null && leftClickAction.IsPressed();
        public bool LeftClickDown => leftClickAction != null && leftClickAction.WasPressedThisFrame();
        public bool RightClickPressed => rightClickAction != null && rightClickAction.IsPressed();
        public bool JumpPressed => jumpAction != null && jumpAction.IsPressed();
        public Vector2 MousePosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            if (InputSystem.actions != null)
            {
                altAction = InputSystem.actions.FindAction("ModifierAlt");
                moveAction = InputSystem.actions.FindAction("Move");
                lookAction = InputSystem.actions.FindAction("Look");
                zoomAction = InputSystem.actions.FindAction("Zoom");
                leftClickAction = InputSystem.actions.FindAction("LeftClick");
                rightClickAction = InputSystem.actions.FindAction("RightClick");
                jumpAction = InputSystem.actions.FindAction("Jump");
            }

            hotkeyInputItemContainer.Register();
            actionBarSettingsContainer.Register();
            controllerInputs.Register();
            cameraModeContainer.Register();
            inputActionContainer.Register();
            inputActionGlobalContainer.Register();
        }

        protected override void OnUnregister()
        {
            inputActionGlobalContainer.Unregister();
            inputActionContainer.Unregister();
            cameraModeContainer.Unregister();
            controllerInputs.Unregister();
            actionBarSettingsContainer.Unregister();
            hotkeyInputItemContainer.Unregister();

            base.OnUnregister();
        }

        protected override void OnUpdate(float deltaTime)
        {
            bool anyDisabled = false;
            for (int i = 0; i < inputDisabledWhen.Count; i++)
            {
                if (inputDisabledWhen[i].IsApplicableAndValid())
                {
                    anyDisabled = true;
                    break;
                }
            }
            IsPlayerInputAllowed = !anyDisabled;

            IReadOnlyList<HotkeyInputItem> hotkeys = hotkeyInputItemContainer.ItemList;
            for (int i = 0; i < hotkeys.Count; i++)
                hotkeys[i].DoUpdate();

            bool showCursor = Player is null or { MovementMode: MovementMode.Rpg }
                || IsAlternativeMode;
            Cursor.visible = showCursor;
            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        }

        protected override void QueueForInject(DiContainer container)
        {
            inputActionContainer.QueueForInject(container);
            inputActionGlobalContainer.QueueForInject(container);
            hotkeyInputItemContainer.QueueForInject(container);
            actionBarSettingsContainer.QueueForInject(container);
            cameraModeContainer.QueueForInject(container);
        }

        public override void OnControlStateChanged(bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(true);

                Player.InputProvider = controllerInputs.Player;
            }
            else
            {
                Player.InputProvider = null;

                base.OnControlStateChanged(false);
            }
        }

        // NOTE: Player movement stays local/direct by design. All other player commands below are routed
        // through the network message bus instead of calling Core directly.
        public void SelectTarget(Unit target)
        {
            if (!Player.ExistsIn(World))
                return;

            // Optimistic: set our own target locally for instant feedback (target frame / selection circle)
            // instead of waiting for the server round-trip — UpdateTarget only sets the reference + fires
            // UnitTargetChanged, no server-authoritative side effects. The server still resolves + applies it
            // to the connection player (for casting + so others can see it) via the request below.
            Player.SetTarget(target);
            messageBus.Send(new TargetSelectionRequest(entityRegistry.GetId(target)), NetTarget.Server);
        }

        public void DoEmote(EmoteType emoteType)
        {
            if (!Player.ExistsIn(World))
                return;

            messageBus.Send(new PlayerEmoteRequest(emoteType), NetTarget.Server);
        }

        public void SwitchClass(ClassType classType)
        {
            if (!Player.ExistsIn(World))
                return;

            // Optimistic: switch locally for instant action-bar/power feedback (HandleClassChange is
            // client-safe — the spellbook build is server-gated). The server applies it to the connection
            // player and the result replicates back via vitals (idempotent, so no flicker); if the server
            // ever disagrees, its replicated class corrects us.
            Player.SwitchClass(classType);
            messageBus.Send(new PlayerClassChangeRequest(classType), NetTarget.Server);
        }

        public void Say(string message)
        {
            if (!Player.ExistsIn(World))
                return;

            messageBus.Send(new PlayerChatRequest(message), NetTarget.Server);
        }

        public void CastSpell(int spellId)
        {
            if (!Player.ExistsIn(World))
                return;

            if (balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo) && spellInfo.ExplicitTargetType == SpellExplicitTargetType.Destination)
                spellTargeting.SelectSpellTargetDestination(spellInfo);
            else
                messageBus.Send(new SpellCastRequest(spellId, Player.MovementFlags), NetTarget.Server);
        }

        public void CastSpellWithDestination(int spellId, Vector3 destination)
        {
            if (!Player.ExistsIn(World))
                return;

            if (!balance.SpellInfosById.ContainsKey(spellId))
                return;

            messageBus.Send(new SpellCastDestinationRequest(spellId, Player.MovementFlags, destination), NetTarget.Server);
        }

        public void CastSpellWithTargetingOptions(int spellId)
        {
            if (!Player.ExistsIn(World))
                return;

            if (!balance.SpellInfosById.ContainsKey(spellId))
                return;

            if (cameraModule.WarcraftCamera.Target != Player)
                return;

            messageBus.Send(
                new SpellCastTargetingRequest(spellId, cameraModule.WarcraftCamera.transform.position,
                    cameraModule.WarcraftCamera.transform.rotation),
                NetTarget.Server);
        }

        public void StopCasting()
        {
            if (!Player.ExistsIn(World))
                return;

            messageBus.Send(new SpellCastCancelRequest(), NetTarget.Server);
        }

#if UNITY_EDITOR
        [ContextMenu("Validate"), UsedImplicitly]
        private void Validate()
        {
            for (int i = 0; i < hotkeyInputItemContainer.ItemList.Count; i++)
            {
                for (int j = i + 1; j < hotkeyInputItemContainer.ItemList.Count; j++)
                {
                    if (hotkeyInputItemContainer.ItemList[i].HasSameInput(hotkeyInputItemContainer.ItemList[j]))
                        Debug.LogWarning($"{hotkeyInputItemContainer.ItemList[i].name} has the same input as " +
                            $"{hotkeyInputItemContainer.ItemList[j].name}, this combination should be properly prioritized!");
                }
            }
        }
#endif
    }
}
