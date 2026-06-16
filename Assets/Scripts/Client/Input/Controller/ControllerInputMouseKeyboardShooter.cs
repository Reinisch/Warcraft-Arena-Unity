using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Controller Input - Mouse Keyboard - Shooter", menuName = "Game Data/Input/Controller/Mouse Keyboard - Shooter")]
    public class ControllerInputMouseKeyboardShooter: ControllerInputSettings
    {
        [Inject] private CameraReference cameraReference;
        [Inject] private InputReference inputReference;

        public override void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            if (inputReference.IsPlayerInputAllowed)
            {
                inputVelocity = PollMovement(unit);
                inputRotation = PollRotation(unit);
                jumping = inputReference.JumpPressed;
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
            return !unit.IsAlive
                ? Vector3.zero
                : new Vector3(inputReference.MoveInput.x, 0, inputReference.MoveInput.y);
        }
    }
}
