using UnityEngine;
using System.Collections.Generic;
using System;

public class TargetInfo
{
    public Guid TargetId { get; set; }
    public float Delay { get; set; }
    public SpellMissInfo MissCondition { get; set; }
    public SpellMissInfo ReflectResult { get; set; }
    public int EffectMask { get; set; }
    public bool Processed { get; set; }
    public bool Alive { get; set; }
    public bool Crit { get; set; }
    public bool ScaleAura { get; set; }
    public int Damage { get; set; }
};

public class TrinitySpell
{
    public TrinitySpellInfo SpellInfo { get; private set; }
    public List<TrinitySpellEffectInfo> SpellEffects { get { return SpellInfo.SpellEffectInfos; } }

    public Guid CastId { get; private set; }
    public Guid OriginalCastId { get; private set; }
    public bool SkipCheck { get; private set; }

    // These vars are used in both delayed spell system and modified immediate spell system
    public bool ExecutedCurrently { get; set; }
    public int ApplyMultiplierMask { get; set; }
    public float[] DamageMultipliers { get; set; }
    public bool ReferencedFromCurrentSpell { get; set; } // mark as references to prevent deleted and access by dead pointers

    public SpellSchoolMask SpellSchoolMask { get; set; } // Spell school (can be overwrite for some spells (wand shoot for example)

    public Unit Caster { get; private set; }
    public Guid OriginalCasterGuid { get; set; }        // real source of cast (aura caster/etc), used for spell targets selection
    public Unit OriginalCaster { get; set; }            // cached pointer for OriginalCaster, updated at UpdatePointers()

    public SpellState SpellState { get; set; }
    public TriggerCastFlags TriggerCastFlags { get; set; }

    // Current targets, to be used in SpellEffects (MUST BE USED ONLY IN SPELL EFFECTS)
    public Unit UnitTarget { get; set; }
    public Transform DestTarget { get; set; }
    public int Damage { get; set; }
    public float Variance { get; set; }
    public SpellEffectHandleMode EffectHandleMode { get; set; }
    public TrinitySpellEffectInfo EffectInfo { get; set; }

    // this is set in Spell Hit, but used in Apply Aura handler
    DiminishingLevels DiminishLevel { get; set; }
    DiminishingGroup DiminishGroup { get; set; }

    // Damage and healing in effects need just calculate
    public int EffectDamage { get; set; }       // Damage  in effects count here
    public int EffectHealing { get; set; }      // Healing in effects count here
    public Aura SpellAura { get; set; }         // Used in effects handlers
    public TrinitySpellInfo TriggeredByAuraSpell { get; set; }

    public List<Aura> UsedSpellMods { get; set; }
    public SpellValue SpellValue { get; set; }
    public int SpellVisual { get; set; }
    public bool CanReflect { get; set; }                            // Can reflect this spell?

    public float CastTime { get; set; }                               // Calculated spell cast time initialized only in Spell::prepare
    public float CastTimeEnd { get; set; }                           // Initialized only in Spell::prepare
    public List<PowerCostData> PowerCost { get; set; }              // Calculated spell cost initialized only in Spell::prepare

    public List<TargetInfo> UniqueTargets { get; set; }
    public SpellCastTargets Targets { get; set; }

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


    public TrinitySpell(Unit caster, TrinitySpellInfo info, TriggerCastFlags triggerFlags, Guid originalCasterId, bool skipCheck = false)
    {
        Caster = caster;
        SpellInfo = info;

        CastId = Guid.NewGuid();
        OriginalCastId = CastId;
        SkipCheck = skipCheck;

        ExecutedCurrently = false;
        ApplyMultiplierMask = 0;
        DamageMultipliers = new float[3];
        ReferencedFromCurrentSpell = false;

        SpellSchoolMask = SpellInfo.SchoolMask;

        if (originalCasterId != Guid.Empty)
            OriginalCasterGuid = originalCasterId;
        else
            OriginalCasterGuid = caster.Character.Id;

        if (OriginalCasterGuid == caster.Character.Id)
            OriginalCaster = caster;
        else
            OriginalCaster = ArenaManager.ArenaUnits.Find(unit => unit.Character.Id == OriginalCasterGuid);

        SpellState = SpellState.NONE;
        TriggerCastFlags = triggerFlags;

        UnitTarget = null;
        DestTarget = null;
        Damage = 0;
        Variance = 0.0f;
        EffectHandleMode = SpellEffectHandleMode.LAUNCH;
        EffectInfo = null;

        DiminishLevel = DiminishingLevels.LEVEL_1;
        DiminishGroup = DiminishingGroup.NONE;

        EffectDamage = 0;
        EffectHealing = 0;
        SpellAura = null;

        SpellVisual = SpellInfo.VisualId;
        CanReflect = SpellInfo.DamageClass == SpellDamageClass.MAGIC && !SpellInfo.HasAttribute(SpellAttributes.CANT_BE_REFLECTED)
            && !SpellInfo.IsPassive() && !SpellInfo.IsPositive();

        UniqueTargets = new List<TargetInfo>();
    }

