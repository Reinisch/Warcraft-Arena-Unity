using UnityEngine;

namespace Core
{
    public interface IControllerInputProvider
    {
        void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping);
    }
}
