using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Wait For Arena End",
        description: "Runs until one team has been eliminated.",
        story: "Wait for arena match to end",
        category: "Action/Arena",
        id: "a4e0a1f2b3c4d5e6f70819a2b3c4d5ea")]
    public class WaitForArenaEndAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<ArenaController> Arena;

        protected override Status OnStart() => Evaluate();

        protected override Status OnUpdate() => Evaluate();

        private Status Evaluate()
        {
            if (Arena?.Value == null)
                return Status.Success;

            return Arena.Value.IsMatchOver() ? Status.Success : Status.Running;
        }
    }
}
