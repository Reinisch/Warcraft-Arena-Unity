using System;
using System.Collections.Generic;
using Core.Scenario;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public class MapSettings : MonoBehaviour
    {
        [Serializable]
        private class ArenaSpawnInfo
        {
            [SerializeField, UsedImplicitly] private Team team;
            [SerializeField, UsedImplicitly] private List<Transform> spawnPoints;

            public Team Team => team;
            public List<Transform> SpawnPoints => spawnPoints;
        }

        [SerializeField, UsedImplicitly, Range(2.0f, 50.0f)] private float gridCellSize;
        [SerializeField, UsedImplicitly] private Transform defaultSpawnPoint;
        [SerializeField, UsedImplicitly] private BoxCollider boundingBox;
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private MapDefinition mapDefinition;
        [SerializeField, UsedImplicitly] private List<ArenaSpawnInfo> spawnInfos;
        [SerializeField, UsedImplicitly] private List<ScenarioAction> scenarioActions;

        internal float GridCellSize => gridCellSize;
        internal BoxCollider BoundingBox => boundingBox;
        internal Transform DefaultSpawnPoint => defaultSpawnPoint;
        internal BalanceReference Balance => balance;
        internal MapDefinition Definition => mapDefinition;

        internal List<ScenarioAction> ScenarioActions => scenarioActions;

        public List<Transform> FindSpawnPoints(Team team)
        {
            return spawnInfos.Find(spawnInfo => spawnInfo.Team == team).SpawnPoints;
        }

#if UNITY_EDITOR
        [UsedImplicitly, ContextMenu("Collect scenario actions")]
        private void CollectScenario()
        {
            scenarioActions = new List<ScenarioAction>(GetComponentsInChildren<ScenarioAction>());
        }
#endif
    }
}