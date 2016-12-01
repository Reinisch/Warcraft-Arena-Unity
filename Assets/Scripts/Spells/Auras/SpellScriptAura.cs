using UnityEngine;
using System.Collections;

public delegate void SpellScriptDelegate(SpellScriptInfoArgs scriptInfo, SpellScriptInvokeArgs spellArgs, params float[] args);

public class SpellScriptAura : IAura
{
    public int eventId;
    public int scriptId;

    public float[] scriptParams;

    public SpellScriptInfoArgs scriptInfo;

    public Buff Buff { get; set; }

    public SpellScriptAura(SpellScriptInfoArgs newScriptInfo, Buff newBuff, params float[] newScriptParams)
    {
        Buff = newBuff;
        eventId = newScriptInfo.eventId;
        scriptId = newScriptInfo.scriptId;
        scriptInfo = newScriptInfo;
        scriptParams = newScriptParams;
    }

    public void Apply(Unit unit)
    {
        unit.Character.CharacterEvents[eventId].Add(this);
    }
    public void Reverse(Unit unit)
    {
        unit.Character.CharacterEvents[eventId].Remove(this);
    }

    public void ApplyScript(SpellScriptInvokeArgs spellArgs)
    {
        spellArgs.world.SpellScripts[scriptId](scriptInfo, spellArgs, scriptParams);
    }

    public IAura Clone(Buff newBuff)
    {
        return new SpellScriptAura(scriptInfo.Clone(), newBuff, (float[])scriptParams.Clone());
    }

    public void Dispose()
    {
        Buff = null;
    }
}
