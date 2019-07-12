using System;

namespace Core
{
    [Flags]
    public enum AuraTargetingMode
    {
        Single = 1 << 0,
        AreaFriend = 1 << 2,
        AreaEnemy = 1 << 3,
    }
}
