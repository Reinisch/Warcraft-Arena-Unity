using System;

namespace Core
{
    [Flags]
    public enum AuraFlags
    {
        NoCaster = 1 << 0,
        Positive = 1 << 1,
        Negative = 1 << 2,
        Duration = 1 << 3,
        Scalable = 1 << 4
    }
}
