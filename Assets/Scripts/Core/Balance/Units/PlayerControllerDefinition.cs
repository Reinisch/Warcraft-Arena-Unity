using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Player Controller Definition", menuName = "Game Data/Player Controller Definiton", order = 3)]
    public class PlayerControllerDefinition : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private float jumpSpeed = 4.0f;
        [SerializeField, UsedImplicitly]
        private float rotateSpeed = 250.0f;
        [SerializeField, UsedImplicitly]
        private float baseGroundCheckDistance = 0.2f;
        [SerializeField, UsedImplicitly]
        private float movementCorrectionDistance = 0.15f;
        [SerializeField, UsedImplicitly]
        private float correctionDampening = 0.8f;

        public float JumpSpeed => jumpSpeed;
        public float RotateSpeed => rotateSpeed;
        public float BaseGroundCheckDistance => baseGroundCheckDistance;
        public float MovementCorrectionDistance => movementCorrectionDistance;
        public float CorrectionDampening => correctionDampening;
    }
}