    public void InitiateExplicitTargets(SpellCastTargets targets)
    {
        Targets = targets;
        Targets.OrigTarget = Targets.UnitTarget;
    }

    public void SelectSpellTargets() { }

    public void Prepare(SpellCastTargets targets, AuraEffect triggeredByAura = null)
    {
        InitiateExplicitTargets(targets);

        if(triggeredByAura != null)
            TriggeredByAuraSpell = triggeredByAura.SpellInfo;

        SpellState = SpellState.PREPARING;

        SpellCastResult result = CheckCast(true);

        CastTime = SpellInfo.CastTime != null ? SpellInfo.CastTime.CastTime : 0;

        Cast(true);
    }
    public void Cancel()
    {

    }
    public void Update(float timeDelta)
    {

    }
    public void Cast(bool skipCheck = false)
    {
        SpellManager.ApplySpellVisuals(Caster, SpellInfo);

        SpellManager.ApplySpellCastSound(Caster, SpellInfo);
    }
    public void Finish(bool ok = true)
    {

    }
    public void TakePower()
    {

    }

    public bool IsIgnoringCooldowns() { return TriggerCastFlags.HasFlag(TriggerCastFlags.IGNORE_SPELL_AND_CATEGORY_CD); }
    public bool HasGlobalCooldown()
    { 
        return Caster.Character.SpellHistory.HasGlobalCooldown();
    }

    public SpellCastResult CheckRange(bool strict)
    {
        // Don't check for instant cast spells
        if (!strict && CastTime == 0)
            return SpellCastResult.SPELL_CAST_OK;

        Unit target = Targets.UnitTarget;
        float minRange = 0.0f;
        float maxRange = 0.0f;
        float rangeMod = 0.0f;

        if (SpellInfo.Range != null)
        {
            if (SpellInfo.Range.Flags.HasFlag(SpellRangeFlag.MELEE))
            {
                rangeMod = 3.0f + 4.0f / 3.0f;
            }
            else
            {
                float meleeRange = 0.0f;
                if (SpellInfo.Range.Flags.HasFlag(SpellRangeFlag.RANGED))
                    meleeRange = 3.0f + 4.0f / 3.0f;

                minRange = Caster.GetSpellMinRangeForTarget(target, SpellInfo) + meleeRange;
                maxRange = Caster.GetSpellMaxRangeForTarget(target, SpellInfo);
            }
        }

        maxRange += rangeMod;

        minRange *= minRange;
        maxRange *= maxRange;

        if (target != null && target != Caster)
        {
            if (Vector3.Distance(Caster.transform.position, target.transform.position) > maxRange)
                return SpellCastResult.OUT_OF_RANGE;

            if (minRange > 0.0f && Vector3.Distance(Caster.transform.position, target.transform.position) < minRange)
                return SpellCastResult.OUT_OF_RANGE;
        }

        return SpellCastResult.SPELL_CAST_OK;
    }

    public SpellCastResult CheckCast(bool strict)
    {
        // check death state
        if (!Caster.IsAlive() && !SpellInfo.IsPassive())
            return SpellCastResult.CASTER_DEAD;

        // check cooldowns to prevent cheating
        if (!SpellInfo.IsPassive())
        {
            if (!Caster.Character.SpellHistory.IsReady(SpellInfo, IsIgnoringCooldowns()))
            {
                if (TriggeredByAuraSpell != null)
                    return SpellCastResult.DONT_REPORT;
                else
                    return SpellCastResult.NOT_READY;
            }
        }

        // Check global cooldown
        if (strict && !(TriggerCastFlags.HasFlag(TriggerCastFlags.IGNORE_GCD) && HasGlobalCooldown()))
            return SpellCastResult.NOT_READY;

        // Check for line of sight for spells with dest
        /*if (m_targets.HasDst())
        {
            float x, y, z;
            m_targets.GetDstPos()->GetPosition(x, y, z);

            if (!m_spellInfo->HasAttribute(SPELL_ATTR2_CAN_TARGET_NOT_IN_LOS) && !DisableMgr::IsDisabledFor(DISABLE_TYPE_SPELL, m_spellInfo->Id, NULL, SPELL_DISABLE_LOS) && !m_caster->IsWithinLOS(x, y, z))
                return SPELL_FAILED_LINE_OF_SIGHT;
        }*/

        SpellCastResult castResult = SpellCastResult.SPELL_CAST_OK;

        // Triggered spells also have range check
        /// @todo determine if there is some flag to enable/disable the check
        castResult = CheckRange(strict);
        if (castResult != SpellCastResult.SPELL_CAST_OK)
            return castResult;

        return SpellCastResult.SUCCESS;
    }

    public int CalculateDamage(int effectIndex, Unit target, float var = 0)
    {
        //return m_caster->CalculateSpellDamage(target, m_spellInfo, effectIndex, &m_spellValue->EffectBasePoints[effectIndex], var, m_castItemLevel)
        return 10;
    }
}
