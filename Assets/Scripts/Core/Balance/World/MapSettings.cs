using System;
using System.Collections.Generic;
using Core.Scenario;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

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

        internal float GridCellSize => gridCellSize;
        internal BoxCollider BoundingBox => boundingBox;
        internal Transform DefaultSpawnPoint => defaultSpawnPoint;
        internal BalanceReference Balance => balance;
        internal MapDefinition Definition => mapDefinition;

        public List<Transform> FindSpawnPoints(Team team)
        {
            return spawnInfos.Find(spawnInfo => spawnInfo.Team == team).SpawnPoints;
        }

        public MapScenario CreateScenario(int scenarioId, Transform parent)
        {
            if (scenarioId < 0 || scenarioId >= mapDefinition.ScenarioPrefabs.Count)
            {
                return null;
            }

            return Instantiate(mapDefinition.ScenarioPrefabs[scenarioId], parent);
        }
    }
}