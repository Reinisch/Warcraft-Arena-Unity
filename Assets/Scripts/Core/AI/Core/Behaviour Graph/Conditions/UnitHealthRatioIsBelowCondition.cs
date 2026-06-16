using System;
using Core;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "UnitHealthRatioIsBelow", story: "[Unit] health ratio is below [Ratio]", category: "Conditions", id: "eb54fe9b6bbf8ae91e54649ce19efe57")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class UnitHealthRatioIsBelowCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Unit> Unit;
    [SerializeReference] public BlackboardVariable<float> Ratio;

    public override bool IsTrue()
    {
        return Unit.Value.HealthRatio < Ratio.Value;
    }
}
