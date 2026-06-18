using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Is Arena Match Over",
        description: "True once one team has been eliminated or the match already ended.",
        story: "Arena match is over",
        category: "Condition/Arena",
        id: "a4e0a1f2b3c4d5e6f70819a2b3c4d5e9")]
    public class IsArenaMatchOverCondition : BehaviourGraphCondition
    {
        [SerializeReference] public BlackboardVariable<ArenaController> Arena;

        public override bool IsTrue() => Arena?.Value != null && Arena.Value.IsMatchOver();
    }
}
