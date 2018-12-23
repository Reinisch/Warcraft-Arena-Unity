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

    [Flags]
    public enum MovementFlags
    {
        None = 1 << 0,
        Forward = 1 << 1,
        Backward = 1 << 2,
        StrafeLeft = 1 << 3,
        StrafeRight = 1 << 4,
        Left = 1 << 5,
        Right = 1 << 6,
        PitchUp = 1 << 7,
        PitchDown = 1 << 8,
        Walking = 1 << 9,
        DisableGravity = 1 << 10,
        Root = 1 << 11,
        Falling = 1 << 12,
        FallingFar = 1 << 13,
        PendingStop = 1 << 14,
        PendingStrafeStop = 1 << 15,
        PendingForward = 1 << 16,
        PendingBackward = 1 << 17,
        PendingStrafeLeft = 1 << 18,
        PendingStrafeRight = 1 << 19,
        PendingRoot = 1 << 20,
        Swimming = 1 << 21,
        Ascending = 1 << 22,
        Descending = 1 << 23,
        CanFly = 1 << 24,
        Flying = 1 << 25,
        SplineElevation = 1 << 26,
        Waterwalking = 1 << 27,
        FallingSlow = 1 << 28,
        Hover = 1 << 29,
        DisableCollision = 1 << 30,

        MaskMoving = Forward | Backward | StrafeLeft | StrafeRight | PitchUp | PitchDown | Falling | FallingFar | Ascending | Descending | SplineElevation,
        MaskTurning = Left | Right,
        MaskMovingFly = Flying | Ascending | Descending,
    }
}