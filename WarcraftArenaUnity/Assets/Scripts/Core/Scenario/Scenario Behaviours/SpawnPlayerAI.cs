using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Scenario
{
    public class SpawnPlayerAI : ScenarioAction
    {
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
            var playerCreateToken = new Player.CreateToken
            {
                Position = customSpawnSettings.SpawnPoint.position,
                Rotation = customSpawnSettings.SpawnPoint.rotation,
                OriginalAIInfoId = customSpawnSettings.UnitInfoAI?.Id ?? 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ModelId = 1,
                ClassType = ClassType.Mage,
                OriginalModelId = 1,
                FactionId = Balance.DefaultFaction.FactionId,
                PlayerName = customSpawnSettings.CustomNameId,
                Scale = customSpawnSettings.CustomScale
            };

            World.UnitManager.Create<Player>(BoltPrefabs.Player, playerCreateToken);
        }
    }
}
