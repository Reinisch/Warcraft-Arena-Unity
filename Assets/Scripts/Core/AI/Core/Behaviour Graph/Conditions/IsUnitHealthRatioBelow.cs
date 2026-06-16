using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Is Unit Health Ratio Below",
        description: "Returns Success if the unit's health ratio is below the given threshold, Failure otherwise.",
        story: "[Unit] health ratio is below [Ratio]",
        category: "Action/Unit/Condition",
        id: "d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a1")]
    public class IsUnitHealthRatioBelow : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<float> Ratio;

        protected override Status OnStart()
        {
            return Unit.Value.HealthRatio < Ratio.Value ? Status.Success : Status.Failure;
        }
    }
}
