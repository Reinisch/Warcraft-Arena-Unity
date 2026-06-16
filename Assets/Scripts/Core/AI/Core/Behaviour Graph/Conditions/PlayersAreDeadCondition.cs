using Core;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "PlayersAreDead", story: "All players are dead for [Unit]", category: "Conditions", id: "ec2f98f12a42081982d21a91d45f883a")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class PlayersAreDeadCondition : BehaviourGraphCondition
{
    [SerializeReference] public BlackboardVariable<Unit> Unit;

    public override bool IsTrue()
    {
        foreach (var unit in Unit.Value.World.UnitManager.Entities)
            if (unit is Player { IsDead: false })
                return false;

        return true;
    }
}
