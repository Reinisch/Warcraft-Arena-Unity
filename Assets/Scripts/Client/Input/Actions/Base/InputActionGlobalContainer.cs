using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action Global Container", menuName = "Game Data/Containers/Input Action Global")]
    public class InputActionGlobalContainer : ScriptableUniqueInfoContainer<InputActionGlobal> { }
}
