using Client;
using Core;
using System;
using Unity.Behavior;
using Unity.Properties;
using Zenject;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HideLobbyScreen", story: "Hides Lobby Screen", category: "Action", id: "9f5ec1222bcbf59669ceda66e5f1263d")]
public class HideLobbyScreenAction : BehaviourGraphAction
{
    [Inject]
    private InterfaceReference interfaceModule;

    protected override Status OnUpdate()
    {
        if (!interfaceModule.IsScreenShown<LobbyScreen>())
            return Status.Success;

        interfaceModule.HideScreen<LobbyScreen>();
        return Status.Success;
    }
}