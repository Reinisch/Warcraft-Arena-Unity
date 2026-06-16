using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Core.Scenario.UnityBehavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Switch Default Movement Mode",
        description: "Sets the world's default movement mode.",
        story: "Set default movement mode to [MovementMode]",
        category: "Action/Map",
        id: "c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8")]
    public class SwitchDefaultMovementMode : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<MovementMode> MovementMode;

        protected override Status OnStart()
        {
            World.DefaultMovementMode = MovementMode.Value;
            return Status.Success;
        }
    }
}
