using UnityEngine;
using System.Collections;

public class SpellScriptInfoArgs
{
    public int spellId;
    public int eventId;
    public int scriptId;

    public Unit caster;
    public Unit target;

    public SpellScriptInfoArgs()
    {

    }

    public SpellScriptInfoArgs(int newSpellId, int newEventId, int newScriptId,
        Unit newCaster, Unit newTarget)
    {
        spellId = newSpellId;
        eventId = newEventId;
        scriptId = newScriptId;

        caster = newCaster;
        target = newTarget;
    }

    public SpellScriptInfoArgs Clone()
    {
        return new SpellScriptInfoArgs(spellId, eventId, scriptId, caster, target);
    }

    public void Dispose()
    {
        caster = null;
        target = null;
    }
}
