using System;
using Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "UnitIsDead", story: "Target [Unit] is dead.", category: "Conditions", id: "1629da764ebd5b559d00e04bca7c93ed")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class UnitIsDeadCondition : BehaviourGraphCondition
{
    [SerializeReference] 
    public BlackboardVariable<Unit> Unit;

    public override bool IsTrue()
    {
        return Unit.Value.IsDead;
    }
}
