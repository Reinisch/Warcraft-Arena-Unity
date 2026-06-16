using Client.Spells;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Visual Info Container", menuName = "Game Data/Containers/Aura Visual Info", order = 1)]
    public class AuraVisualsInfoContainer : ScriptableUniqueInfoContainer<AuraVisualsInfo>
    {
        // The id lookup lives on RenderingReference (a MonoBehaviour whose state resets each play session).
        // A ScriptableObject must not retain runtime lookup state between editor/MPPM sessions.
    }
}
