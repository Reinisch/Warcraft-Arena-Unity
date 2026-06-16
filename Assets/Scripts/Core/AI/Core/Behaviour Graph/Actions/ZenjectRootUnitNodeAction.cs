using Core;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ZenjectRootUnitNodeAction", story: "Ensure Unit Graph Injected", category: "Action", id: "f70edac1769a5a39e8ffa1312fe655f8")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class ZenjectRootUnitNodeAction : Action
{
    [SerializeReference]
    public BlackboardVariable<UnitAI> UnitAI;

    protected override Status OnStart()
    {
        Parent.ExecuteRecursive(InjectNode);

        return Status.Success;
    }

    private void InjectNode(Node node) => UnitAI.Value.DiContainer.Inject(node);
}

