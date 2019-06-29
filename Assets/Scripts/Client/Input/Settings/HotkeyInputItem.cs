using Common;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Hotkey Input Item", menuName = "Player Data/Input/Hotkey Input Item", order = 1)]
    public class HotkeyInputItem : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private string id;
        [SerializeField, UsedImplicitly] private KeyCode key;
        [SerializeField, UsedImplicitly] private HotkeyModifier modifier;

        private KeyCode modifierKeyCode;
        private HotkeyState hotkeyState;

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

        [UsedImplicitly]
        private void Awake()
        {
            modifierKeyCode = modifier.ToKeyCode();
        }

        [UsedImplicitly]
        private void OnValidate()
        {
            modifierKeyCode = modifier.ToKeyCode();
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
    }
}
