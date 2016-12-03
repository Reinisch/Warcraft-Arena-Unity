using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
    Dictionary<BaseStatType, Stat> baseStats = new Dictionary<BaseStatType, Stat>();
    Dictionary<CombatRating, float> baseRatings = new Dictionary<CombatRating, float>();

    int baseSpellCritChance;
    int baseSpellPower;
    int baseManaRegen;
    int baseHealthRegen;

    public UnitState UnitState { get; set; }
    public DeathState DeathState { get; set; }
    public UnitMoveType MoveType { get; set; }

    public Unit target;
    public string className;

    public ParametersList parameters;
    public EntityStatesList states;

    public AttributePair health;
    public AttributePair mainResourse;

    public Cooldown GlobalCooldown { get; private set; }
    public SpellCast SpellCast { get; set; }

    public Buffs Buffs { get; private set; }
    public SpellStorage Spells { get; private set; }

    public Dictionary<int, List<SpellScriptAura>> CharacterEvents;
    public List<PeriodicEffectAura> PeriodicEffects;
    public List<AbsorbEffect> AbsorbEffects;
    public List<long> PreviousTargets;

    public void Initialize(Unit unit)
    {
        if (unit.IsHumanPlayer)
            StatSystem.InitializePlayerStats(stats);
        else
            StatSystem.InitializeCreatureStats(stats);

        GlobalCooldown = new Cooldown(1);
        SpellCast = new SpellCast();

        Buffs = new Buffs(unit);
        Spells = new SpellStorage();

        PreviousTargets = new List<long>();
        PeriodicEffects = new List<PeriodicEffectAura>();
        AbsorbEffects = new List<AbsorbEffect>();
        CharacterEvents = new Dictionary<int, List<SpellScriptAura>>();
        for (int i = 0; i < CharacterEventTypes.Count; i++)
            CharacterEvents.Add(i, new List<SpellScriptAura>());
    }

    public void Update(Unit unit, ArenaManager world)
    {
        SpellCast.Update(world);
        Spells.Update();
        for (int i = 0; i < PeriodicEffects.Count; i++)
            PeriodicEffects[i].Update(unit, world);
        Buffs.Update(world);
        GlobalCooldown.Update();
    }

    public void Dispose()
    {
        target = null;

        health = null;
        mainResourse = null;
        parameters = null;
        states = null;

        GlobalCooldown.Dispose();
        GlobalCooldown = null;

        SpellCast.Dispose();
        SpellCast = null;

        Buffs.Dispose();
        Buffs = null;

        Spells.Dispose();
        Spells = null;

        AbsorbEffects.Clear();
        AbsorbEffects = null;

        PeriodicEffects.Clear();
        PeriodicEffects = null;

        PreviousTargets.Clear();
        PreviousTargets = null;

        for (int i = 0; i < CharacterEventTypes.Count; i++)
            CharacterEvents[i].Clear();

        CharacterEvents.Clear();
        CharacterEvents = null;
    }
}