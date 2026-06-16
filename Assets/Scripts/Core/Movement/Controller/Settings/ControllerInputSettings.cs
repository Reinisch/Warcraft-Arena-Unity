using Common;
using UnityEngine;

namespace Core
{
    public abstract class ControllerInputSettings : ScriptableUniqueInfo<ControllerInputSettings>
    {
        public abstract void PollInput(
            Unit unit,
            out Vector3 inputVelocity,
            out Quaternion inputRotation,
            out bool jumping);
    }
}
