using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Sound
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Sound Group Settings Container", menuName = "Game Data/Containers/Sound Group Settings", order = 1)]
    internal class SoundGroupSettingsContainer : ScriptableUniqueInfoContainer<SoundGroupSettings>
    {
        // The runtime source pool (settings→SoundHandleComponent) lives on SoundModule (a MonoBehaviour).
        // It holds runtime GameObjects/AudioSources, which a ScriptableObject must not retain between
        // editor/MPPM play sessions.
    }
}
