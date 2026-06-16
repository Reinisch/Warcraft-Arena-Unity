using System;
using Unity.Behavior;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Failer", 
    description: "Always return failure.",
    icon: "Icons/failure",
    id: "d97f1ad9c3da63c9107284b3fca09990")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class FailerModifier : Modifier
{
    protected override Status OnStart()
    {
        if (Child == null)
        {
            return Status.Failure;
        }
        Status childStatus = StartNode(Child);
        return FailIfChildIsComplete(childStatus);
    }

    protected override Status OnUpdate()
    {
        return FailIfChildIsComplete(Child.CurrentStatus);
    }

    private Status FailIfChildIsComplete(Status childStatus)
    {
        if (childStatus is Status.Success or Status.Failure)
        {
            return Status.Failure;
        }
        return Status.Waiting;
    }
}

