using System;

namespace Core
{
    [Flags]
    public enum AuraEffectHandleMode
    {
        Normal = 1 << 0,
        SendForClient = 1 << 1,
        ChangeAmount = 1 << 2,
        ReApply = 1 << 3,
    }
}
