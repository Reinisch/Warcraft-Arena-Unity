using System;
using System.Collections.Generic;
using UnityEngine;

public static class ParameterType
{
    public static readonly int Strength = 0;
    public static readonly int Agility = 1;
    public static readonly int Intelligence = 2;
    public static readonly int Stamina = 3;
    public static readonly int Haste = 4;
    public static readonly int CritChance = 5;
    public static readonly int CritDamageMultiplier = 6;
    public static readonly int AttackPower = 7;
    public static readonly int SpellPower = 8;
    public static readonly int Speed = 9;
    public static readonly int DamageDealing = 10;
    public static readonly int DamageTaken = 11;
    public static readonly int Count = 12;
}

public static class ParameterValue
{
    public const int BaseValue = 0;
    public const int BaseBonusAddition = 1;
    public const int BasePenaltyAddition = 2;
    public const int BonusMultiplier = 3;
    public const int PenaltyMultiplier = 4;
    public const int FinalBonusAddition = 5;
    public const int FinalPenaltyAddition = 6;
    public const int Count = 7;
}

public static class EntityStateType
{
    public static readonly int Root = 0;
    public static readonly int Disarm = 1;
    public static readonly int Stun = 2;
    public static readonly int Snare = 3;
    public static readonly int Freeze = 4;
    public static readonly int Disorient = 5;
    public static readonly int Fear = 6;
    public static readonly int Sleep = 7;
    public static readonly int Silence = 8;
    public static readonly int Knockback = 9;
    public static readonly int Invulnerability = 10;
    public static readonly int Pacified = 11;
    public static readonly int Invisible = 12;
    public static readonly int ModelChange = 13;
    public static readonly int Count = 14;
}

[Serializable]
public class EntityState
{
    public int stateEffectCount;
    public bool InEffect { get { return stateEffectCount > 0; } }
}

[Serializable]
public class Parameter
{
    public float[] parameterValues = new float[ParameterValue.Count];

    public float BaseValue
    {
        get { return parameterValues[0]; }
    }

    public float FinalValue
    {
        get { return (parameterValues[0] + parameterValues[1] + parameterValues[2]) * parameterValues[3] * parameterValues[4] + parameterValues[5] + parameterValues[6]; }
    }

    public float NoPenaltyValue
    {
        get { return (parameterValues[0] + parameterValues[1]) * parameterValues[3] + parameterValues[5]; }
    }

    public float NoBonusValue
    {
        get { return (parameterValues[0] + parameterValues[2]) * parameterValues[4] + parameterValues[6]; }
    }

    public float this[int index]
    {
        get { return parameterValues[index]; }
        set { parameterValues[index] = value; }
    }
}

[Serializable]
public class EntityStatesList
{
    public EntityState[] entityStates = new EntityState[EntityStateType.Count];

    public EntityState this[int index]
    {
        get { return entityStates[index]; }
        private set { entityStates[index] = value; }
    }
}

[Serializable]
public class ParametersList
{
    public Parameter[] parameters = new Parameter[ParameterType.Count];

    public ParametersList()
    {
        parameters = new Parameter[ParameterType.Count];
        for (int i = 0; i < ParameterType.Count; i++)
            parameters[i] = new Parameter();
    }

    public Parameter this[int index]
    {
        get { return parameters[index]; }
        private set { parameters[index] = value; }
    }

    public void Copy(ParametersList newList)
    {
        for (int i = 0; i < ParameterType.Count; i++)
        {
            for (int j = 0; j < ParameterValue.Count; j++)
            {
                newList.parameters[i][j] = parameters[i][j];
            }
        }
    }
}