using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();                    // standard non spell modifiers
    Dictionary<BaseStatType, float> baseStats = new Dictionary<BaseStatType, float>();      // base character stats : strength, agility, intellect, stamina
    Dictionary<CombatRating, float> baseRatings = new Dictionary<CombatRating, float>();    // all ratings : crit, haste, dodge...
    Dictionary<UnitMoveType, float> speedRates = new Dictionary<UnitMoveType, float>();     // movement rates: run, back, turn

    bool isPlayerControlled;

    float baseSpellCritChance;
    int baseSpellPower;
    int baseManaRegen;
    int baseHealthRegen;

    public UnitState UnitState { get; set; }
    public DeathState DeathState { get; set; }
    public UnitMoveType MoveType { get; set; }

    #region Deprecated
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

    #endregion

    public void Initialize(Unit unit)
    {
        isPlayerControlled = unit.IsHumanPlayer;

        if (unit.IsHumanPlayer)
            StatSystem.InitializePlayerStats(stats);
        else
            StatSystem.InitializeCreatureStats(stats);

        StatSystem.InitializeSpeedRates(speedRates);

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

    #region Speed Functions

    public void UpdateSpeed(UnitMoveType type)
    {
        int main_speed_mod = 0;
        float stack_bonus = 1.0f;
        float non_stack_bonus = 1.0f;

        switch (type)
        {
            // only apply debuffs
            case UnitMoveType.RunBack:
                break;
            case UnitMoveType.Run:
                main_speed_mod = /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_INCREASE_SPEED)*/0;
                stack_bonus = /*GetTotalAuraMultiplier(SPELL_AURA_MOD_SPEED_ALWAYS)*/0;
                non_stack_bonus += /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_SPEED_NOT_STACK) / 100.0f*/0;
                break;
            default:
                Debug.LogErrorFormat("Character::UpdateSpeed: Unsupported move type - {0}", type);
                return;
        }

        // now we ready for speed calculation
        float speed = Mathf.Max(non_stack_bonus, stack_bonus);
        if (main_speed_mod != 0)
            speed *= main_speed_mod;

        switch (type)
        {
            case UnitMoveType.Run:
                // Normalize speed by 191 aura SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED if need #TODO
                int normalization/* = GetMaxPositiveAuraModifier(SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED)*/ = 0;
                if (normalization > 0)
                {
                    // Use speed from aura
                    float max_speed = normalization / StatSystem.BaseMovementSpeed(type);
                    if (speed > max_speed)
                        speed = max_speed;
                }

                // force minimum speed rate @ aura 437 SPELL_AURA_MOD_MINIMUM_SPEED_RATE
                int minSpeedModRate = /*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_MINIMUM_SPEED_RATE)*/0;
                if (minSpeedModRate != 0)
                    {
                    float minSpeed = minSpeedModRate / StatSystem.BaseMovementSpeed(type);
                    if (speed < minSpeed)
                        speed = minSpeed;
                }
                break;
            default:
                break;
        }

        // Apply strongest slow aura mod to speed
        int slow = /*GetMaxNegativeAuraModifier(SPELL_AURA_MOD_DECREASE_SPEED)*/0;
        if (slow != 0)
            speed *= slow;

        float minSpeedMod = (float)/*GetMaxPositiveAuraModifier(SPELL_AURA_MOD_MINIMUM_SPEED)*/0;
        if (minSpeedMod > 0)
        {
            float min_speed = minSpeedMod / 100.0f;
            if (speed < min_speed)
                speed = min_speed;
        }

        SetSpeedRate(type, speed);
    }
    
    public float GetSpeed(UnitMoveType type)
    {
        return speedRates[type] * StatSystem.BaseMovementSpeed(type);
    }

    public float GetSpeedRate(UnitMoveType type)
    {
        return speedRates[type];
    }

    public void SetSpeed(UnitMoveType type, float newValue)
    {
        SetSpeedRate(type, newValue / StatSystem.BaseMovementSpeed(type));
    }

    public void SetSpeedRate(UnitMoveType type, float rate)
    { 
         if (rate < 0)
            rate = 0.0f;

        if (speedRates[type] == rate)
            return;

        speedRates[type] = rate;
    }

    #endregion

    #region Ownership Functions

    public bool IsPlayerControlled()
    {
        return isPlayerControlled;
    }

    #endregion
}