using System;
using Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Copy Scenario Variables",
        description: "Copies all matching blackboard variables from the map's ScenarioGraph into the current graph's blackboard.",
        story: "Copy scenario variables from map into current graph",
        category: "Action/Map",
        id: "f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6c7")]
    public class CopyScenarioVariables : BehaviourGraphAction
    {
        public const string BlackboardSelfVariableName = "Self";

        [SerializeReference] public BlackboardVariable<Unit> Unit;

        [Inject]
        private World world;

        protected override Status OnStart()
        {
            var agent = GameObject.GetComponent<BehaviorGraphAgent>();
            if (agent == null || agent.Graph == null)
                return Status.Success;

            BlackboardReference targetBlackboard = agent.Graph.BlackboardReference;
            BlackboardReference sourceBlackboard = Unit.Value.Map.ScenarioBlackboard;

            if (targetBlackboard == null || sourceBlackboard == null)
                return Status.Success;

            foreach (BlackboardVariable sourceVariable in sourceBlackboard.Blackboard.Variables)
            {
                if (sourceVariable.Name == BlackboardSelfVariableName)
                    continue;
                    
                if (targetBlackboard.GetVariable(sourceVariable.Name, out BlackboardVariable targetVariable))
                    targetVariable.ObjectValue = sourceVariable.ObjectValue;
            }

            return Status.Success;
        }
    }
}
