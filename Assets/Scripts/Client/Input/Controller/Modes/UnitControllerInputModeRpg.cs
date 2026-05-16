using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Player Unit Input Mode - Rpg", menuName = "Player Data/Input/Unit/Modes/Rpg", order = 1)]
    public class UnitControllerInputModeRpg : UnitControllerInputMode
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
            Quaternion expectedRotation = unit.Rotation;

            if (!unit.IsAlive)
                return expectedRotation;

            if (Input.GetMouseButton(1))
                expectedRotation = Quaternion.Euler(0, cameraReference.WarcraftCamera.transform.eulerAngles.y, 0);
            else
            {
                Quaternion turnRotation = Quaternion.Euler(0, Input.GetAxis("Horizontal") * unit.RotationSpeed * Time.unscaledDeltaTime, 0);
                expectedRotation = unit.transform.localRotation * turnRotation;
            }

            return expectedRotation;
        }

        private Vector3 PollMovement(Unit unit)
        {
            Vector3 expectedVelocity;

            if (!unit.IsAlive)
                expectedVelocity = Vector3.zero;
            else if (!unit.HasMovementFlag(MovementFlags.Flying))
            {
                expectedVelocity = new Vector3(Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0, 0, Input.GetAxis("Vertical"));

                if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                    expectedVelocity.z = 1;
            }
            else
                expectedVelocity = Vector3.zero;

            return expectedVelocity;
        }
    }
}
