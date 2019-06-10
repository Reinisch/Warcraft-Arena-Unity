using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Unit Movement Definition", menuName = "Game Data/Unit Movement Definition", order = 4)]
    public class UnitMovementDefinition : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private float walkSpeed = 3.0f;
        [SerializeField, UsedImplicitly]
        private float runSpeed = 7.0f;
        [SerializeField, UsedImplicitly]
        private float runBackSpeed = 4.5f;
        [SerializeField, UsedImplicitly]
        private float turnRate = 250.0f;
        [SerializeField, UsedImplicitly]
        private float pitchRate = 250.0f;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float RunBackSpeed => runBackSpeed;
        public float TurnRate => turnRate;

        public float BaseSpeedByType(UnitMoveType moveType)
        {
            switch (moveType)
            {
                case UnitMoveType.Walk:
                    return walkSpeed;
                case UnitMoveType.Run:
                    return runSpeed;
                case UnitMoveType.RunBack:
                    return runBackSpeed;
                case UnitMoveType.TurnRate:
                    return turnRate;
                case UnitMoveType.PitchRate:
                    return pitchRate;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moveType), moveType, "Unknown movement type!");
            }
        }
    }
}
