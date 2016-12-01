using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public enum SpellCastType { Instant, Casted }
public enum SpellTargetMode { Self, NoTarget, SingleTarget, ManualArea }
public enum SpellGlobalCooldownMode { Normal, NoCooldown }
public enum SpellProjectileEffect { Instant, TargetedProjectile }
public enum AoeMode { None, Self, Front, Splash }
public enum DamageType { Physical, Frost, Fire, Arcane, Nature, Shadow, Holy }
public enum BuffType { Buff, Debuff, Passive }
public enum AbilityType { Magic, Physical }
public enum BuffEffectType { None, StaticEffect }

[Flags]
public enum SpellFlags
{
    None = 0,
    PassiveSpell = 1,
    CastableWhileStunned = 2
}

public class SpellDataStorage
{
    Dictionary<int, SpellData> spellDictionary = new Dictionary<int, SpellData>();

    public int Count
    {
        get { return spellDictionary.Count; }
    }

    public bool HasSpell(int index)
    {
        return spellDictionary.ContainsKey(index);
    }

    public SpellData this[int index]
    {
        get { return spellDictionary.ElementAt(index).Value; }
    }

    public void AddSpellData(SpellData spellData)
    {
        if (!spellDictionary.ContainsKey(spellData.id))
            spellDictionary.Add(spellData.id, spellData);
    }

    public SpellData GetSpell(int id)
    {
        return spellDictionary[id];
    }
}

public class SpellData
{
    public int id;
    public string spellName;
    public string iconName;
    public string effectName;
    public string castSoundName;
    public string hitSoundName;
    public string spellProjectileName;

    public float baseCooldown;
    public float baseCastTime;
    public float range;
    public int activationCost;

    public SpellCastType spellCastType;
    public SpellTargetMode spellTargetMode;
    public SpellGlobalCooldownMode spellGlobal;
    public SpellProjectileEffect spellProjectileType;
    public SpellFlags flags;

    public List<IEffect> Effects;
    public List<SpellModifier> Modifiers;

    public SpellData()
    {
        Effects = new List<IEffect>();
        Modifiers = new List<SpellModifier>();
        flags = SpellFlags.None;
    }
}