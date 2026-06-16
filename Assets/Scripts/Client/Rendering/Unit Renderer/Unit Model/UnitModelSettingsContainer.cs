using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Unit Model Settings Container", menuName = "Game Data/Containers/Unit Model Settings", order = 1)]
    public class UnitModelSettingsContainer : ScriptableUniqueInfoContainer<UnitModelSettings>
    {
        // The id lookup lives on RenderingReference (a MonoBehaviour whose state resets each play session).
        // A ScriptableObject must not retain runtime lookup state between editor/MPPM sessions.
    }
}
