using System;
using UnityEngine;

[Serializable]
public class SpellCast
{
    private bool needsTarget;
    public Spell spell;
    public Unit caster;
    public Unit target;

    public float castTimeLeft;
    public float castTime;

    public bool HasCast { get { return castTimeLeft > 0; } }

    public SpellCast()
    {
    }

    public SpellCast(Spell newSpell, SpellData spellData, Unit newCaster, Unit newTarget)
    {
        spell = newSpell;
        caster = newCaster;
        target = newTarget;
        needsTarget = target == null ? false : true;
        castTime = spellData.baseCastTime/(1+(caster.Character.parameters[ParameterType.Haste].FinalValue)/100);
        castTimeLeft = castTime;
    }

    public bool CheckInterrupt()
    {
        if (needsTarget)
        {
            if (target == null)
                return true;
        }
        return false;
    }

    public bool Update(ArenaManager world)
    {
        if (!HasCast)
            return true;
        castTimeLeft -= Time.deltaTime;
        if (castTimeLeft <= 0)
        {
            world.SpellCastFinished(this);
            Dispose();
            castTimeLeft = 0;
            return false;
        }
        
        return true;
    }

    public void Dispose()
    {
        spell = null;
        caster = null;
        target = null;
    }
}