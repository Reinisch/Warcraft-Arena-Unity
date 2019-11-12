using System;

namespace Core
{
    [Flags]
    public enum UnitControlState
    {
        Died = 1 << 0,
        Stunned = 1 << 1,
        Fleeing = 1 << 2,
        InFlight = 1 << 3,
        Root = 1 << 4,
        Confused = 1 << 5,
        Distracted = 1 << 6,
        Isolated = 1 << 7,
        Casting = 1 << 8,
        Charging = 1 << 9,
        Jumping = 1 << 10,
        Move = 1 << 11,
        Rotating = 1 << 12,
        RoamingMove = 1 << 13,
        ConfusedMove = 1 << 14,
        FleeingMove = 1 << 15,
        ChaseMove = 1 << 16,
        FollowMove = 1 << 17,

        Moving = RoamingMove | ConfusedMove | FleeingMove | ChaseMove | FollowMove,
        Controlled = Confused | Stunned | Fleeing,
        LostControl = Controlled | Jumping | Charging,
        CantMove = Root | Stunned | Died | Distracted
    }
}
