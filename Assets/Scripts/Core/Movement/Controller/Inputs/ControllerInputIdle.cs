using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Controller Input - Idle", menuName = "Game Data/Input/Controller/Idle")]
    public class ControllerInputIdle: ControllerInputSettings
    {
        public override void PollInput(
            Unit unit,
            out Vector3 inputVelocity,
            out Quaternion inputRotation,
            out bool jumping)
        {
            inputVelocity = Vector3.zero;
            inputRotation = unit.Rotation;
            jumping = false;
        }
    }
}