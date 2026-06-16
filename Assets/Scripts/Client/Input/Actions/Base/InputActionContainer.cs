using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Input Action Container", menuName = "Game Data/Containers/Input Action")]
    public class InputActionContainer : ScriptableUniqueInfoContainer<InputAction> { }
}
