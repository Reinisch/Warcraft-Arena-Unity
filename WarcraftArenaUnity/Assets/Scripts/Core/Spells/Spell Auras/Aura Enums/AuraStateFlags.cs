using System;

namespace Core
{
    [Flags]
    public enum AuraStateFlags
    {
        Frozen = 1 << 0,
        Defense = 1 << 1,
        Berserking = 1 << 2,
        Judgement = 1 << 3,
        Conflagrate = 1 << 4,
        Swiftmend = 1 << 5,
        DeadlyPoison = 1 << 6,
        Enrage = 1 << 7,
        Bleeding = 1 << 8
    }
}
