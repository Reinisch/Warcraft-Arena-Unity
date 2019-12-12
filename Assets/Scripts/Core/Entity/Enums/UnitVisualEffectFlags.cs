using System;

namespace Core
{
    /// <summary>
    /// Compressed in <seealso cref="IUnitState"/> to 4 bits.
    /// </summary>
    [Flags]
    public enum UnitVisualEffectFlags
    {
        StealthTransparency = 1 << 0,
        InvisibilityTransparency = 1 << 1,

        AnyTransparency = StealthTransparency | InvisibilityTransparency
    }
}