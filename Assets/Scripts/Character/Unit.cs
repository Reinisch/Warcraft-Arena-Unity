using System;
using UnityEngine;
using System.Collections.Generic;

public enum Team { Red, Blue }

public class Unit : MonoBehaviour
{
    public int id;
    public string unitName;
    public bool isHumanPlayer;
    public bool isDead;
    public bool isGrounded;
    public Character character;
    public Transform castPoint;
    public Transform centerPoint;
    public Team team;

    public float moveTimer = 0;
    public float castTimer = 0;

    public int Id 
    {
        get { return id; }
        set { id = value; }
    }
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    private CapsuleCollider unitCollider;
    private Animator animator;

    public bool IsHumanPlayer
    { 
        get { return isHumanPlayer; }
        set { isHumanPlayer = value; }
    }
    public bool IsGrounded 
    {
        get { return isGrounded; }
        set
        {
            isGrounded = value;
        }
    }
    public bool IsMovementBlocked
    {
        get
        {
            return Character.states[EntityStateType.Root].InEffect || Character.states[EntityStateType.Stun].InEffect;
        }
    }

    public CapsuleCollider UnitCollider { get { return unitCollider; } }
    public Character Character { get { return character; } }
    public Animator Animator { get { return animator; } }

    void Awake()
    {
        animator = GetComponent<Animator>();
        unitCollider = GetComponent<CapsuleCollider>();

        Character.Initialize(this);
    }

    void Update()
    {
        UpdateUnit(ArenaManager.Instanse);
    }

    public void UpdateUnit(ArenaManager world)
    {
        if (moveTimer > 0)
            moveTimer -= Time.deltaTime;

        Character.Update(this, world);
    }

    public void TriggerInstantCast()
    {
        // Switch leg animation for casting
        if (animator.GetBool("Grounded"))
        {
            if (animator.GetFloat("Speed") > 0.1f)
                animator.Play("Run", 1);
            else
                animator.Play("Cast", 1);
        }
        else
            animator.Play("Air", 1);
    }

    #region Relations

    public bool IsHostileTo(Unit unit)
    {
        return team != unit.team;
    }

    public bool IsFriendlyTo(Unit unit)
    {
        return team == unit.team;
    }

    #endregion

    #region State Info

    public bool IsAlive() { return (Character.DeathState == DeathState.Alive); }

    public bool IsDying() { return (Character.DeathState == DeathState.JustDied); }

    public bool IsDead() { return (Character.DeathState == DeathState.Dead || Character.DeathState == DeathState.Corpse); }

    #endregion

    #region Spells

    public SpellMissInfo SpellHitResult(Unit victim, TrinitySpellInfo spellInfo, bool canReflect)
    {
        // Check for immune
        /*if (victim->IsImmunedToSpell(spellInfo))
            return SPELL_MISS_IMMUNE;*/

        // All positive spells can`t miss
        if (spellInfo.IsPositive() && !IsHostileTo(victim)) // prevent from affecting enemy by "positive" spell
            return SpellMissInfo.None;

        // Check for immune
        /*if (victim->IsImmunedToDamage(spellInfo))
            return SPELL_MISS_IMMUNE;*/

        if (this == victim)
            return SpellMissInfo.None;

        // Try victim reflect spell
        /*if (CanReflect)
        {
            int32 reflectchance = victim->GetTotalAuraModifier(SPELL_AURA_REFLECT_SPELLS);
                Unit::AuraEffectList const& mReflectSpellsSchool = victim->GetAuraEffectsByType(SPELL_AURA_REFLECT_SPELLS_SCHOOL);
            for (Unit::AuraEffectList::const_iterator i = mReflectSpellsSchool.begin(); i != mReflectSpellsSchool.end(); ++i)
                if ((*i)->GetMiscValue() & spellInfo->GetSchoolMask())
                    reflectchance += (*i)->GetAmount();
            if (reflectchance > 0 && roll_chance_i(reflectchance))
            {
                // Start triggers for remove charges if need (trigger only for victim, and mark as active spell)
                ProcDamageAndSpell(victim, PROC_FLAG_NONE, PROC_FLAG_TAKEN_SPELL_MAGIC_DMG_CLASS_NEG, PROC_EX_REFLECT, 1, BASE_ATTACK, spellInfo);
                return SPELL_MISS_REFLECT;
            }
        }*/

        /*switch (spellInfo->DmgClass)
        {
            case SPELL_DAMAGE_CLASS_RANGED:
            case SPELL_DAMAGE_CLASS_MELEE:
                return MeleeSpellHitResult(victim, spellInfo);
            case SPELL_DAMAGE_CLASS_NONE:
                return SPELL_MISS_NONE;
            case SPELL_DAMAGE_CLASS_MAGIC:
                return MagicSpellHitResult(victim, spellInfo);
        }*/
        return SpellMissInfo.None;
    }


