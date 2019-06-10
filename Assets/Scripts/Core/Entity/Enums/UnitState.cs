using System;

namespace Core
{
    [Flags]
    public enum UnitState
    {
        None = 0x00000000,
        Died = 0x00000001,
        MeleeAttacking = 0x00000002,
        Stunned = 0x00000008,
        Roaming = 0x00000010,
        Chase = 0x00000020,
        Fleeing = 0x00000080,
        InFlight = 0x00000100,
        Follow = 0x00000200,
        Root = 0x00000400,
        Confused = 0x00000800,
        Distracted = 0x00001000,
        Isolated = 0x00002000,
        AttackPlayer = 0x00004000,
        Casting = 0x00008000,
        Possessed = 0x00010000,
        Charging = 0x00020000,
        Jumping = 0x00040000,
        Move = 0x00100000,
        Rotating = 0x00200000,
        Evade = 0x00400000,
        RoamingMove = 0x00800000,
        ConfusedMove = 0x01000000,
        FleeingMove = 0x02000000,
        ChaseMove = 0x04000000,
        FollowMove = 0x08000000,

        Moving = RoamingMove | ConfusedMove | FleeingMove | ChaseMove | FollowMove,
        Controlled = Confused | Stunned | Fleeing,
        LostControl = Controlled | Jumping | Charging,
        CantMove = Root | Stunned | Died | Distracted
    }
}
