using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Sound Info Container", menuName = "Game Data/Containers/Spell Sound Info", order = 1)]
    public class SpellSoundInfoContainer : ScriptableUniqueInfoContainer<SpellSoundInfo>
    {
        // The spell→sound lookup lives on SoundReference (a MonoBehaviour whose state resets each play
        // session). Keeping it here, on a ScriptableObject, leaked stale entries between editor/MPPM
        // sessions.
    }
}
