using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Map Definition", menuName = "Game Data/World/Scenario Definition", order = 3)]
    public class ScenarioDefinition: ScriptableObject
    {
        [SerializeField, UsedImplicitly] private MapDefinition map;
        [SerializeField, UsedImplicitly] private int scenario;
        [SerializeField, UsedImplicitly] private string scenarioName;

        public MapDefinition Map => map;
        public int Scenario => scenario;
        public string ScenarioName => scenarioName;
    }
}