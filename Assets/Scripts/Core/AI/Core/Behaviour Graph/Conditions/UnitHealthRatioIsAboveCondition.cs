using System;
using Core;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "UnitHealthRatioIsAbove", story: "[Unit] health ratio is above [Ratio]", category: "Conditions", id: "46bad74957c8e50717fb5f2aec748848")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class UnitHealthRatioIsAboveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Unit> Unit;
    [SerializeReference] public BlackboardVariable<float> Ratio;

    public override bool IsTrue()
    {
        return Unit.Value.HealthRatio > Ratio.Value;
    }
}
