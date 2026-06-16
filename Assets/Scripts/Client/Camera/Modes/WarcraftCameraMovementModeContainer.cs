using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Camera Movement Mode Container", menuName = "Game Data/Containers/Camera Movement Mode")]
    public class WarcraftCameraMovementModeContainer : ScriptableUniqueInfoContainer<WarcraftCameraMovementMode> { }
}
