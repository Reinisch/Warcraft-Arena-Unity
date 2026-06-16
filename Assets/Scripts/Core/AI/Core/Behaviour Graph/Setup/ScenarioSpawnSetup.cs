using System;
using Assets.Scripts.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable]
    public class ScenarioSpawnSetup
    {
        [SerializeField, UsedImplicitly] private WorldEntityPrefab entityPrefab;
        [SerializeField, UsedImplicitly] private UnitInfoAI unitInfoAI;
        [SerializeField, UsedImplicitly] private string customNameId;
        [SerializeField, UsedImplicitly] private float customScale = 1.0f;
        [SerializeField, UsedImplicitly] private FactionDefinition faction;

        public WorldEntityPrefab EntityPrefab => entityPrefab;
        public UnitInfoAI UnitInfoAI => unitInfoAI;
        public string CustomNameId => customNameId;
        public float CustomScale => customScale;
        public FactionDefinition Faction => faction;
    }
}
