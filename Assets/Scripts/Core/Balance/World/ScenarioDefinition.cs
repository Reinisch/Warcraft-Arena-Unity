using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public enum ScenarioType
    {
        Standard,
        Arena
    }

    [CreateAssetMenu(fileName = "Map Definition", menuName = "Game Data/World/Scenario Definition", order = 3)]
    public class ScenarioDefinition: ScriptableObject
    {
        [SerializeField, UsedImplicitly] private MapDefinition map;
        [SerializeField, UsedImplicitly] private string scenarioName;
        [SerializeField, UsedImplicitly] private Sprite slotBackground;
        [SerializeField, UsedImplicitly] private MapScenarioGraphSettings scenarioSettings;
        [SerializeField, UsedImplicitly] private bool supportsMultiplayer = true;
        [SerializeField, UsedImplicitly] private ScenarioType type = ScenarioType.Standard;
        [SerializeField, UsedImplicitly] private int teamSize = 2;
        [SerializeField, UsedImplicitly] private float minArenaWaitTime = 15;

        public MapDefinition Map => map;
        public string ScenarioName => scenarioName;
        public Sprite SlotBackground => slotBackground;
        public MapScenarioGraphSettings ScenarioSettings => scenarioSettings;
        public float MinArenaWaitTime => minArenaWaitTime;
        public bool SupportsMultiplayer => supportsMultiplayer;
        public ScenarioType Type => type;
        public bool IsArena => type == ScenarioType.Arena;
        public int TeamSize => teamSize;
    }
}