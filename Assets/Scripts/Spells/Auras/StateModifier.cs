using System;
using UnityEngine;

public class StateModifier : IAura
{
    public int stateId;

    public StateModifier(int newStateId)
    {
        stateId = newStateId;
    }

    public void Apply(Unit unit)
    {
        unit.Character.states[stateId].stateEffectCount++;
    }
    public void Reverse(Unit unit)
    {
        unit.Character.states[stateId].stateEffectCount--;
    }

    public IAura Clone(Buff newBuff)
    {
        return new StateModifier(stateId);
    }
    public void Dispose()
    {

    }
}