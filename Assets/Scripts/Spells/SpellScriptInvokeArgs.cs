using UnityEngine;
using System.Collections;

public class SpellScriptInvokeArgs
{
    public int spellId;

    public Unit caster;
    public Unit target;
    public ArenaManager world;
    public Spell spell;
    public SpellInfo spellInfo;

    public SpellScriptInvokeArgs()
    {

    }

    public SpellScriptInvokeArgs(int newSpellId, Unit newCaster, Unit newTarget,
        ArenaManager newWorld, Spell newSpell, SpellInfo newSpellInfo)
    {
        spellId = newSpellId;
        caster = newCaster;
        target = newTarget;
        world = newWorld;
        spell = newSpell;
        spellInfo = newSpellInfo;
    }

    public SpellScriptInvokeArgs Clone()
    {
        return new SpellScriptInvokeArgs(spellId, caster, target, world, spell, spellInfo);
    }

    public void Dispose()
    {
        caster = null;
        target = null;
        world = null;
        spell = null;
        spellInfo = null;
    }
}
