using Client;
using Core;
using System;
using Unity.Behavior;
using Unity.Properties;
using Zenject;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HideBattleScreen", story: "Hides Battle Hud", category: "Action", id: "9f5ec1111bcbf59669ceda66e5f1263d")]
public class HideBattleScreenAction : BehaviourGraphAction
{
    [Inject]
    private InterfaceReference interfaceModule;

    protected override Status OnUpdate()
    {
        if (!interfaceModule.IsScreenShown<BattleScreen>())
            return Status.Success;

        interfaceModule.HideScreen<BattleScreen>();
        return Status.Success;
    }
}