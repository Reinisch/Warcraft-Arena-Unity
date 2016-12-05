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

    Unit caster;
    SpellValue spellValue;

    Guid originalCasterGuid;                    // real source of cast (aura caster/etc), used for spell targets selection
                                                        // e.g. damage around area spell trigered by victim aura and damage enemies of aura caster
    Unit originalCaster;                             // cached pointer for m_originalCaster, updated at Spell::UpdatePointers()

    //Spell data
    SpellSchoolMask m_spellSchoolMask;                  // Spell school (can be overwrite for some spells (wand shoot for example)

    List<PowerCostData> powerCost;                  // Calculated spell cost initialized only in Spell::prepare
    int casttime;                                   // Calculated spell cast time initialized only in Spell::prepare
    int channeledDuration;                          // Calculated channeled spell duration in order to calculate correct pushback.
    bool canReflect;                                  // can reflect this spell?
    bool autoRepeat;
    bool isDelayedInstantCast;                        // whether this is a creature's delayed instant cast
    int runesState;

    // These vars are used in both delayed spell system and modified immediate spell system
    bool referencedFromCurrentSpell;                  // mark as references to prevent deleted and access by dead pointers
    bool executedCurrently;                           // mark as executed to prevent deleted and access by dead pointers
    bool mneedComboPoints;
    int m_applyMultiplierMask;
    float[] m_damageMultipliers;

    // Current targets, to be used in SpellEffects (MUST BE USED ONLY IN SPELL EFFECTS)
    Unit unitTarget;
    GameObject gameObjTarget;
    int damage;
    float variance;
    SpellEffectHandleMode effectHandleMode;
    TrinitySpellEffectInfo effectInfo;
    // used in effects handlers
    Aura spellAura;

    // this is set in Spell Hit, but used in Apply Aura handler
    //DiminishingLevels m_diminishLevel;
    //DiminishingGroup m_diminishGroup;

    // -------------------------------------------
    GameObject focusObject;

    // Damage and healing in effects need just calculate
    int effectDamage;           // Damage  in effects count here
    int effectHealing;          // Healing in effects count here

    // ******************************************
    // Spell trigger system
    // ******************************************
    int procAttacker;                // Attacker trigger flags
    int procVictim;                  // Victim   trigger flags
    int procEx;
}
