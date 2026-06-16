using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Controller Input Container", menuName = "Game Data/Containers/Controller Input")]
    public class ControllerInputContainer : ScriptableUniqueInfoContainer<ControllerInputSettings>
    {
        [field: SerializeField]
        public ControllerInputIdle Idle { get; private set; }

        [field: SerializeField]
        public ControllerInputMovementSet Player { get; private set; }
    }
}