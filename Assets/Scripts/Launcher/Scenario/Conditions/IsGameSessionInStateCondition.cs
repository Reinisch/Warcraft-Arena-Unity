using Client;
using Core;
using Core.Scenario;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "IsGameSessionInState", story: "Game session is [GameState] in [Injector]", category: "Conditions", id: "d4e5f6a7b8c90a1b2c3d4e5f60718293")]
// ReSharper disable once PartialTypeWithSinglePart
public partial class IsGameSessionInStateCondition : BehaviourGraphCondition
{
    [SerializeReference] public BlackboardVariable<BehaviourTreeInjector> Injector;
    [SerializeReference] public BlackboardVariable<GameSessionState> GameState;

    private GameSession session;
    private bool started;

    public override void OnStart()
    {
        base.OnStart();

        if (!started)
        {
            session = Injector.Value.DiContainer.Resolve<GameSession>();
            started = true;
        }
    }

    public override bool IsTrue()
    {
        return session.State == GameState.Value;
    }
}
