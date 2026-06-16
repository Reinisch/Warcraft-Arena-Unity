using Client;
using Core;
using Core.Scenario;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "IsBattleScreenHidden", story: "Battle screen hidden with [Injector]", category: "Conditions", id: "1523f6a7b8c9111b2c3d4e5f10718293")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class IsBattleScreenHiddenCondition : BehaviourGraphCondition
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
        return !interfaceModule.IsScreenShown<BattleScreen>() ||
         !interfaceModule.IsPanelShown<BattleScreen, BattleHudPanel>();
    }
}
