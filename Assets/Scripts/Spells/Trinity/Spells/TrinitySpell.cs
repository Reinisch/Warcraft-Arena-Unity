using UnityEngine;
using System.Collections.Generic;
using System;

public class TrinitySpell
{
    public SpellInfo spellInfo;
    public Guid castId;
    public Guid originalCastId;
    public int castFlagsEx;
    public int SpellVisual;
    public int preCastSpell;
    public int comboPointGain;
    //public UsedSpellMods m_appliedMods;

    public Unit caster;
    public SpellValue spellValue;

    public Guid originalCasterGuid;                    // real source of cast (aura caster/etc), used for spell targets selection
                                                       // e.g. damage around area spell trigered by victim aura and damage enemies of aura caster
    public Unit originalCaster;                             // cached pointer for m_originalCaster, updated at Spell::UpdatePointers()

    //Spell data
    public SpellSchoolMask m_spellSchoolMask;                  // Spell school (can be overwrite for some spells (wand shoot for example)

    public List<PowerCostData> powerCost;                  // Calculated spell cost initialized only in Spell::prepare
    public int casttime;                                   // Calculated spell cast time initialized only in Spell::prepare
    public int channeledDuration;                          // Calculated channeled spell duration in order to calculate correct pushback.
    public bool canReflect;                                  // can reflect this spell?
    public bool autoRepeat;
    public bool isDelayedInstantCast;                        // whether this is a creature's delayed instant cast
    public int runesState;

    // These vars are used in both delayed spell system and modified immediate spell system
    public bool referencedFromCurrentSpell;                  // mark as references to prevent deleted and access by dead pointers
    public bool executedCurrently;                           // mark as executed to prevent deleted and access by dead pointers
    public bool mneedComboPoints;
    public int m_applyMultiplierMask;
    public float[] m_damageMultipliers;

    // Current targets, to be used in SpellEffects (MUST BE USED ONLY IN SPELL EFFECTS)
    public Unit unitTarget;
    public GameObject gameObjTarget;
    public int damage;
    public float variance;
    public SpellEffectHandleMode effectHandleMode;
    public TrinitySpellEffectInfo effectInfo;
    // used in effects handlers
    public Aura spellAura;

    // this is set in Spell Hit, but used in Apply Aura handler
    //DiminishingLevels m_diminishLevel;
    //DiminishingGroup m_diminishGroup;

    // -------------------------------------------
    public GameObject focusObject;

    // Damage and healing in effects need just calculate
    public int effectDamage;           // Damage  in effects count here
    public int effectHealing;          // Healing in effects count here

    // ******************************************
    // Spell trigger system
    // ******************************************
    public int procAttacker;                // Attacker trigger flags
    public int procVictim;                  // Victim   trigger flags
    public int procEx;

    public SpellState spellState;
    public int timer;

    public void EffectDistract(SpellEffIndex effIndex) { }
    public void EffectPull(SpellEffIndex effIndex) { }
    public void EffectSchoolDMG(SpellEffIndex effIndex) { }
    public void EffectInstaKill(SpellEffIndex effIndex) { }
    public void EffectDummy(SpellEffIndex effIndex) { }
    public void EffectTeleportUnits(SpellEffIndex effIndex) { }
    public void EffectApplyAura(SpellEffIndex effIndex) { }
    public void EffectPowerBurn(SpellEffIndex effIndex) { }
    public void EffectPowerDrain(SpellEffIndex effIndex) { }
    public void EffectHeal(SpellEffIndex effIndex) { }
    public void EffectBind(SpellEffIndex effIndex) { }
    public void EffectPersistentAA(SpellEffIndex effIndex) { }
    public void EffectEnergize(SpellEffIndex effIndex) { }
    public void EffectApplyAreaAura(SpellEffIndex effIndex) { }
    public void EffectSummonType(SpellEffIndex effIndex) { }
    public void EffectDispel(SpellEffIndex effIndex) { }
    public void EffectUntrainTalents(SpellEffIndex effIndex) { }
    public void EffectHealMechanical(SpellEffIndex effIndex) { }
    public void EffectLeapBack(SpellEffIndex effIndex) { }
    public void EffectTeleUnitsFaceCaster(SpellEffIndex effIndex) { }
    public void EffectSummonPet(SpellEffIndex effIndex) { }
    public void EffectWeaponDmg(SpellEffIndex effIndex) { }
    public void EffectForceCast(SpellEffIndex effIndex) { }
    public void EffectTriggerSpell(SpellEffIndex effIndex) { }
    public void EffectTriggerMissileSpell(SpellEffIndex effIndex) { }
    public void EffectHealMaxHealth(SpellEffIndex effIndex) { }
    public void EffectInterruptCast(SpellEffIndex effIndex) { }
    public void EffectScriptEffect(SpellEffIndex effIndex) { }
    public void EffectCharge(SpellEffIndex effIndex) { }
    public void EffectChargeDest(SpellEffIndex effIndex) { }
    public void EffectKnockBack(SpellEffIndex effIndex) { }
    public void EffectPullTowards(SpellEffIndex effIndex) { }
    public void EffectDispelMechanic(SpellEffIndex effIndex) { }
    public void EffectSkill(SpellEffIndex effIndex) { }
    public void EffectRemoveAura(SpellEffIndex effIndex) { }

    public List<Aura> UsedSpellMods { get; set; }

    public TrinitySpell(Unit caster, SpellInfo info, TriggerCastFlags triggerFlags, Guid originalCasterGUID, bool skipCheck = false)
    {
    }

    public void InitiateExplicitTargets(SpellCastTargets targets) { }

    public void SelectSpellTargets() { }

    public void Prepare(SpellCastTargets targets, AuraEffect triggeredByAura = null)
    {

    }
    public void Cancel()
    {

    }
    public void Update(float timeDelta)
    {

    }
    public void Cast(bool skipCheck = false)
    {

    }
    public void Finish(bool ok = true)
    {

    }
    public void TakePower()
    {

    }

    public SpellCastResult CheckCast(bool strict)
    {
        return SpellCastResult.SUCCESS;
    }

    public int CalculateDamage(int effectIndex, Unit target, float var = 0)
    {
        //return m_caster->CalculateSpellDamage(target, m_spellInfo, effectIndex, &m_spellValue->EffectBasePoints[effectIndex], var, m_castItemLevel)
        return 10;
    }
}
