using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Camera Mode", menuName = "Player Data/Camera/Modes/Rpg", order = 1)]
    public class WarcraftCameraModeRpg: WarcraftCameraMovementMode
    {
        [SerializeField]
        private InputReference input;

        public override void PollInput(
            WarcraftCamera camera,
            float deltaTime,
            ref float zoom,
            ref float yaw,
            ref float pitch)
        {
            // If either mouse buttons are down, let the mouse govern camera position
            if (GUIUtility.hotControl == 0)
            {
                if (Input.GetMouseButton(0) && !InterfaceUtils.IsPointerOverUI || Input.GetMouseButton(1))
                {
                    yaw += Input.GetAxis("Mouse X") * camera.SpeedX;
                    pitch -= Input.GetAxis("Mouse Y") * camera.SpeedY;
                }
                // otherwise, ease behind the target if any of the directional keys are pressed
                else if (!Mathf.Approximately(Input.GetAxis("Vertical"), 0) || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0))
                {
                    if (camera.Target.IsAlive && input.IsPlayerInputAllowed)
                    {
                        float targetRotationAngle = camera.Target.transform.eulerAngles.y;
                        float currentRotationAngle = camera.transform.eulerAngles.y;
                        yaw = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, camera.RotationDampening * deltaTime);
                    }
                }
            }

            if (!InterfaceUtils.IsPointerOverUI)
                zoom -= Input.GetAxis("Mouse ScrollWheel") * deltaTime * camera.ZoomRate * Mathf.Abs(zoom);
        }
    }
}