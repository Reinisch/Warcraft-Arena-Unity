using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellStorage
{
    Dictionary<int, Spell> spells = new Dictionary<int, Spell>();

    public int Count { get { return spells.Count; } }

    public void AddSpell(Spell spell)
    {
        if (!spells.ContainsKey(spell.id))
            spells.Add(spell.id, spell);
    }

    public Dictionary<int, Spell> GetAll()
    {
        return spells;
    }

    public bool HasSpell(int id)
    {
        return spells.ContainsKey(id);
    }

    public Spell GetSpell(int id)
    {
        return spells[id];
    }

    public void Update()
    {
        foreach (KeyValuePair<int, Spell> spell in spells)
            spell.Value.spellCooldown.Update();
    }

    public void Dispose()
    {
        for (int i = 0; i < spells.Count; i++)
            spells.ElementAt(i).Value.Dispose();
        spells.Clear();
        spells = null;
    }
}

public class Spell
{
    public int id;
    public string name;
    public Cooldown spellCooldown;

    public List<SpellModifier> Modifiers { get; private set; }

    public Spell(int newSpellId, string newName, Cooldown newSpellCooldown)
    {
        id = newSpellId;
        name = newName;
        spellCooldown = newSpellCooldown;
        Modifiers = new List<SpellModifier>();
    }

    public void Dispose()
    {
        spellCooldown.Dispose();
        spellCooldown = null;
    }
}