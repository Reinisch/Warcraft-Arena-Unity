using Common;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Hotkey Input Item", menuName = "Player Data/Input/Hotkey Input Item", order = 1)]
    public class HotkeyInputItem : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private string id;
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
                if (modifierKeyCode != KeyCode.None && !Input.GetKey(modifierKeyCode))
                    return false;

                return Input.GetKeyDown(key) && !InputUtils.AnyHotkeyModifiersPressedExcept(modifierKeyCode);
            }
        }

        private bool IsHotkeyDown => Input.GetKey(key);

        public KeyCode KeyCode => key;
        public HotkeyModifier Modifier => modifier;

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
            if (appliedKey != key || appliedModifier != modifier)
                Modify(key, modifier);
        }

        public void Register()
        {
            hotkeyState = HotkeyState.Released;
        }

        public void Unregister()
        {
            hotkeyState = HotkeyState.Released;
        }

        public void DoUpdate()
        {
            if (hotkeyState == HotkeyState.Released && IsPressed)
            {
                hotkeyState = HotkeyState.Pressed;
                if (input.IsPlayerInputAllowed)
                    EventHandler.ExecuteEvent(this, GameEvents.HotkeyStateChanged, HotkeyState.Pressed);
            }
            else if (hotkeyState == HotkeyState.Pressed && !IsHotkeyDown)
            {
                hotkeyState = HotkeyState.Released;
                EventHandler.ExecuteEvent(this, GameEvents.HotkeyStateChanged, HotkeyState.Released);
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

            EventHandler.ExecuteEvent(this, GameEvents.HotkeyBindingChanged);
        }
    }
}
