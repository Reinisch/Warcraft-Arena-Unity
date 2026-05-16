using Core;
using System;
using UnityEngine;

namespace Client
{
    public abstract class UnitControllerInputMode: ScriptableObject
    {
        [Serializable]
        public class ModeType
        {
            [field: SerializeField]
            public UnitControllerInputMode Mode { get; private set; }

            [field: SerializeField]
            public MovementMode Type { get; private set; }
        }

        public abstract void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping);
    }
}
