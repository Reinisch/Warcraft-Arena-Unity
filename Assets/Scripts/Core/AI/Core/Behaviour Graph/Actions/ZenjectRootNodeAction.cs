using Core;
using Core.Scenario;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Zenject Root Node", story: "Ensures that tree is injected.", category: "Action", id: "77ec84c5cc870e0f7122679be7f7203f")]
public class ZenjectRootNodeAction : Action
{
    [SerializeReference]
    public BlackboardVariable<BehaviourTreeInjector> Container;

    protected override Status OnStart()
    {
        Parent.ExecuteRecursive(InjectNode);

        return Status.Success;
    }

    private void InjectNode(Node node) => Container.Value.DiContainer.Inject(node);
}

