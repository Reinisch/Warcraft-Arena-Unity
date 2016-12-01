using System;
using UnityEngine;

public class ParameterModifier : IAura
{
    public int parameterId;
    public int parameterFieldId;
    public float value;

    public ParameterModifier(int newParamId, int newParamFieldId, float newValue)
    {
        parameterId = newParamId;
        parameterFieldId = newParamFieldId;
        value = newValue;
    }

    public void Apply(Unit unit)
    {
        switch (parameterFieldId)
        {
            case ParameterValue.BaseBonusAddition:
            case ParameterValue.BasePenaltyAddition:
            case ParameterValue.BaseValue:
            case ParameterValue.FinalBonusAddition:
            case ParameterValue.FinalPenaltyAddition:
                unit.Character.parameters[parameterId][parameterFieldId] += value;
                break;
            case ParameterValue.BonusMultiplier:
            case ParameterValue.PenaltyMultiplier:
                unit.Character.parameters[parameterId][parameterFieldId] *= value;
                break;
        }
    }
    public void Reverse(Unit unit)
    {
        switch(parameterFieldId)
        {
            case ParameterValue.BaseBonusAddition:
            case ParameterValue.BasePenaltyAddition:
            case ParameterValue.BaseValue:
            case ParameterValue.FinalBonusAddition:
            case ParameterValue.FinalPenaltyAddition:
                unit.Character.parameters[parameterId][parameterFieldId] -= value;
                break;
            case ParameterValue.BonusMultiplier:
            case ParameterValue.PenaltyMultiplier:
                unit.Character.parameters[parameterId][parameterFieldId] /= value;
                break;
        }
    }

    public IAura Clone(Buff newBuff)
    {
        return new ParameterModifier(parameterId, parameterFieldId, value);
    }
    public void Dispose()
    {

    }
}