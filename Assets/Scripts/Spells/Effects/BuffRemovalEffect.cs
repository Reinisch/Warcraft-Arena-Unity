using System;
using System.Collections.Generic;
using UnityEngine;

public delegate bool RemovalCondition(Buff buff, Unit caster, Unit target, Spell spell, ArenaManager world);

public class BuffRemovalEffect : IEffect
{
    public RemovalCondition removalCondition;

    public AoeMode AoeMode { get; private set; }

    public BuffRemovalEffect(RemovalCondition newRemovalCondition)
    {
        removalCondition = newRemovalCondition;
        AoeMode = AoeMode.None;
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        target.Character.Buffs.RemoveOnCondition(removalCondition, caster, target, spell, world);
    }
}