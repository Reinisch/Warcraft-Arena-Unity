using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Sound Kit Container", menuName = "Game Data/Containers/Unit Sound Kit", order = 1)]
    public class UnitSoundKitContainer : ScriptableUniqueInfoContainer<UnitSoundKit>
    {
        // No id lookup needed (nothing consumed SoundKitsById). Lookup dictionaries on a ScriptableObject
        // leak runtime state between editor/MPPM play sessions, so they belong on MonoBehaviour references.
    }
}
