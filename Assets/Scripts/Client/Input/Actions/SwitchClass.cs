using Core;
using UnityEngine;
using JetBrains.Annotations;
using Zenject;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Switch Class", menuName = "Player Data/Input/Actions/Switch Class", order = 2)]
    public class SwitchClass : InputAction
    {
        [Inject] private InputReference input;
        [SerializeField, UsedImplicitly] private ClassType classType;

        public override void Execute() => input.SwitchClass(classType);
    }
}
