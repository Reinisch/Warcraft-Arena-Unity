using System;
using UnityEngine;

namespace Client
{
    public static class InputUtils
    {
        private static readonly KeyCode[] HotkeyModifiers = {KeyCode.LeftAlt, KeyCode.LeftControl, KeyCode.LeftShift};

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
            for (int i = 0; i < HotkeyModifiers.Length; i++)
                if (HotkeyModifiers[i] != modifier && Input.GetKey(HotkeyModifiers[i]))
                    return true;

            return false;
        }

        public static bool HasTargetFlag(this TargetingEntityType entityTypes, TargetingEntityType targetingEntityType)
        {
            return (entityTypes & targetingEntityType) == targetingEntityType;
        }
    }
}
