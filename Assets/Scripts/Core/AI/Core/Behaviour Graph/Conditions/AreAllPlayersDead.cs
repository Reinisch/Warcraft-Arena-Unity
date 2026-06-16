using System;
using Unity.Behavior;
using Unity.Properties;
using Zenject;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Are All Players Dead",
        description: "Returns Success if every player in the world is dead, Failure if at least one is alive.",
        story: "All players are dead",
        category: "Condition/Unit",
        id: "e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b2")]
    public class AreAllPlayersDead : BehaviourGraphCondition
    {
        [Inject]
        private World world;

        public override bool IsTrue()
        {
            foreach (var unit in world.UnitManager.Entities)
                if (unit is Player { IsDead: false })
                    return false;

            return true;
        }
    }
}
