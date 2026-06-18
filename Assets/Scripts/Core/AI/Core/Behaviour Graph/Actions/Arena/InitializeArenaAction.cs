using System;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Common;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Initialize Arena",
        description: "Creates the arena match controller from the configured teams and spawn points. Server-side.",
        story: "Initialize arena",
        category: "Action/Arena",
        id: "a4e0a1f2b3c4d5e6f70819a2b3c4d5e6")]
    public class InitializeArenaAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<FactionDefinition> FactionA;
        [SerializeReference] public BlackboardVariable<FactionDefinition> FactionB;
        [SerializeReference] public BlackboardVariable<List<GameObject>> SpawnPointsA;
        [SerializeReference] public BlackboardVariable<List<GameObject>> SpawnPointsB;
        [SerializeReference] public BlackboardVariable<float> WarmupSeconds = new(60f);
        [SerializeReference] public BlackboardVariable<SpellInfo> PreparationSpell;
        [SerializeReference] public BlackboardVariable<WorldEntityPrefab> PlayerAiPrefab;
        [SerializeReference] public BlackboardVariable<UnitInfoAI> PlayerAiInfo;
        [SerializeReference] public BlackboardVariable<UnitAttributeDefinition> ArenaAttributes;
        [SerializeReference] public BlackboardVariable<ArenaController> Arena;

        [Inject]
        private Map Map { get; set; }

        [Inject]
        private EventBus EventBus { get; set; }

        protected override Status OnStart()
        {
            // The scenario graph runs server-only, but guard anyway.
            if (World == null || !World.HasServerLogic)
                return Status.Success;

            if (FactionA?.Value == null || FactionB?.Value == null)
                Debug.LogWarning("Initialize Arena: team factions are not set — players will keep their default faction and may not be hostile.");

            var controllerObject = new GameObject("Arena Controller");
            controllerObject.transform.SetParent(Map.Settings.transform, false);

            var controller = controllerObject.AddComponent<ArenaController>();
            controller.Initialize(World, Map, EventBus,
                FactionA?.Value, FactionB?.Value,
                ToTransforms(SpawnPointsA?.Value), ToTransforms(SpawnPointsB?.Value),
                Map.Scenario, WarmupSeconds.Value, PreparationSpell?.Value,
                PlayerAiPrefab?.Value, PlayerAiInfo?.Value, ArenaAttributes?.Value);

            if (Arena != null)
                Arena.Value = controller;

            return Status.Success;
        }

        private static List<Transform> ToTransforms(List<GameObject> objects)
        {
            var transforms = new List<Transform>();
            if (objects != null)
                foreach (GameObject gameObject in objects)
                    if (gameObject != null)
                        transforms.Add(gameObject.transform);

            return transforms;
        }
    }
}
