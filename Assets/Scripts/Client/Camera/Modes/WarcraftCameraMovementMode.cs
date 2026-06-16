using Common;
using UnityEngine;

namespace Client
{
    public abstract class WarcraftCameraMovementMode : ScriptableUniqueInfo<WarcraftCameraMovementMode>
    {
        public abstract void PollInput(
            WarcraftCamera camera,
            float deltaTime,
            ref float zoom,
            ref float yaw,
            ref float pitch);
    }
}
