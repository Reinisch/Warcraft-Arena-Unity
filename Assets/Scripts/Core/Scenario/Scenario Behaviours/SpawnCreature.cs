using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Scenario
{
    public class SpawnCreature : ScenarioAction
    {
        [SerializeField, UsedImplicitly] private CreatureDefinition creatureDefinition;
        [SerializeField, UsedImplicitly] private CustomSpawnSettings customSpawnSettings;

        internal override void Initialize(Map map)
        {
            base.Initialize(map);

            EventHandler.RegisterEvent(WorldManager, GameEvents.ServerLaunched, OnServerLaunched);
        }

        internal override void DeInitialize()
        {
            EventHandler.UnregisterEvent(WorldManager, GameEvents.ServerLaunched, OnServerLaunched);

            base.DeInitialize();
        }

        private void OnServerLaunched()
        {
            Creature creature = WorldManager.UnitManager.Create<Creature>(BoltPrefabs.Creature, new Creature.CreateToken
            {
                Position = customSpawnSettings.SpawnPoint.position,
                Rotation = customSpawnSettings.SpawnPoint.rotation,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = ClassType.Warrior,
                ModelId = creatureDefinition.ModelId,
                OriginalModelId = creatureDefinition.ModelId,
                FactionId = Balance.DefaultFaction.FactionId,
                CustomNameId = customSpawnSettings.CustomNameId,
                Scale = customSpawnSettings.CustomScale
            });

            creature.BoltEntity.TakeControl();
        }
    }
}
