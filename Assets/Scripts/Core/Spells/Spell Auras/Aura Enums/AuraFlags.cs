using System;

namespace Core
{
    [Flags]
    public enum AuraFlags
    {
        None = 0x00,
        Nocaster = 0x01,
        Positive = 0x02,
        Duration = 0x04,
        Scalable = 0x08,
        Negative = 0x10,
    }
}
