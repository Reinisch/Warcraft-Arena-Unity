using UnityEngine;

namespace Core
{
    public class IdleControllerInputProvider : IControllerInputProvider
    {
        public void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            inputVelocity = Vector3.zero;
            inputRotation = unit.Rotation;
            jumping = false;
        }
    }
}
