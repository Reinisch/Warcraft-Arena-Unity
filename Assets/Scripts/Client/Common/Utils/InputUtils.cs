using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client
{
    public static class InputUtils
    {
        public const int ActionBarSlotCount = 14;
        public const int ActionBarCount = 6;

        public static KeyCode ToKeyCode(this HotkeyModifier hotkeyModifier)
        {
            switch (hotkeyModifier)
            {
                case HotkeyModifier.None:
                    return KeyCode.None;
                case HotkeyModifier.LeftControl:
                    return KeyCode.LeftControl;
                case HotkeyModifier.LeftAlt:
                    return KeyCode.LeftAlt;
                case HotkeyModifier.LeftShift:
                    return KeyCode.LeftShift;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hotkeyModifier));
            }
        }

        public static bool AnyHotkeyModifiersPressedExcept(KeyCode modifier)
        {
            if (modifier != KeyCode.LeftAlt && (Keyboard.current?.leftAltKey.isPressed == true || Keyboard.current?.rightAltKey.isPressed == true))
                return true;
            if (modifier != KeyCode.LeftControl && (Keyboard.current?.leftCtrlKey.isPressed == true || Keyboard.current?.rightCtrlKey.isPressed == true))
                return true;
            if (modifier != KeyCode.LeftShift && (Keyboard.current?.leftShiftKey.isPressed == true || Keyboard.current?.rightShiftKey.isPressed == true))
                return true;

            return false;
        }

        public static bool HasTargetFlag(this TargetingEntityType entityTypes, TargetingEntityType targetingEntityType)
        {
            return (entityTypes & targetingEntityType) == targetingEntityType;
        }
    }
}
