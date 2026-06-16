using Client;
using Core;
using Core.Scenario;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "IsLobbyScreenHidden", story: "Lobby screen hidden with [Injector]", category: "Conditions", id: "d4e5f6a7b8c9222b2c3d4e5f11234293")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class IsLobbyScreenHiddenCondition : BehaviourGraphCondition
{
    [SerializeReference] public BlackboardVariable<BehaviourTreeInjector> Injector;

    private InterfaceReference interfaceModule;
    private bool started;

    public override void OnStart()
    {
        base.OnStart();

        if (!started)
        {
            interfaceModule = Injector.Value.DiContainer.Resolve<InterfaceReference>();
            started = true;
        }
    }

    public override bool IsTrue()
    {
        return !interfaceModule.IsScreenShown<LobbyScreen>() ||
         !interfaceModule.IsPanelShown<LobbyScreen, LobbyPanel>();
    }
}
