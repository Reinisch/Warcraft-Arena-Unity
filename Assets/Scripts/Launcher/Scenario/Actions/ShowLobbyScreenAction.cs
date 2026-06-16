using Client;
using Core;
using System;
using Unity.Behavior;
using Unity.Properties;
using Zenject;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShowLobbyScreen", story: "Shows Lobby Screen", category: "Action", id: "9f5ec9f65bcbf23663ceda66e5f1263d")]
public class ShowLobbyScreenAction : BehaviourGraphAction
{
    [Inject]
    private InterfaceReference interfaceModule;

    protected override Status OnUpdate()
    {
        if (interfaceModule.IsPanelShown<LobbyScreen, LobbyPanel>())
            return Status.Success;

        interfaceModule.ShowScreen<LobbyScreen, LobbyPanel>();
        return Status.Success;
    }
}