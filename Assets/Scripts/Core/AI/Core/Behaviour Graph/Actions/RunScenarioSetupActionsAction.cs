using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace Core.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "RunScenarioSetupActions",
        story: "Execute all actions in scenario setup container.",
        category: "Action",
        id: "825162557f5af955f9cd31445091a9b9")]
    public partial class RunScenarioSetupActionsAction : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<ScenarioSetupContainer> ScenarioSetupContainer;

        protected override Status OnStart()
        {
            // Entity spawning is server-authoritative; clients receive units via replication. A client loads
            // its map scene-only (no scenario graph), so this normally won't run there — but guard anyway.
            if (World == null || !World.HasServerLogic)
                return Status.Success;

            foreach (ScenarioSetupAction action in ScenarioSetupContainer.Value.SetupActions)
            {
                switch(action)
                {
                    case SpawnCreature spawnCreature:
                        spawnCreature.Execute(World, ScenarioSetupContainer.Value.Map);
                        break;
                    case SpawnPlayerAI spawnPlayerAI:
                        spawnPlayerAI.Execute(World, ScenarioSetupContainer.Value.Map);
                        break;
                    default:
                        Debug.LogWarning($"Unsupported scenario setup action type: {action.GetType().Name}");
                        break;
                }
            }
            return Status.Success;
        }
    }
}
