using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.BehaviorGraph
{
    public class SpawnPlayerAI : ScenarioSetupAction
    {
        [SerializeField, UsedImplicitly] private ScenarioSpawnSetup customSpawnSettings;

        public void Execute(World world, Map map)
        {
            world.UnitManager.Create<Player>(customSpawnSettings.EntityPrefab, new Player.CreateToken
            {
                Position = transform.position,
                Rotation = transform.rotation,
                OriginalAIInfoId = customSpawnSettings.UnitInfoAI?.Id ?? 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ModelId = 1,
                ClassType = ClassType.Mage, // AI bot player — keeps its designed class, not the host's lobby choice
                OriginalModelId = 1,
                FactionId = customSpawnSettings.Faction.FactionId,
                PlayerName = customSpawnSettings.CustomNameId,
                Scale = customSpawnSettings.CustomScale,
                Map = map
            });
        }
    }
}
