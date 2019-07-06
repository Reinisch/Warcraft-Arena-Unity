using System;

namespace Core
{
    /// <summary>
    /// Compressed to 12 bits in UnitState and MoveState, SpellCastRequestEvent.
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

        MaskMoving = Forward | Backward | StrafeLeft | StrafeRight | Falling | Ascending | Descending,
        MaskTurning = TurnLeft | TurnRight,
    }
}
