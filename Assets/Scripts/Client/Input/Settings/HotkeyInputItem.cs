using Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Zenject;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Hotkey Input Item", menuName = "Player Data/Input/Hotkey Input Item", order = 1)]
    public class HotkeyInputItem : ScriptableUniqueInfo<HotkeyInputItem>
    {
        [Inject] private InputReference input;
        [Inject] private EventBus eventBus;

        [SerializeField, UsedImplicitly] private KeyCode key;
        [SerializeField, UsedImplicitly] private HotkeyModifier modifier;

        private KeyCode modifierKeyCode;
        private HotkeyState hotkeyState;

        private KeyCode appliedKey;
        private HotkeyModifier appliedModifier;

        private bool IsPressed
        {
            get
            {
                if (modifierKeyCode != KeyCode.None && !IsModifierDown(modifierKeyCode))
                    return false;

                return IsKeyDownThisFrame(key) && !InputUtils.AnyHotkeyModifiersPressedExcept(modifierKeyCode);
            }
        }

        private bool IsHotkeyDown => IsKeyDown(key);

        public KeyCode KeyCode => key;
public HotkeyModifier Modifier => modifier;

        private bool IsModifierDown(KeyCode modifier)
        {
            if (Keyboard.current == null) return false;
            switch (modifier)
            {
                case KeyCode.LeftAlt: return Keyboard.current.leftAltKey.isPressed || Keyboard.current.rightAltKey.isPressed;
                case KeyCode.LeftControl: return Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
                case KeyCode.LeftShift: return Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
                default: return false;
            }
        }

        private bool IsKeyDown(KeyCode keyCode)
        {
            var keyControl = GetKeyControl(keyCode);
            return keyControl != null && keyControl.isPressed;
        }

        private bool IsKeyDownThisFrame(KeyCode keyCode)
        {
            var keyControl = GetKeyControl(keyCode);
            return keyControl != null && keyControl.wasPressedThisFrame;
        }

        private KeyControl GetKeyControl(KeyCode keyCode)
        {
            if (Keyboard.current == null) return null;

            switch (keyCode)
            {
                case KeyCode.None: return null;
                case KeyCode.Alpha0: return Keyboard.current.digit0Key;
                case KeyCode.Alpha1: return Keyboard.current.digit1Key;
                case KeyCode.Alpha2: return Keyboard.current.digit2Key;
                case KeyCode.Alpha3: return Keyboard.current.digit3Key;
                case KeyCode.Alpha4: return Keyboard.current.digit4Key;
                case KeyCode.Alpha5: return Keyboard.current.digit5Key;
                case KeyCode.Alpha6: return Keyboard.current.digit6Key;
                case KeyCode.Alpha7: return Keyboard.current.digit7Key;
                case KeyCode.Alpha8: return Keyboard.current.digit8Key;
                case KeyCode.Alpha9: return Keyboard.current.digit9Key;
                case KeyCode.Q: return Keyboard.current.qKey;
                case KeyCode.W: return Keyboard.current.wKey;
                case KeyCode.E: return Keyboard.current.eKey;
                case KeyCode.R: return Keyboard.current.rKey;
                case KeyCode.T: return Keyboard.current.tKey;
                case KeyCode.Y: return Keyboard.current.yKey;
                case KeyCode.U: return Keyboard.current.uKey;
                case KeyCode.I: return Keyboard.current.iKey;
                case KeyCode.O: return Keyboard.current.oKey;
                case KeyCode.P: return Keyboard.current.pKey;
                case KeyCode.A: return Keyboard.current.aKey;
                case KeyCode.S: return Keyboard.current.sKey;
                case KeyCode.D: return Keyboard.current.dKey;
                case KeyCode.F: return Keyboard.current.fKey;
                case KeyCode.G: return Keyboard.current.gKey;
                case KeyCode.H: return Keyboard.current.hKey;
                case KeyCode.J: return Keyboard.current.jKey;
                case KeyCode.K: return Keyboard.current.kKey;
                case KeyCode.L: return Keyboard.current.lKey;
                case KeyCode.Z: return Keyboard.current.zKey;
                case KeyCode.X: return Keyboard.current.xKey;
                case KeyCode.C: return Keyboard.current.cKey;
                case KeyCode.V: return Keyboard.current.vKey;
                case KeyCode.B: return Keyboard.current.bKey;
                case KeyCode.N: return Keyboard.current.nKey;
                case KeyCode.M: return Keyboard.current.mKey;
                case KeyCode.Escape: return Keyboard.current.escapeKey;
                case KeyCode.Tab: return Keyboard.current.tabKey;
                case KeyCode.Space: return Keyboard.current.spaceKey;
                case KeyCode.Return: return Keyboard.current.enterKey;
                case KeyCode.Backspace: return Keyboard.current.backspaceKey;
                case KeyCode.F1: return Keyboard.current.f1Key;
                case KeyCode.F2: return Keyboard.current.f2Key;
                case KeyCode.F3: return Keyboard.current.f3Key;
                case KeyCode.F4: return Keyboard.current.f4Key;
                case KeyCode.F5: return Keyboard.current.f5Key;
                case KeyCode.F6: return Keyboard.current.f6Key;
                case KeyCode.F7: return Keyboard.current.f7Key;
                case KeyCode.F8: return Keyboard.current.f8Key;
                case KeyCode.F9: return Keyboard.current.f9Key;
                case KeyCode.F10: return Keyboard.current.f10Key;
                case KeyCode.F11: return Keyboard.current.f11Key;
                case KeyCode.F12: return Keyboard.current.f12Key;
                default:
                    // Fallback for other keys - this is not perfect but covers most used
                    return Keyboard.current[keyCode.ToString()] as KeyControl;
            }
        }

        [UsedImplicitly]
        private void Awake()
        {
            modifierKeyCode = modifier.ToKeyCode();
            appliedKey = key;
            appliedModifier = modifier;
        }

        [UsedImplicitly]
        private void OnValidate()
        {
            modifierKeyCode = modifier.ToKeyCode();
            if (Application.isPlaying && eventBus != null && (appliedKey != key || appliedModifier != modifier))
                Modify(key, modifier);
        }

        protected override void OnRegister()
        {
            hotkeyState = HotkeyState.Released;
        }

        protected override void OnUnregister()
        {
            hotkeyState = HotkeyState.Released;
        }

        public void DoUpdate()
        {
            if (hotkeyState == HotkeyState.Released && IsPressed)
            {
                hotkeyState = HotkeyState.Pressed;
                if (input.IsPlayerInputAllowed)
                    eventBus.ExecuteEvent(this, GameEvents.HotkeyStateChanged, HotkeyState.Pressed);
            }
            else if (hotkeyState == HotkeyState.Pressed && !IsHotkeyDown)
            {
                hotkeyState = HotkeyState.Released;
                eventBus.ExecuteEvent(this, GameEvents.HotkeyStateChanged, HotkeyState.Released);
            }
        }

        public bool HasSameInput(HotkeyInputItem hotkeyItem)
        {
            return hotkeyItem.key == key && hotkeyItem.modifier == modifier;
        }

        public void Modify(KeyCode keyCode, HotkeyModifier modifier)
        {
            appliedKey = keyCode;
            appliedModifier = modifier;

            eventBus.ExecuteEvent(this, GameEvents.HotkeyBindingChanged);
        }
    }
}
