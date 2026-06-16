using JetBrains.Annotations;
using Unity.Behavior;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Map Scenario - Behaviour Graph", menuName = "Game Data/World/Map Scenario Graph", order = 4)]
    public sealed class MapScenarioGraphSettings : ScriptableObject
    {
        [field: SerializeField] public BehaviorGraphAgent GraphPrefab { get; private set; }
    }
}
