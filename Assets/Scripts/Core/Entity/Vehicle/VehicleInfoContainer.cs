using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Vehicle Info Container", menuName = "Game Data/Containers/Vehicle Info", order = 1)]
    internal class VehicleInfoContainer : ScriptableUniqueInfoContainer<VehicleInfo>
    {
        // The id→info lookup lives on BalanceReference (a MonoBehaviour whose state resets each play
        // session). Keeping it here, on a ScriptableObject, leaked stale entries between editor/MPPM
        // sessions and threw "same key already added" on re-register.
    }
}
