using System;

namespace Core
{
    [Flags]
    public enum AuraAttributes
    {
        Passive = 1 << 0,
        TriggeredCanTriggerProc = 1 << 1,
        CanProcWithTriggered = 1 << 2,
        CantTriggerProc = 1 << 3,
        DisableProc = 1 << 4,
        StackForAnyCasters = 1 << 5,
        DeathPersistent = 1 << 6,
    }
}
