using System;
using UnityEngine;

public class HealEffect : IEffect
{
    int minValue;
    int maxValue;
    int modifier;

    public AoeMode AoeMode { get; private set; }

    public HealEffect(int MinValue, int MaxValue, int Modifier)
    {
        modifier = Modifier;
        minValue = MinValue;
        maxValue = MaxValue;
        AoeMode = AoeMode.None;
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        int amount = modifier;

        amount += RandomSolver.Next(minValue, maxValue);

        if (amount < 1)
            amount = 1;

        target.Character.health.Increase((ushort)amount);
    }
}