using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Vehicle Seat Info Container", menuName = "Game Data/Containers/Vehicle Seat Info", order = 1)]
    internal class VehicleSeatInfoContainer : ScriptableUniqueInfoContainer<VehicleSeatInfo>
    {
        // No id lookup needed (nothing consumed VehicleSeatInfoById). Lookup dictionaries on a
        // ScriptableObject leak runtime state between editor/MPPM play sessions, so they belong on
        // MonoBehaviour references.
    }
}
