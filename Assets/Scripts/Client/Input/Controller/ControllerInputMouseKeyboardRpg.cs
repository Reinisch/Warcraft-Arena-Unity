using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Controller Input - Mouse Keyboard - Rpg", menuName = "Game Data/Input/Controller/Mouse Keyboard - Rpg")]
    public class ControllerInputMouseKeyboardRpg : ControllerInputSettings
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
            Quaternion expectedRotation = unit.Rotation;

            if (!unit.IsAlive)
                return expectedRotation;

            if (inputReference.RightClickPressed)
                expectedRotation = Quaternion.Euler(0, cameraReference.WarcraftCamera.transform.eulerAngles.y, 0);
            else
            {
                Quaternion turnRotation = Quaternion.Euler(0, inputReference.MoveInput.x * unit.RotationSpeed * Time.unscaledDeltaTime, 0);
                expectedRotation = unit.transform.localRotation * turnRotation;
            }

            return expectedRotation;
        }

        private Vector3 PollMovement(Unit unit)
        {
            Vector3 expectedVelocity;

            if (!unit.IsAlive)
                expectedVelocity = Vector3.zero;
            else 
            {
                expectedVelocity = new Vector3(inputReference.RightClickPressed ? inputReference.MoveInput.x : 0, 0, inputReference.MoveInput.y);

                if (inputReference.LeftClickPressed && inputReference.RightClickPressed && Mathf.Approximately(inputReference.MoveInput.y, 0))
                    expectedVelocity.z = 1;
            }
            return expectedVelocity;
        }
    }
}
