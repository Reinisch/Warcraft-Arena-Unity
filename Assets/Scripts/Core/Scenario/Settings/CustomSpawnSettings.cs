using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Scenario
{
    [Serializable]
    public class CustomSpawnSettings
    {
        [SerializeField, UsedImplicitly] private Transform spawnPoint;
        [SerializeField, UsedImplicitly] private string customNameId;

        public Transform SpawnPoint => spawnPoint;
        public string CustomNameId => customNameId;
    }
}
