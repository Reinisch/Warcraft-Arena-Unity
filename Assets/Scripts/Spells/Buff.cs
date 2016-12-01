using System;
using System.Collections.Generic;
using UnityEngine;

public class Buffs : IDisposable
{
    private Unit unit;
    private List<Buff> buffs;

    public Buffs(Unit newUnit)
    {
        unit = newUnit;
        buffs = new List<Buff>();
    }

    public int Count
    {
        get { return buffs.Count; }
    }

    public void Add(Buff buff)
    {
        foreach (IAura aura in buff.Auras)
            aura.Apply(unit);
        buffs.Add(buff);
    }

    public Buff Find(Predicate<Buff> match)
    {
        return buffs.Find(match);
    }

    public bool Contains(Buff item)
    {
        return buffs.Contains(item);
    }

    public bool Remove(Buff buff)
    {
        if (buffs.Remove(buff))
        {
            foreach (IAura aura in buff.Auras)
                aura.Reverse(unit);
            
            buff.Dispose();
            return true;
        }
        return false;
    }

    public void RemoveAllTheSame(int id)
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].id == id)
            {
                for(int j = 0; j < buffs[i].Auras.Count; j++)
                    buffs[i].Auras[j].Reverse(unit);

                buffs[i].Dispose();
                buffs.RemoveAt(i);
            }
        }
    }

    public void RemoveOnCondition(RemovalCondition condition, Unit caster, Unit target, Spell spell, ArenaManager world)
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (condition(buffs[i], caster, target, spell, world))
            {
                for (int j = 0; j < buffs[i].Auras.Count; j++)
                    buffs[i].Auras[j].Reverse(unit);

                buffs[i].Dispose();
                buffs.RemoveAt(i);
            }
        }
    }

    public void RemoveCastersAllTheSame(int id, Unit caster)
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].id == id && buffs[i].caster.id == caster.id)
            {
                for (int j = 0; j < buffs[i].Auras.Count; j++)
                    buffs[i].Auras[j].Reverse(unit);

                buffs[i].Dispose();
                buffs.RemoveAt(i);
            }
        }
    }

    public void Update(ArenaManager world)
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].Update())
            {
                for (int k = 0; k < unit.Character.CharacterEvents[CharacterEventTypes.OnBuffExpired].Count; k++)
                {
                    if(unit.Character.CharacterEvents[CharacterEventTypes.OnBuffExpired][k].Buff == buffs[i])
                        unit.Character.CharacterEvents[CharacterEventTypes.OnBuffExpired][k].ApplyScript(
                            new SpellScriptInvokeArgs(0, buffs[i].caster, unit, world, null, null));
                }

                for (int j = 0; j < buffs[i].Auras.Count; j++)
                    buffs[i].Auras[j].Reverse(unit);

                buffs[i].Dispose();
                buffs.RemoveAt(i);
            }
        }
    }

    public Buff this[int index]
    {
        get { return buffs[index]; }
    }

    public void Dispose()
    {
        unit = null;
        for(int i = 1; i < buffs.Count; i ++)
            buffs[i].Dispose();
    }
}

public class Buff
{
    public int id;
    public int spellId;

    public string name;
    public string iconName;
    public int currentStucks;
    public int maxStucks;

    public bool stackSameCaster;
    public bool stackWithAllCasters;

    public AbilityType magicType;
    public BuffType buffType;
    public AoeMode aoeMode;

    public BuffEffectType buffEffectType;
    public string buffEffectName;

    public float radius;

    public Unit caster;

    public float duration;
    public float timeLeft;

    public bool isDisposed;

    public List<IAura> Auras { get; private set; }

    public Buff(int newId, int newSpellId, string newName, string newIconName, BuffType newBuffType, AbilityType newMagicType,
        float newDuration, int newCurrentStucks, int newMaxStucks,
        bool isStuckableSameCaster, bool isStuckableWithAllCasters)
    {
        id = newId;
        spellId = newSpellId;
        currentStucks = newCurrentStucks;
        maxStucks = newMaxStucks;
        name = newName;
        iconName = newIconName;

        buffType = newBuffType;

        duration = newDuration;
        timeLeft = duration;

        stackSameCaster = isStuckableSameCaster;
        stackWithAllCasters = isStuckableWithAllCasters;

        isDisposed = false;
        Auras = new List<IAura>();
    }

    public void Apply(Unit caster, Unit target, ArenaManager world)
    {
        if (!stackWithAllCasters)
            target.Character.Buffs.RemoveAllTheSame(id);

        if (!stackSameCaster)
            target.Character.Buffs.RemoveCastersAllTheSame(id, caster);

        Buff newBuff = Clone(caster);
        if (newBuff.buffEffectType == BuffEffectType.StaticEffect)
        {
            GameObject effect = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Spells/" + buffEffectName),
                target.transform.position, target.transform.rotation) as GameObject;
            effect.GetComponent<BuffBindedEffect>().Initialize(target, newBuff);
        }
        target.Character.Buffs.Add(newBuff);
    }
    public bool Update()
    {
        if (duration > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timeLeft = 0;
                return true;
            }

        }
        return false;
    }

    public Buff Clone(Unit newCaster)
    {
        Buff clone = new Buff(id, spellId, name, iconName, buffType, magicType, duration, currentStucks, maxStucks, stackSameCaster, stackWithAllCasters);
        for (int i = 0; i < Auras.Count; i++)
        {
            clone.Auras.Add(Auras[i].Clone(clone));
        }
        clone.buffEffectType = buffEffectType;
        clone.buffEffectName = buffEffectName;
        clone.caster = newCaster;
        return clone;
    }
    public void Dispose()
    {
        caster = null;
        isDisposed = true;
        for (int i = 0; i < Auras.Count; i++)
        {
            Auras[i].Dispose();
        }
    }
}