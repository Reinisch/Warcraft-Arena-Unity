using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Controller Input - Movement Set", menuName = "Game Data/Input/Controller/Movement Set")]
    public class ControllerInputMovementSet : ControllerInputSettings
    {
        [Serializable]
        public class ModeType
        {
            [field: SerializeField]
            public ControllerInputSettings Mode { get; private set; }

            [field: SerializeField]
            public MovementMode Type { get; private set; }
        }

        [SerializeField]
        private List<ModeType> inputModes;

        private Dictionary<MovementMode, ControllerInputSettings> inputModesByType;

        private Dictionary<MovementMode, ControllerInputSettings> InputModesByType
        {
            get
            {
                if (inputModesByType == null)
                {
                    inputModesByType = new Dictionary<MovementMode, ControllerInputSettings>();
                    inputModes.ForEach(item => inputModesByType.Add(item.Type, item.Mode));
                }

                return inputModesByType;
            }
        }

        public override void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            InputModesByType[unit.MovementMode].PollInput(unit, out inputVelocity, out inputRotation, out jumping);
        }
    }
}
