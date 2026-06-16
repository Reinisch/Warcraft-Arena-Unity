using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Tooltip Info Container", menuName = "Game Data/Containers/Spell Tooltip Info", order = 1)]
    public class SpellTooltipInfoContainer : ScriptableUniqueInfoContainer<SpellTooltipInfo>
    {
        // The spell→tooltip lookups live on LocalizationReference (a MonoBehaviour whose state resets each
        // play session). Keeping them here, on a ScriptableObject, leaked stale entries between editor/MPPM
        // sessions and threw "same key already added" on re-register.
    }
}
