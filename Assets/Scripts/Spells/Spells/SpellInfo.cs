using UnityEngine;

public class SpellInfo
{
    public int spellId;
    public string spellName;
    public ParametersList parameters;   

    public SpellInfo(int newSpellId, string newSpellName)
    {
        spellId = newSpellId;
        spellName = newSpellName;
        parameters = new ParametersList();
    }
}