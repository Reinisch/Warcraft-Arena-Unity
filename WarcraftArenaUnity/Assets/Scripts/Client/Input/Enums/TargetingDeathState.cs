using System;

namespace Client
{
    [Flags]
    public enum TargetingDeathState
    {
        Dead = 1 << 0,
        Alive = 1 << 1,
    }
}
