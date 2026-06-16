using Client;
using Core;
using System;
using Unity.Behavior;
using Unity.Properties;
using Zenject;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShowBattleScreen", story: "Shows Battle Hud", category: "Action", id: "9f5ec9f65bcbf59669ceda66e5f1263d")]
public class ShowBattleScreenAction : BehaviourGraphAction
{
    [Inject]
    private InterfaceReference interfaceModule;

    protected override Status OnUpdate()
    {
        if (interfaceModule.IsPanelShown<BattleScreen, BattleHudPanel>())
            return Status.Success;

        interfaceModule.ShowScreen<BattleScreen, BattleHudPanel>();
        return Status.Success;
    }
}