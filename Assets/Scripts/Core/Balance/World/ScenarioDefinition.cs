using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Map Definition", menuName = "Game Data/World/Scenario Definition", order = 3)]
    public class ScenarioDefinition: ScriptableObject
    {
        [SerializeField, UsedImplicitly] private MapDefinition map;
        [SerializeField, UsedImplicitly] private string scenarioName;
        [SerializeField, UsedImplicitly] private Sprite slotBackground;
        [SerializeField, UsedImplicitly] private MapScenarioGraphSettings scenarioSettings;

        [Tooltip("Whether this scenario can be hosted for other players. Disable for scenarios whose mechanics " +
                 "aren't multiplayer-ready (e.g. the bossfight) — Create Server is then blocked; single-player still works.")]
        [SerializeField, UsedImplicitly] private bool supportsMultiplayer = true;

        public MapDefinition Map => map;
        public string ScenarioName => scenarioName;
        public Sprite SlotBackground => slotBackground;
        public MapScenarioGraphSettings ScenarioSettings => scenarioSettings;

        /// <summary>False for scenarios whose mechanics aren't multiplayer-ready; gates the lobby's Create Server.</summary>
        public bool SupportsMultiplayer => supportsMultiplayer;
    }
}