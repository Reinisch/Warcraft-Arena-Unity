using UnityEngine;
using Zenject;

namespace Client
{
    [CreateAssetMenu(fileName = "Camera Mode", menuName = "Player Data/Camera/Modes/Shooter", order = 2)]
    public class WarcraftCameraModeShooter: WarcraftCameraMovementMode
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
            if (input.IsAlternativeMode)
                return;

            yaw += input.LookInput.x * camera.SpeedX;
            pitch -= input.LookInput.y * camera.SpeedY;

            if (!InterfaceUtils.IsPointerOverUI)
                zoom -= input.ZoomInput * deltaTime * camera.ZoomRate * Mathf.Abs(zoom);
        }
    }
}