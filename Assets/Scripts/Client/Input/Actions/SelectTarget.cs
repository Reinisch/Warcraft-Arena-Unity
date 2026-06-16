using UnityEngine;
using JetBrains.Annotations;
using Zenject;

namespace Client.Actions
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action - Select Target", menuName = "Player Data/Input/Actions/Select Target", order = 2)]
    public class SelectTarget : InputAction
    {
        [Inject] private TargetingReference targeting;
        [SerializeField, UsedImplicitly] private TargetingOptions options;

        public override void Execute()
        {
            targeting.SelectTarget(options);
        }
    }
}
