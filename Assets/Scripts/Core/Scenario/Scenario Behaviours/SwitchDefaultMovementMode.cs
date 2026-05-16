using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Scenario
{
    public class SwitchDefaultMovementMode: ScenarioAction
    {
        [SerializeField, UsedImplicitly] private MovementMode movementMode;

        internal override void Initialize(MapScenario scenario)
        {
            base.Initialize(scenario);

            EventHandler.RegisterEvent(World, GameEvents.ServerLaunched, OnServerLaunched);
        }

        internal override void DeInitialize()
        {
            EventHandler.UnregisterEvent(World, GameEvents.ServerLaunched, OnServerLaunched);

            base.DeInitialize();
        }

        private void OnServerLaunched()
        {
            World.DefaultMovementMode = movementMode;

            foreach(var unit in World.UnitManager.Entities)
            {
                if (unit is Player player)
                {
                    player.MovementMode = movementMode;
                }
            }
        }
    }
}