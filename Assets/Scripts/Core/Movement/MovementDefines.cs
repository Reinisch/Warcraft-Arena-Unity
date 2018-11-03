using System;

namespace Core
{
    public enum MovementGeneratorType
    {
        IdleMotionType = 0,                              // IdleMovementGenerator.h
        RandomMotionType = 1,                              // RandomMovementGenerator.h
        WaypointMotionType = 2,                              // WaypointMovementGenerator.h
        MaxDbMotionType = 3,                              // *** this and below motion types can't be set in DB.
        AnimalRandomMotionType = MaxDbMotionType,         // AnimalRandomMovementGenerator.h
        ConfusedMotionType = 4,                              // ConfusedMovementGenerator.h
        ChaseMotionType = 5,                              // TargetedMovementGenerator.h
        HomeMotionType = 6,                              // HomeMovementGenerator.h
        FlightMotionType = 7,                              // WaypointMovementGenerator.h
        PointMotionType = 8,                              // PointMovementGenerator.h
        FleeingMotionType = 9,                              // FleeingMovementGenerator.h
        DistractMotionType = 10,                             // IdleMovementGenerator.h
        AssistanceMotionType = 11,                             // PointMovementGenerator.h (first part of flee for assistance)
        AssistanceDistractMotionType = 12,                   // IdleMovementGenerator.h (second part of flee for assistance)
        TimedFleeingMotionType = 13,                         // FleeingMovementGenerator.h (alt.second part of flee for assistance)
        FollowMotionType = 14,
        RotateMotionType = 15,
        EffectMotionType = 16,
        NullMotionType = 17
    }

    public enum MovementSlot
    {
        Idle,
        Active,
        Controlled,
    }

    [Flags]
    public enum MotionCleanFlag
    {
        None = 0,
        Update = 1, // Clear or Expire called from update
        Reset = 2   // Flag if need top()->Reset()
    }

    public enum RotateDirection
    {
        Left,
        Right
    }

    [Flags]
    public enum MovementFlags
    {
        None = 0x00000000,
        Forward = 0x00000001,
        Backward = 0x00000002,
        StrafeLeft = 0x00000004,
        StrafeRight = 0x00000008,
        Left = 0x00000010,
        Right = 0x00000020,
        PitchUp = 0x00000040,
        PitchDown = 0x00000080,
        Walking = 0x00000100,                       // walking
        DisableGravity = 0x00000200,                // former LEVITATING. This is used when walking is not possible.
        Root = 0x00000400,                          // must not be set along with MASK_MOVING
        Falling = 0x00000800,                       // damage dealt on that type of falling
        FallingFar = 0x00001000,
        PendingStop = 0x00002000,
        PendingStrafeStop = 0x00004000,
        PendingForward = 0x00008000,
        PendingBackward = 0x00010000,
        PendingStrafeLeft = 0x00020000,
        PendingStrafeRight = 0x00040000,
        PendingRoot = 0x00080000,
        Swimming = 0x00100000,                      // appears with fly flag also
        Ascending = 0x00200000,                     // press "space" when flying
        Descending = 0x00400000,
        CanFly = 0x00800000,                        // Appears when unit can fly AND also walk
        Flying = 0x01000000,                        // unit is actually flying. pretty sure this is only used for players. creatures use disable_gravity
        SplineElevation = 0x02000000,               // used for flight paths
        Waterwalking = 0x04000000,                  // prevent unit from falling through water
        FallingSlow = 0x08000000,                   // active rogue safe fall spell (passive)
        Hover = 0x10000000,                         // hover, cannot jump
        DisableCollision = 0x20000000,

        MaskMoving = Forward | Backward | StrafeLeft | StrafeRight | PitchUp |
                     PitchDown | Falling | FallingFar | Ascending | Descending | SplineElevation,

        MaskTurning = Left | Right,

        MaskMovingFly = Flying | Ascending | Descending,

        /// Movement flags allowed for creature in CreateObject - we need to keep all other enabled serverside
        MaskCreatureAllowed = Forward | DisableGravity | Root | Swimming |
                              CanFly | Waterwalking | FallingSlow | Hover | DisableCollision,

        MaskPlayerOnly = Flying,

        /// Movement flags that have change status opcodes associated for players
        MaskHasPlayerStatusOpcode = DisableGravity | Root | CanFly |
                                    Waterwalking | FallingSlow | Hover | DisableCollision
    }

    [Flags]
    public enum MovementExtraFlags
    {
        None = 0x00000000,
        NoStrafe = 0x00000001,
        NoJumping = 0x00000002,
        FullSpeedTurning = 0x00000004,
        FullSpeedPitching = 0x00000008,
        AlwaysAllowPitching = 0x00000010,
        CanSwimToFlyTrans = 0x00000400,
        CanTurnWhileFalling = 0x00001000,
        InterpolatedMovement = 0x00002000,
        InterpolatedTurning = 0x00004000,
        InterpolatedPitching = 0x00008000,
        DoubleJump = 0x00010000
    }

    public enum MonsterMoveType
    {
        Normal = 0,
        FacingSpot = 1,
        FacingTarget = 2,
        FacingAngle = 3
    }
}