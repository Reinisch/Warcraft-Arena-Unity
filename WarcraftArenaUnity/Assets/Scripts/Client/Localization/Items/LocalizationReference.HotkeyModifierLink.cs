using System;

namespace Client
{
    public partial class LocalizationReference
    {
        [Serializable]
        private class HotKeyModifierLink
        {
            public HotkeyModifier Modifier;
            public string String;
        }
    }
}