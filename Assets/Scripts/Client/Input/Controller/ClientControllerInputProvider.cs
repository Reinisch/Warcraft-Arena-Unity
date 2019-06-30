using Core;
using UnityEngine;

namespace Client
{
    public class ClientControllerMouseKeyboardInput : IControllerInputProvider
    {
        private readonly Unit unit;
        private readonly CameraReference cameraReference;

        public ClientControllerMouseKeyboardInput(Unit unit, CameraReference cameraReference)
        {
            this.unit = unit;
            this.cameraReference = cameraReference;
        }

        public void PollInput(out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            inputVelocity = PollMovement();
            inputRotation = PollRotation();
            jumping = PollJumping();
            
            Quaternion PollRotation()
            {
                Quaternion expectedRotation = unit.Rotation;

                if (!unit.IsAlive)
                    return expectedRotation;

                if (Input.GetMouseButton(1))
                    expectedRotation = Quaternion.Euler(0, cameraReference.WarcraftCamera.transform.eulerAngles.y, 0);
                else
                {
                    Quaternion turnRotation = Quaternion.Euler(0, Input.GetAxis("Horizontal") * unit.ControllerDefinition.RotateSpeed * Time.unscaledDeltaTime, 0);
                    expectedRotation = unit.transform.localRotation * turnRotation;
                }

                return expectedRotation;
            }

            Vector3 PollMovement()
            {
                Vector3 expectedVelocity;

                if (!unit.IsAlive)
                    expectedVelocity = Vector3.zero;
                else if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                {
                    expectedVelocity = new Vector3(Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0, 0, Input.GetAxis("Vertical"));

                    if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                        expectedVelocity = new Vector3(expectedVelocity.x, expectedVelocity.y, expectedVelocity.z + 1);

                    if (expectedVelocity.z > 1)
                        expectedVelocity.z = 1;

                    expectedVelocity = new Vector3(expectedVelocity.x - Input.GetAxis("Strafing"), expectedVelocity.y, expectedVelocity.z);

                    // if moving forward and to the side at the same time, compensate for distance
                    if (Input.GetMouseButton(1) && !Mathf.Approximately(Input.GetAxis("Horizontal"), 0) && !Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                        expectedVelocity *= 0.7f;
                }
                else
                    expectedVelocity = Vector3.zero;

                return expectedVelocity;
            }

            bool PollJumping()
            {
                return Input.GetButton("Jump");
            }
        }
    }
}
