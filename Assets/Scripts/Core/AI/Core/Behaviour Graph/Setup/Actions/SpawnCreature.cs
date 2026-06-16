using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.BehaviorGraph
{
    public class SpawnCreature : ScenarioSetupAction
    {
        [SerializeField, UsedImplicitly] private CreatureInfo creatureInfo;
        [SerializeField, UsedImplicitly] private ScenarioSpawnSetup customSpawnSettings;

        public void Execute(World world, Map map)
        {
            world.UnitManager.Create<Creature>(customSpawnSettings.EntityPrefab, new Creature.CreateToken
            {
                Position = transform.position,
                Rotation = transform.rotation,
                OriginalAIInfoId = customSpawnSettings.UnitInfoAI?.Id ?? 0,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                ClassType = ClassType.Warrior,
                ModelId = creatureInfo.ModelId,
                OriginalModelId = creatureInfo.ModelId,
                FactionId = customSpawnSettings.Faction.FactionId,
                CreatureInfoId = creatureInfo.Id,
                CustomName = string.IsNullOrEmpty(customSpawnSettings.CustomNameId) ? creatureInfo.CreatureName : customSpawnSettings.CustomNameId,
                Scale = customSpawnSettings.CustomScale,
                Map = map,
            });
        }
    }
}
