using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class MapSettings : MonoBehaviour
    {
        [Serializable]
        public class ArenaSpawnInfo
        {
            [SerializeField, UsedImplicitly] private Team team;
            [SerializeField, UsedImplicitly] private List<Transform> spawnPoints;

            public Team Team => team;
            public List<Transform> SpawnPoints => spawnPoints;
        }

        [SerializeField, UsedImplicitly] private BoxCollider boundingBox;
        [SerializeField, UsedImplicitly] private GridLayoutGroup gridLayout;
        [SerializeField, UsedImplicitly] private List<GridCell> gridCells;
        [SerializeField, UsedImplicitly] private List<ArenaSpawnInfo> spawnInfos;

        public GridLayoutGroup GridLayout => gridLayout;
        public List<GridCell> GridCells => gridCells;

        public List<Transform> FindSpawnPoints(Team team)
        {
            return spawnInfos.Find(spawnInfo => spawnInfo.Team == team).SpawnPoints;
        }

#if UNITY_EDITOR
        [ContextMenu("Collect grid cells")]
        private void CollectCells()
        {
            gridCells = new List<GridCell>(gridLayout.GetComponentsInChildren<GridCell>());
        }
#endif
    }
}