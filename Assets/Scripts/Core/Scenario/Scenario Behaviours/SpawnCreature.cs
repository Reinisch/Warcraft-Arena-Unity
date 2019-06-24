using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Scenario
{
    public class SpawnCreature : ScenarioAction
    {
        [SerializeField, UsedImplicitly] private Transform spawnPoint;
        [SerializeField, UsedImplicitly] private Creature creaturePrototype;

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
            WorldManager.UnitManager.Create<Creature>(creaturePrototype.BoltEntity.PrefabId, new Unit.CreateToken
            {
                Position = spawnPoint.position,
                Rotation = spawnPoint.rotation,
                DeathState = DeathState.Alive,
                FreeForAll = true,
                FactionId = Balance.DefaultFaction.FactionId,
            }).BoltEntity.TakeControl();
        }
    }
}
