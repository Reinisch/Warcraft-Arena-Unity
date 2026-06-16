using System;
using Unity.Behavior;
using Unity.Properties;
using Zenject;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Heal All Players",
        description: "Fully heals every player currently in the world.",
        story: "Heal all players to full health",
        category: "Action/Unit",
        id: "d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a0")]
    public class HealAllPlayers : BehaviourGraphAction
    {
        [Inject]
        private World world;

        protected override Status OnStart()
        {
            foreach (var unit in world.UnitManager.Entities)
                if (unit is Player)
                    unit.DealHeal(unit, unit.MaxHealth);

            return Status.Success;
        }
    }
}
