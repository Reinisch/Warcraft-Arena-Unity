using UnityEngine;

namespace Core
{
    public interface IControllerInputProvider
    {
        void PollInput(out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping);
    }
}
