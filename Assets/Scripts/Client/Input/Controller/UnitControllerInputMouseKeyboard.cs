using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Player Unit Input - Mouse Keyboard", menuName = "Player Data/Input/Unit/Mouse Keyboard", order = 1)]
    public class UnitControllerInputMouseKeyboard : ScriptableObject, IControllerInputProvider
    {
        [SerializeField] private CameraReference cameraReference;
        [SerializeField] private InputReference inputReference;
        [SerializeField] private List<UnitControllerInputMode.ModeType> inputModes;

        private Dictionary<MovementMode, UnitControllerInputMode> inputModesByType;

        private Dictionary<MovementMode, UnitControllerInputMode> InputModesByType
        {
            get
            {
                if (inputModesByType == null)
                {
                    inputModesByType = new Dictionary<MovementMode, UnitControllerInputMode>();
                    inputModes.ForEach(item => inputModesByType.Add(item.Type, item.Mode));
                }

                return inputModesByType;
            }
        }

        public void PollInput(Unit unit, out Vector3 inputVelocity, out Quaternion inputRotation, out bool jumping)
        {
            InputModesByType[unit.MovementMode].PollInput(unit, out inputVelocity, out inputRotation, out jumping);
        }
    }
}
