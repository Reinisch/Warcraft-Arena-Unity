using System;
using Common;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "End Arena Match",
        description: "Declares the winner and ends the match, waits a short result delay.",
        story: "End arena match after [PostMatchSeconds]s",
        category: "Action/Arena",
        id: "a4e0a1f2b3c4d5e6f70819a2b3c4d5e8")]
    public class EndArenaMatchAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<ArenaController> Arena;
        [SerializeReference] public BlackboardVariable<float> PostMatchSeconds = new(5f);

        [Inject]
        private EventBus EventBus { get; set; }

        private float remaining;
        private bool leaveRequested;

        protected override Status OnStart()
        {
            if (World == null || !World.HasServerLogic || Arena?.Value == null)
                return Status.Success;

            Arena.Value.EndMatch();
            remaining = PostMatchSeconds.Value;
            leaveRequested = false;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            remaining -= Time.deltaTime;
            if (remaining > 0f)
                return Status.Running;

            if (!leaveRequested)
            {
                leaveRequested = true;
                EventBus.ExecuteEvent(GameEvents.SessionLeaveRequested);
            }

            return Status.Success;
        }
    }
}
