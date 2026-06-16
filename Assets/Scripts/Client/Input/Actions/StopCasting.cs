using UnityEngine;
using JetBrains.Annotations;
using Zenject;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Stop Casting", menuName = "Player Data/Input/Actions/Stop Casting", order = 1)]
    public class StopCasting : InputAction
    {
        [Inject] private InputReference inputReference;

        public override void Execute()
        {
            inputReference.StopCasting();
        }
    }
}
