using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Is Unit Dead",
        description: "Returns Success if the unit is dead, Failure otherwise.",
        story: "[Unit] is dead",
        category: "Action/Unit/Condition",
        id: "b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e9")]
    public class IsUnitDead : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;

        protected override Status OnStart()
        {
            return Unit.Value.IsDead ? Status.Success : Status.Failure;
        }
    }
}
