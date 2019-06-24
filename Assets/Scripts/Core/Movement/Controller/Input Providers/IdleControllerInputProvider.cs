using UnityEngine;

namespace Core
{
    public class IdleControllerInputProvider : IControllerInputProvider
    {
        private readonly Unit unit;

        public IdleControllerInputProvider(Unit unit)
        {
            this.unit = unit;
        }

        public void PollInput(out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            inputVelocity = Vector3.zero;
            inputRotation = unit.Rotation;
            jumping = false;
        }
    }
}
