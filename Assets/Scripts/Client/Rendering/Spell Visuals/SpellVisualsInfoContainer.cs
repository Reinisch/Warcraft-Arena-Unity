using Client.Spells;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Visual Info Container", menuName = "Game Data/Containers/Spell Visual Info", order = 1)]
    public class SpellVisualsInfoContainer : ScriptableUniqueInfoContainer<SpellVisualsInfo>
    {
        // The id lookup (and per-item Initialize/Deinitialize) lives on RenderingReference (a MonoBehaviour
        // whose state resets each play session). A ScriptableObject must not retain runtime lookup state
        // between editor/MPPM sessions.
    }
}
