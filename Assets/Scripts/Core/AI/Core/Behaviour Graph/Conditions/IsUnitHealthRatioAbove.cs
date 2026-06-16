using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Is Unit Health Ratio Above",
        description: "Returns Success if the unit's health ratio is above the given threshold, Failure otherwise.",
        story: "[Unit] health ratio is above [Ratio]",
        category: "Action/Unit/Condition",
        id: "c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f0")]
    public class IsUnitHealthRatioAbove : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<float> Ratio;

        protected override Status OnStart()
        {
            return Unit.Value.HealthRatio > Ratio.Value ? Status.Success : Status.Failure;
        }
    }
}
