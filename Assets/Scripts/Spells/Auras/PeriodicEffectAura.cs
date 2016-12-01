using System;
using UnityEngine;

public class PeriodicEffectAura : IAura
{
    float tickTime;
    float tickTimeLeft;

    public IEffect effect;

    public Buff Buff { get; set; }

    public PeriodicEffectAura(float newTickTime, IEffect newEffect, Buff buff)
    {
        Buff = buff;
        tickTime = newTickTime;
        tickTimeLeft = tickTime;
        effect = newEffect;//////!!!!!!!!!!!!!!!!!!!!!!!!!!!!! copy maybe
    }

    public void Apply(Unit unit)
    {
        unit.Character.PeriodicEffects.Add(this);
    }

    public void Reverse(Unit unit)
    {
        unit.Character.PeriodicEffects.Remove(this);
    }

    public void Update(Unit target, ArenaManager world)
    {
        tickTimeLeft -= Time.deltaTime;
        if (tickTimeLeft <= 0)
        {
            tickTimeLeft = tickTime + tickTimeLeft;
            world.ApplyEffect(Buff.caster, target, effect, Buff.spellId);
        }
    }

    public IAura Clone(Buff newBuff)
    {
        return new PeriodicEffectAura(tickTime, effect, newBuff);
    }
    public void Dispose()
    {
        Buff = null;
    }
}