using UnityEngine;

namespace Client
{
    public abstract class WarcraftCameraMovementMode: ScriptableObject
    {
        public abstract void PollInput(
            WarcraftCamera camera,
            float deltaTime,
            ref float zoom,
            ref float yaw,
            ref float pitch);
    }
}
