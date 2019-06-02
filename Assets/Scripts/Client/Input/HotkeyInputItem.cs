using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Hotkey Input Item", menuName = "Player Data/Input/Hotkey Input Item", order = 1)]
    public class HotkeyInputItem : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private string id;
        [SerializeField, UsedImplicitly] private KeyCode key;
        [SerializeField, UsedImplicitly] private HotkeyModifier modifier;

        private KeyCode modifierKeyCode;

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

        public bool HasSameInput(HotkeyInputItem hotkeyItem)
        {
            return hotkeyItem.key == key && hotkeyItem.modifier == modifier;
        }

        public bool IsPressed()
        {
            if (modifierKeyCode != KeyCode.None && !Input.GetKey(modifierKeyCode))
                return false;

            return Input.GetKeyDown(key) && !InputUtils.AnyHotkeyModifiersPressedExcept(modifierKeyCode);
        }
    }
}
