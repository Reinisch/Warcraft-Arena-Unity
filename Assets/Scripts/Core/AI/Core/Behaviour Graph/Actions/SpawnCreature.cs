using System;
using Assets.Scripts.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Core.Scenario.UnityBehavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Spawn Creature",
        description: "Spawns a creature at the specified spawn point.",
        story: "Spawn [CustomNameId] [CustomScale] [CreatureInfo] at [SpawnPoint], AI: [UnitInfoAI]",
        category: "Action/Map")]
    public class SpawnCreature : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<CreatureInfo> CreatureInfo;
        [SerializeReference] public BlackboardVariable<Transform> SpawnPoint;
        [SerializeReference] public BlackboardVariable<WorldEntityPrefab> Prefab;
        [SerializeReference] public BlackboardVariable<UnitInfoAI> UnitInfoAI;
        [SerializeReference] public BlackboardVariable<FactionDefinition> Faction;
        [SerializeReference] public BlackboardVariable<string> CustomNameId = new("");
        [SerializeReference] public BlackboardVariable<float> CustomScale = new(1.0f);

        [Inject]
        private Map Map { get; set; }

        protected override Status OnStart()
        {
            var info = CreatureInfo.Value;
            var spawnPoint = SpawnPoint.Value;

            World.UnitManager.Create<Creature>(Prefab.Value, new Creature.CreateToken
            {
                Map = Map,
                Position = spawnPoint.position,
                Rotation = spawnPoint.rotation,
                OriginalAIInfoId = UnitInfoAI.Value?.Id ?? 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = ClassType.Warrior,
                ModelId = info.ModelId,
                OriginalModelId = info.ModelId,
                FactionId = Faction.Value.FactionId,
                CreatureInfoId = info.Id,
                CustomName = string.IsNullOrEmpty(CustomNameId.Value) ? info.CreatureName : CustomNameId.Value,
                Scale = CustomScale.Value
            });

            return Status.Success;
        }
    }
}
