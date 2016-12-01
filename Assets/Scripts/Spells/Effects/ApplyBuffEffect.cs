using System;
using UnityEngine;

public class ApplyBuffEffect : IEffect
{
    public Buff Buff { get; set; }
    public AoeMode AoeMode { get; private set; }

    public ApplyBuffEffect(Buff buff)
    {
        Buff = buff;
        AoeMode = AoeMode.None;
    }

    public void Apply(Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        Buff.Apply(caster, target, world);
    }
}