using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Scenario
{
    public class SpawnCreature : ScenarioAction
    {
        [SerializeField, UsedImplicitly] private CreatureInfo creatureInfo;
        [SerializeField, UsedImplicitly] private CustomSpawnSettings customSpawnSettings;

        internal override void Initialize(Map map)
        {
            base.Initialize(map);

            EventHandler.RegisterEvent(World, GameEvents.ServerLaunched, OnServerLaunched);
        }

        internal override void DeInitialize()
        {
            EventHandler.UnregisterEvent(World, GameEvents.ServerLaunched, OnServerLaunched);

            base.DeInitialize();
        }

        private void OnServerLaunched()
        {
            Creature creature = World.UnitManager.Create<Creature>(BoltPrefabs.Creature, new Creature.CreateToken
            {
                Position = customSpawnSettings.SpawnPoint.position,
                Rotation = customSpawnSettings.SpawnPoint.rotation,
                OriginalAIInfoId = customSpawnSettings.UnitInfoAI?.Id ?? 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = ClassType.Warrior,
                ModelId = creatureInfo.ModelId,
                OriginalModelId = creatureInfo.ModelId,
                FactionId = Balance.DefaultFaction.FactionId,
                CreatureInfoId = creatureInfo.Id,
                CustomName = string.IsNullOrEmpty(customSpawnSettings.CustomNameId) ? creatureInfo.CreatureName : customSpawnSettings.CustomNameId,
                Scale = customSpawnSettings.CustomScale
            });

            creature.BoltEntity.TakeControl();
        }
    }
}
