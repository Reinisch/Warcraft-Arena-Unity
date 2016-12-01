using System;
using UnityEngine;

public delegate bool ModifierApplier(Unit caster, Unit target, SpellInfo spellInfo, Spell spell, ArenaManager world, params float[] args);

public class SpellModifier
{
    public int applierId;
    public float[] modParams;

    public SpellModifier(int newApplierId, params float[] newModParams)
    {
        applierId = newApplierId;
        modParams = newModParams;
    }

    public bool ModifySpell(Unit caster, Unit target, SpellInfo spellInfo, Spell spell, ArenaManager world)
    {
        return world.SpellModifiers[applierId](caster, target, spellInfo, spell, world, modParams);
    }
}