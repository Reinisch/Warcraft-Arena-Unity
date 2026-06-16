using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Core.Scenario.UnityBehavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Spawn Player",
        description: "Spawns the player at the specified spawn point.",
        story: "Spawn player at [SpawnPoint] with model [ModelId]",
        category: "Action/Map",
        id: "b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7")]
    public class SpawnPlayer : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Transform> SpawnPoint;
        [SerializeReference] public BlackboardVariable<int> ModelId = new(0);

        [Inject]
        private Map Map { get; set; }

        protected override Status OnStart()
        {
            if (World.PlayerManager.Player != null)
                return Status.Success;

            var player = World.PlayerManager.Create(new Player.CreateToken
            {
                Map = Map,
                Position = SpawnPoint.Value.position,
                Rotation = SpawnPoint.Value.rotation,
                OriginalAIInfoId = 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = World.LocalPlayerClass, // lobby-chosen class for the host/single-player player (sanitised upstream)
                ModelId = ModelId.Value,
                OriginalModelId = ModelId.Value,
                FactionId = Balance.DefaultFaction.FactionId,
                PlayerName = "Player",
                Scale = 1
            });
            player.MovementMode = World.DefaultMovementMode;

            return Status.Success;
        }
    }
}
