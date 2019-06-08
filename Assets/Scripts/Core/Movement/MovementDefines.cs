using System;

namespace Core
{
    public enum MovementGeneratorType
    {
        IdleMotionType = 0,
        RandomMotionType = 1,
        WaypointMotionType = 2,
        ConfusedMotionType = 4,
        ChaseMotionType = 5,
        HomeMotionType = 6,
        FlightMotionType = 7,
        PointMotionType = 8,
        FleeingMotionType = 9,
        DistractMotionType = 10,
        AssistanceMotionType = 11,
        AssistanceDistractMotionType = 12,
        TimedFleeingMotionType = 13,
        FollowMotionType = 14,
        RotateMotionType = 15,
        EffectMotionType = 16,
    }

    /// <summary>
    /// Compressed to 12 bits in UnitState and MoveState.
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