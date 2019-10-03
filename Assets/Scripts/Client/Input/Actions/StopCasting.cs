using UnityEngine;
using JetBrains.Annotations;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Stop Casting", menuName = "Player Data/Input/Actions/Stop Casting", order = 1)]
    public class StopCasting : InputAction
    {
        [SerializeField, UsedImplicitly] private InputReference inputReference;

        public override void Execute()
        {
            inputReference.StopCasting();
        }
    }
}
