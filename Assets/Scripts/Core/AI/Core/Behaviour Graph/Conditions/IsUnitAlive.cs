using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Is Unit Alive",
        description: "Returns Success if the unit is alive, Failure otherwise.",
        story: "[Unit] is alive",
        category: "Action/Unit/Condition",
        id: "a2b3c4d5e6f7a8b9c0d1e2f3a4b5c6d8")]
    public class IsUnitAlive : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;

        protected override Status OnStart()
        {
            return Unit.Value.IsAlive ? Status.Success : Status.Failure;
        }
    }
}