    public float GetSpellMinRangeForTarget(Unit target, TrinitySpellInfo spellInfo)
    {
        if (spellInfo.Range == null)
            return 0;
        if (spellInfo.Range.MinRangeFriend == spellInfo.Range.MinRangeHostile)
            return spellInfo.GetMinRange(false);
        if (target != null)
            return spellInfo.GetMinRange(true);
        return spellInfo.GetMinRange(!IsHostileTo(target));
    }

    public float GetSpellMaxRangeForTarget(Unit target, TrinitySpellInfo spellInfo)
    {
        if (spellInfo.Range == null)
            return 0;
        if (spellInfo.Range.MaxRangeFriend == spellInfo.Range.MaxRangeHostile)
            return spellInfo.GetMaxRange(false);
        if (!target)
            return spellInfo.GetMaxRange(true);
        return spellInfo.GetMaxRange(!IsHostileTo(target));
    }


    public void CastSpell(SpellCastTargets targets, TrinitySpellInfo spellInfo, TriggerCastFlags triggerFlags, AuraEffect triggeredByAura, Guid originalCaster)
    {
        if (spellInfo == null)
        {
            Debug.LogError("Unknown spell for unit: " + gameObject.name);
            return;
        }

        TrinitySpell spell = new TrinitySpell(this, spellInfo, triggerFlags, originalCaster);
        spell.Prepare(targets, triggeredByAura);
    }

    public void CastSpell(Unit target, int spellId, bool triggered, AuraEffect triggeredByAura, Guid originalCaster)
    {
        CastSpell(target, spellId, triggered ? TriggerCastFlags.FullMask : TriggerCastFlags.None, triggeredByAura, originalCaster);
    }

    public void CastSpell(Unit target, int spellId, TriggerCastFlags triggerFlags, AuraEffect triggeredByAura, Guid originalCaster)
    {
        TrinitySpellInfo spellInfo = WarcraftDatabase.SpellInfos.ContainsKey(spellId) ? WarcraftDatabase.SpellInfos[spellId] : null;
        if (spellInfo == null)
        {
            Debug.LogError("Unknown spell for unit: " + gameObject.name);
            return;
        }

        CastSpell(target, spellInfo, triggerFlags, triggeredByAura, originalCaster);
    }

    public void CastSpell(Unit target, TrinitySpellInfo spellInfo, bool triggered, AuraEffect triggeredByAura, Guid originalCaster)
    {
        CastSpell(target, spellInfo, triggered ? TriggerCastFlags.FullMask : TriggerCastFlags.None, triggeredByAura, originalCaster);
    }

    public void CastSpell(Unit target, TrinitySpellInfo spellInfo, TriggerCastFlags triggerFlags, AuraEffect triggeredByAura, Guid originalCaster)
    {
        SpellCastTargets targets = new SpellCastTargets();
        targets.UnitTarget = target;
        CastSpell(targets, spellInfo, triggerFlags, triggeredByAura, originalCaster);
    }


    public int CalculateSpellDamage(Unit target, TrinitySpellInfo spellProto, int effectIndex, int basePoints, float variance)
    {
        TrinitySpellEffectInfo effect = spellProto.GetEffect(effectIndex);
        if (variance > 0)
            variance = 0.0f;

        return effect != null? effect.CalcValue(this, basePoints, target, variance) : 0;
    }

    public float ApplyEffectModifiers(TrinitySpellInfo spellProto, int effectIndex, float value)
    {
        var modOwner = this;
        /*modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_ALL_EFFECTS, value);
        switch (effect_index)
        {
            case 0:
                modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT1, value);
                break;
            case 1:
                modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT2, value);
                break;
            case 2:
                modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT3, value);
                break;
            case 3:
                modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT4, value);
                break;
            case 4:
                modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT5, value);
                break;
        }*/
        return value;
    }

