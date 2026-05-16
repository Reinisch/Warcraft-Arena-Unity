using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Camera Mode", menuName = "Player Data/Camera/Modes/Shooter", order = 2)]
    public class WarcraftCameraModeShooter: WarcraftCameraMovementMode
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
            if (input.IsAlternativeModeActive)
                return;

            yaw += Input.GetAxis("Mouse X") * camera.SpeedX;
            pitch -= Input.GetAxis("Mouse Y") * camera.SpeedY;

            if (!InterfaceUtils.IsPointerOverUI)
                zoom -= Input.GetAxis("Mouse ScrollWheel") * deltaTime * camera.ZoomRate * Mathf.Abs(zoom);
        }
    }
}