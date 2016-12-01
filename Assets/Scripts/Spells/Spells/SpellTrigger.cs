using UnityEngine;

public class SpellTrigger
{
    public bool isTriggered;
    public float triggerChance;
    public int triggeredSpellId;
    public string spellName;

    public SpellTrigger(string newSpellName, float newTriggerChance, int newTriggeredSpellId)
    {
        spellName = newSpellName;
        triggerChance = newTriggerChance;
        triggeredSpellId = newTriggeredSpellId;
    }

    public bool CheckTrigger(Unit caster, Unit target, SpellInfo spellInformation, Spell spell)
    {
        isTriggered = RandomSolver.Next(0, 10000) < triggerChance * 100;
        return isTriggered;
    }
}