    public void CalculateSpellDamageTaken(SpellDamage damageInfo, int damage, TrinitySpellInfo spellInfo, bool crit)
    {
        if (damage < 0)
            return;

        Unit victim = damageInfo.Target;
        if (victim == null || !victim.IsAlive())
            return;

        SpellSchoolMask damageSchoolMask = damageInfo.SchoolMask;

        // Script Hook For CalculateSpellDamageTaken -- Allow scripts to change the Damage post class mitigation calculations
        //sScriptMgr->ModifySpellDamageTaken(damageInfo->target, damageInfo->attacker, damage);

        // Calculate absorb resist
        /*if (damage > 0)
        {
            CalcAbsorbResist(victim, damageSchoolMask, SPELL_DIRECT_DAMAGE, damage, &damageInfo->absorb, &damageInfo->resist, spellInfo);
            damage -= damageInfo->absorb + damageInfo->resist;
        }
        else
            damage = 0;*/

        damageInfo.Damage = damage;
        damageInfo.Crit = crit;
    }

    public void DealSpellDamage(SpellDamage damageInfo)
    {
        if (damageInfo == null)
            return;

        Unit victim = damageInfo.Target;

        if (victim == null)
            return;

        if (!victim.IsAlive())
            return;

        TrinitySpellInfo spellProto = WarcraftDatabase.SpellInfos.ContainsKey(damageInfo.SpellID) ? WarcraftDatabase.SpellInfos[damageInfo.SpellID] : null;
        if (spellProto == null)
        {
            Debug.LogErrorFormat("Unit.DealSpellDamage has wrong damageInfo->SpellID: {0}", damageInfo.SpellID);
            return;
        }

        // Call default DealDamage
        CleanDamage cleanDamage = new CleanDamage(damageInfo.Absorb, damageInfo.Damage);
        DealDamage(victim, damageInfo.Damage, cleanDamage, DamageEffectType.SpellDirectDamage, damageInfo.SchoolMask, spellProto);

        SpellManager.SpellDamageEvent(this, victim, damageInfo.Damage, damageInfo.Crit);
    }

    public void Kill(Unit victim)
    {
        // Prevent killing unit twice (and giving reward from kill twice)
        if (victim.Character.Health <= 0)
            return;

        victim.Character.DeathState = DeathState.Dead;
        victim.Character.health.SetCurrent(0);
    }

    public int DealDamage(Unit victim, int damage, CleanDamage cleanDamage, DamageEffectType damagetype, SpellSchoolMask damageSchoolMask, TrinitySpellInfo spellProto)
    {
        // Hook for OnDamage Event
        //sScriptMgr->OnDamage(this, victim, damage);

        if (damage < 1)
            return 0;

        int health = victim.Character.health.CurrentValue;

        if (health <= damage)
        {
            Debug.Log("DealDamage: Victim just died");

            Kill(victim);
        }
        else
        {
            Debug.Log("DealDamage: Alive");

            victim.Character.health.Decrease(damage);
            //victim.ModifyHealth(-damage);
        }
        return damage;
    }

    /*public void ApplySpellMod(TrinitySpellInfo spellInfo, SpellModOp op, float baseValue, TrinitySpell spell = null)
    {
        float totalmul = 1.0f;
        int totalflat = 0;

        for (SpellModList::iterator itr = m_spellMods[op].begin(); itr != m_spellMods[op].end(); ++itr)
        {
            SpellModifier* mod = *itr;

            // Charges can be set only for mods with auras
            if (!mod->ownerAura)
                ASSERT(mod->charges == 0);

            if (!IsAffectedBySpellmod(spellInfo, mod, spell))
                continue;

            if (mod->type == SPELLMOD_FLAT)
                totalflat += mod->value;
            else if (mod->type == SPELLMOD_PCT)
            {
                // skip percent mods for null basevalue (most important for spell mods with charges)
                if (baseValue == T(0))
                    continue;

                // special case (skip > 10sec spell casts for instant cast setting)
                if (mod->op == SPELLMOD_CASTING_TIME && baseValue >= T(10000) && mod->value <= -100)
                    continue;

                totalmul += CalculatePct(1.0f, mod->value);
            }
        }

        baseValue = float((baseValue + totalflat) * totalmul);
    }*/

    #endregion
}