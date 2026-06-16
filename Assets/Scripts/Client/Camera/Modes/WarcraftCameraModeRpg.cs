using UnityEngine;
using Zenject;

namespace Client
{
    [CreateAssetMenu(fileName = "Camera Mode", menuName = "Player Data/Camera/Modes/Rpg", order = 1)]
    public class WarcraftCameraModeRpg: WarcraftCameraMovementMode
    {
        [Inject]
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
                if ((input.LeftClickPressed && !InterfaceUtils.IsPointerOverUI) || input.RightClickPressed)
                {
                    yaw += input.LookInput.x * camera.SpeedX;
                    pitch -= input.LookInput.y * camera.SpeedY;
                }
                // otherwise, ease behind the target if any of the directional keys are pressed
                else if (!Mathf.Approximately(input.MoveInput.y, 0) || !Mathf.Approximately(input.MoveInput.x, 0))
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
                zoom -= input.ZoomInput * deltaTime * camera.ZoomRate * Mathf.Abs(zoom);
        }
    }
}