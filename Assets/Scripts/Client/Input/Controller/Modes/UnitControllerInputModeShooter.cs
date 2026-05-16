using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Player Unit Input Mode - Shooter", menuName = "Player Data/Input/Unit/Modes/Shooter", order = 1)]
    public class UnitControllerInputModeShooter: UnitControllerInputMode
    {
        [SerializeField, UsedImplicitly] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private InputReference inputReference;

        public override void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            if (inputReference.IsPlayerInputAllowed)
            {
                inputVelocity = PollMovement(unit);
                inputRotation = PollRotation(unit);
                jumping = Input.GetButton("Jump");
            }
            else
            {
                inputVelocity = Vector3.zero;
                inputRotation = unit.Rotation;
                jumping = false;
            }
        }

        private Quaternion PollRotation(Unit unit)
        {
            return unit.IsAlive
                ? Quaternion.Euler(0, cameraReference.WarcraftCamera.transform.eulerAngles.y, 0)
                : unit.Rotation;
        }

        private Vector3 PollMovement(Unit unit)
        {
            return !unit.IsAlive || unit.HasMovementFlag(MovementFlags.Flying)
                ? Vector3.zero
                : new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }
    }
}
