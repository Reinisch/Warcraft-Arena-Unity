using System;
using UnityEngine;

public interface IEffect
{
    AoeMode AoeMode  { get; }
    void Apply(Unit caster, Unit target, Spell spell, ArenaManager world);
}