using System;
using UnityEngine;

public class AoeEffect : IEffect
{
    public IEffect Effect { get; private set; }
    public AoeMode AoeMode { get; private set; }

    public float Radius { get; private set; } 

    public AoeEffect(IEffect baseEffect, AoeMode newAoeMode, float newRadius)
    {
        Effect = baseEffect;
        Radius = newRadius;
        AoeMode = newAoeMode;
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        Effect.Apply(caster, target, spell, world);
    }
}