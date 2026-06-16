using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client.Sound
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Sound Entry Container", menuName = "Game Data/Containers/Sound Entry", order = 1)]
    internal class SoundEntryContainer : ScriptableUniqueInfoContainer<SoundEntry> { }
}
