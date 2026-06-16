using System;

namespace Core
{
    /// <summary>
    /// Compressed to 13 bits in <seealso cref="UnitState"/> and <seealso cref="MoveState"/>, <seealso cref="SpellCastRequestEvent"/>, <seealso cref="PlayerMovementControlChanged"/>.
    /// </summary>
    [Flags]
    public enum MovementFlags
    {
        Root = 1 << 0,
        Forward = 1 << 1,
        Backward = 1 << 2,
        StrafeLeft = 1 << 3,
        StrafeRight = 1 << 4,
        TurnLeft = 1 << 5,
        TurnRight = 1 << 6,
        Walking = 1 << 7,
        Falling = 1 << 8,
        Ascending = 1 << 9,
        Descending = 1 << 10,
        Flying = 1 << 11,
        Charging = 1 << 12,

        MaskMoving = Forward | Backward | StrafeLeft | StrafeRight | Falling | Ascending | Descending | Charging,
        MaskAir = Falling | Ascending | Descending | Flying,
        MaskTurning = TurnLeft | TurnRight,
        // Movement that interrupts a cast: deliberate locomotion only. Excludes Ascending/Descending/Falling,
        // which flicker from KCC grounding probes/gravity even while standing (false interrupts). Flying IS
        // included — it's set only on an actual jump, so a jump cleanly breaks the cast as it should.
        MaskCastInterrupt = Forward | Backward | StrafeLeft | StrafeRight | Charging | Flying,
    }
}
