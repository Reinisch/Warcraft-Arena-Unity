using System;
using JetBrains.Annotations;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class HotKeyModifierLink
        {
            [UsedImplicitly] public HotkeyModifier Modifier;
            [UsedImplicitly] public string String;
        }
    }
}