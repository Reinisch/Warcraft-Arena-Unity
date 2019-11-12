using System;

namespace Core
{
    [Flags]
    public enum AuraEffectHandleMode
    {
        Normal = 1 << 0,
        UpdateStacks = 1 << 2,
        Refresh = 1 << 3,
    }
}
