using UnityEngine;
using JetBrains.Annotations;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Toggle Option", menuName = "Player Data/Input/Actions/Toggle Option", order = 2)]
    public class ToggleOption : InputAction
    {
        [SerializeField, UsedImplicitly] private GameOptionBool gameOption;

        public override void Execute()
        {
            gameOption.Toggle();
        }
    }
}
