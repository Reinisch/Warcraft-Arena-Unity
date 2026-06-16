using System;
using UnityEngine.AI;

namespace Core
{
    public static class MovementUtils
    {
        public const int SpellMovementInterruptThreshold = 200;

        public const float GridCellSwitchDifference = 1.0f;
        public const float MaxHeight = 100.0f;
        public const float MinHeight = -100.0f;

        public const float Gravity = 19.29110527038574f;
        public const float TerminalVelocity = 60.148003f;
        public const float TerminalSafefallVelocity = 7.0f;

        public const float TerminalLength = TerminalVelocity * TerminalVelocity / (2.0f * Gravity);
        public const float TerminalSafeFallLength = TerminalSafefallVelocity * TerminalSafefallVelocity / (2.0f * Gravity);
        public const float TerminalFallTime = TerminalVelocity / Gravity;
        public const float TerminalSafeFallFallTime = TerminalSafefallVelocity / Gravity;

        public const int MaxConfusedPath = 30;
        public const int MaxNavMeshSampleRange = 30;
        public const int MaxChargeSampleRange = 5;
        public const int MaxPointPathLength = 120;
        public const int PointArrivalRange = 2;

        public const float ChargeRotationSpeed = 360;
        public const float MoveRotationSpeed = 180;

        public static int WalkableAreaMask { get; private set; }

        public const float DirectionalMovementThreshold = 0.01f;

        public static readonly MovementSlot[] MovementSlots = (MovementSlot[])Enum.GetValues(typeof(MovementSlot));

        public static bool IsMoving(this MovementFlags movementFlags) => (movementFlags & MovementFlags.MaskMoving) != 0;

        /// <summary>Whether this movement state should interrupt an in-progress cast — deliberate locomotion or a
        /// jump, ignoring the noisy grounding/gravity air flags (see <see cref="MovementFlags.MaskCastInterrupt"/>).</summary>
        public static bool InterruptsCast(this MovementFlags movementFlags) => (movementFlags & MovementFlags.MaskCastInterrupt) != 0;

        internal static void Initialize()
        {
            WalkableAreaMask = 1 << NavMesh.GetAreaFromName("Walkable");
        }
    }
}