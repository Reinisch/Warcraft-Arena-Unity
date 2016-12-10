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

    public Guid CastId { get; private set; }
    public Guid OriginalCastId { get; private set; }
    public bool SkipCheck { get; private set; }

    // These vars are used in both delayed spell system and modified immediate spell system
    public bool ExecutedCurrently { get; set; }
    public int ApplyMultiplierMask { get; set; }
    public float[] DamageMultipliers { get; set; }
    public bool ReferencedFromCurrentSpell { get; set; }    // mark as references to prevent deleted and access by dead pointers

    public SpellSchoolMask SpellSchoolMask { get; set; }    // Spell school (can be overwrite for some spells (wand shoot for example)

    public Unit Caster { get; private set; }
    public Guid OriginalCasterGuid { get; set; }            // real source of cast (aura caster/etc), used for spell targets selection
    public Unit OriginalCaster { get; set; }                // cached pointer for OriginalCaster, updated at UpdatePointers()

    public SpellState SpellState { get; set; }
    public TriggerCastFlags TriggerCastFlags { get; set; }

    // Current targets, to be used in SpellEffects (MUST BE USED ONLY IN SPELL EFFECTS)
    public Unit UnitTarget { get; set; }
    public Transform DestTarget { get; set; }
    public int Damage { get; set; }
    public float Variance { get; set; }
    public SpellEffectHandleMode EffectHandleMode { get; set; }
    public TrinitySpellEffectInfo EffectInfo { get; set; }

    // This is set in Spell Hit, but used in Apply Aura handler
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
    public float DelayMoment { get; set; }

    public float CastTime { get; set; }                             // Calculated spell cast time initialized only in Spell::prepare
    public float CastTimeEnd { get; set; }                          // Initialized only in Spell::prepare
    public List<PowerCostData> PowerCost { get; set; }              // Calculated spell cost initialized only in Spell::prepare

    public List<TrinitySpellEffectInfo> SpellEffects { get; private set; }
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

    public TrinitySpellEffectInfo GetEffect(int effIndex)
    {
        if (SpellEffects.Count < effIndex)
            return null;

        return SpellEffects[effIndex];
    }

    public TrinitySpell(Unit caster, TrinitySpellInfo info, TriggerCastFlags triggerFlags, Guid originalCasterId, bool skipCheck = false)
    {
        SpellValue = new SpellValue(info);

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
        SpellEffects = new List<TrinitySpellEffectInfo>(SpellInfo.SpellEffectInfos);
}


    private bool CheckEffectTarget(Unit target, TrinitySpellEffectInfo effect, Vector3 losPosition)
    {
        // check for ignore LOS on the effect itself

        /// @todo shit below shouldn't be here, but it's temporary
        //Check targets for LOS visibility
        /*if (losPosition)
            return target->IsWithinLOS(losPosition->GetPositionX(), losPosition->GetPositionY(), losPosition->GetPositionZ());
        else
        {
            // Get GO cast coordinates if original caster -> GO
            WorldObject* caster = NULL;
            if (m_originalCasterGUID.IsGameObject())
                caster = m_caster->GetMap()->GetGameObject(m_originalCasterGUID);
            if (!caster)
                caster = m_caster;
            if (target != m_caster && !target->IsWithinLOSInMap(caster))
                return false;
        }*/

        return true;
    }

    private void AddUnitTarget(Unit target, int effectMask, bool checkIfValid = true, bool isImplicit = true, Vector3 losPosition = default(Vector3))
    {
        int validEffectMask = 0;
        foreach (var effect in SpellEffects)
            if (effect != null && (effectMask & (1 << effect.EffectIndex)) != 0 && CheckEffectTarget(target, effect, losPosition))
                validEffectMask |= 1 << effect.EffectIndex;

        effectMask &= validEffectMask;

        // no effects left
        if (effectMask == 0)
            return;

        /*// Check for effect immune skip if immuned
        for (SpellEffectInfo const* effect : GetEffects())
            if (effect && target->IsImmunedToSpellEffect(m_spellInfo, effect->EffectIndex))
                effectMask &= ~(1 << effect->EffectIndex);*/

        // Lookup target in already in list
        var sameTargetInfo = UniqueTargets.Find(unit => unit.TargetId == target.Character.Id);
        if (sameTargetInfo != null)
        {
            sameTargetInfo.EffectMask |= effectMask;             // Immune effects removed from mask
            return;
        }

        // This is new target calculate data for him

        // Get spell hit result on target
        TargetInfo targetInfo = new TargetInfo();
        targetInfo.TargetId = target.Character.Id;                  // Store target GUID
        targetInfo.EffectMask = effectMask;                         // Store all effects not immune
        targetInfo.Processed  = false;                              // Effects not apply on target
        targetInfo.Alive      = target.IsAlive();
        targetInfo.Damage     = 0;
        targetInfo.Crit       = false;
        targetInfo.ScaleAura  = false;

        // Calculate hit result
        if (OriginalCaster != null)
        {
            targetInfo.MissCondition = OriginalCaster.SpellHitResult(target, SpellInfo, CanReflect);
            if (SkipCheck && targetInfo.MissCondition != SpellMissInfo.IMMUNE)
                targetInfo.MissCondition = SpellMissInfo.NONE;
        }
        else
            targetInfo.MissCondition = SpellMissInfo.EVADE; //SPELL_MISS_NONE;

        // Spell have speed - need calculate incoming time
        // Incoming time is zero for self casts. At least I think so.
        if (SpellInfo.Speed > 0.0f && Caster != target)
        {
            // calculate spell incoming interval
            /// @todo this is a hack
            float dist = Vector3.Distance(Caster.transform.position, target.transform.position);

            if (dist < 5.0f)
                dist = 5.0f;

            targetInfo.Delay = SpellInfo.Speed;

            // Calculate minimum incoming time
            if (DelayMoment == 0 || DelayMoment > targetInfo.Delay)
                DelayMoment = targetInfo.Delay;
        }
        else
            targetInfo.Delay = 0.0f;

        // If target reflect spell back to caster
        /*if (targetInfo.missCondition == SPELL_MISS_REFLECT)
        {
            // Calculate reflected spell result on caster
            targetInfo.reflectResult = m_caster->SpellHitResult(m_caster, m_spellInfo, m_canReflect);

            if (targetInfo.reflectResult == SPELL_MISS_REFLECT)     // Impossible reflect again, so simply deflect spell
                targetInfo.reflectResult = SPELL_MISS_PARRY;

            // Increase time interval for reflected spells by 1.5
            targetInfo.timeDelay += targetInfo.timeDelay >> 1;
        }
        else
            targetInfo.reflectResult = SPELL_MISS_NONE;*/

        // Add target to list
        UniqueTargets.Add(targetInfo);
    }

    private void SelectSpellTargets()
    {
        int processedAreaEffectsMask = 0;

        foreach (var effect in SpellEffects)
        {
            if (effect == null)
                continue;

            SelectEffectImplicitTargets(effect.EffectIndex, effect.TargetA, ref processedAreaEffectsMask);
            SelectEffectImplicitTargets(effect.EffectIndex, effect.TargetB, ref processedAreaEffectsMask);
        }

        /*if (m_targets.HasDst())
        {
            if (m_targets.HasTraj())
            {
                float speed = m_targets.GetSpeedXY();
                if (speed > 0.0f)
                    m_delayMoment = uint64(std::floor(m_targets.GetDist2d() / speed * 1000.0f));
            }
            else if (m_spellInfo->Speed > 0.0f)
            {
                float dist = m_caster->GetDistance(*m_targets.GetDstPos());
                if (!m_spellInfo->HasAttribute(SPELL_ATTR9_SPECIAL_DELAY_CALCULATION))
                    m_delayMoment = uint64(std::floor(dist / m_spellInfo->Speed * 1000.0f));
                else
                    m_delayMoment = uint64(m_spellInfo->Speed * 1000.0f);
            }
        }*/
    }

    private void SelectEffectImplicitTargets(int effIndex, TargetTypes targetType, ref int processedEffectMask)
    {
        int effectMask = (1 << effIndex);
        // set the same target list for all effects
        // some spells appear to need this, however this requires more research
        switch (targetType.Category())
        {
            case SpellTargetSelectionCategories.NEARBY:
            case SpellTargetSelectionCategories.CONE:
            case SpellTargetSelectionCategories.AREA:
                // targets for effect already selected
                if ((effectMask & processedEffectMask) > 0)
                    return;

                var currentEffect = GetEffect(effIndex);
                if (currentEffect != null)
                {
                    // choose which targets we can select at once
                    foreach (var effect in SpellEffects)
                    {
                        //for (uint32 j = effIndex + 1; j < MAX_SPELL_EFFECTS; ++j)
                        if (effect == null || effect.EffectIndex <= effIndex)
                            continue;

                        if (currentEffect.TargetA == effect.TargetA && currentEffect.TargetB == effect.TargetB &&
                            currentEffect.CalcRadius(Caster, this) == effect.CalcRadius(Caster, this))
                        {
                            effectMask |= 1 << effect.EffectIndex;
                        }
                    }
                }
                processedEffectMask |= effectMask;
                break;
            default:
                break;
        }

        switch (targetType.Category())
        {
            case SpellTargetSelectionCategories.CHANNEL:
                //SelectImplicitChannelTargets(effIndex, targetType);
                break;
            case SpellTargetSelectionCategories.NEARBY:
                //SelectImplicitNearbyTargets(effIndex, targetType, effectMask);
                break;
            case SpellTargetSelectionCategories.CONE:
                //SelectImplicitConeTargets(effIndex, targetType, effectMask);
                break;
            case SpellTargetSelectionCategories.AREA:
                SelectImplicitAreaTargets(effIndex, targetType, effectMask);
                break;
            case SpellTargetSelectionCategories.DEFAULT:
                #region Default Targets
                /*switch (targetType.GetObjectType())
                {
                    case TARGET_OBJECT_TYPE_SRC:
                        switch (targetType.GetReferenceType())
                        {
                            case TARGET_REFERENCE_TYPE_CASTER:
                                m_targets.SetSrc(* m_caster);
                                break;
                            default:
                                ASSERT(false && "Spell::SelectEffectImplicitTargets: received not implemented select target reference type for TARGET_TYPE_OBJECT_SRC");
                                break;
                        }
                        break;
                    case TARGET_OBJECT_TYPE_DEST:
                         switch (targetType.GetReferenceType())
                         {
                             case TARGET_REFERENCE_TYPE_CASTER:
                                 SelectImplicitCasterDestTargets(effIndex, targetType);
                                 break;
                             case TARGET_REFERENCE_TYPE_TARGET:
                                 SelectImplicitTargetDestTargets(effIndex, targetType);
                                 break;
                             case TARGET_REFERENCE_TYPE_DEST:
                                 SelectImplicitDestDestTargets(effIndex, targetType);
                                 break;
                             default:
                                 ASSERT(false && "Spell::SelectEffectImplicitTargets: received not implemented select target reference type for TARGET_TYPE_OBJECT_DEST");
                                 break;
                         }
                         break;
                    default:
                        switch (targetType.GetReferenceType())
                        {
                            case TARGET_REFERENCE_TYPE_CASTER:
                                SelectImplicitCasterObjectTargets(effIndex, targetType);
                                break;
                            case TARGET_REFERENCE_TYPE_TARGET:
                                SelectImplicitTargetObjectTargets(effIndex, targetType);
                                break;
                            default:
                                ASSERT(false && "Spell::SelectEffectImplicitTargets: received not implemented select target reference type for TARGET_TYPE_OBJECT");
                                break;
                        }
                        break;
                }*/
                #endregion
                break;
            case SpellTargetSelectionCategories.NYI:
                Debug.LogErrorFormat("SPELL: Target type in spell #{0} is not implemented yet!", SpellInfo.Id);
                break;
            default:
                break;
        }
    }

    private void SelectImplicitAreaTargets(int effIndex, TargetTypes targetType, int effMask)
    {
        Unit referer = null;
        switch (targetType.ReferenceType())
        {
            case SpellTargetReferenceTypes.SRC:
            case SpellTargetReferenceTypes.DEST:
            case SpellTargetReferenceTypes.CASTER:
                referer = Caster;
                break;
            case SpellTargetReferenceTypes.TARGET:
                referer = Targets.UnitTarget;
                break;
            case SpellTargetReferenceTypes.LAST:
            {
                for (int i = UniqueTargets.Count - 1; i >= 0; i--)
                {
                    if ((UniqueTargets[i].EffectMask & (1 << effIndex)) > 0)
                    {
                        referer = ArenaManager.ArenaUnits.Find(unit => unit.Character.Id == UniqueTargets[i].TargetId);
                        break;
                    }
                }
                break;
            }
            default:
                return;
        }

        if (referer == null)
            return;

        Vector3 center = Vector3.zero;
        switch (targetType.ReferenceType())
        {
            case SpellTargetReferenceTypes.SRC:
                center = Targets.Source;
                break;
            case SpellTargetReferenceTypes.DEST:
                center = Targets.Dest;
                break;
            case SpellTargetReferenceTypes.CASTER:
            case SpellTargetReferenceTypes.TARGET:
            case SpellTargetReferenceTypes.LAST:
                center = referer.transform.position;
                break;
             default:
                Debug.LogError("SelectImplicitAreaTargets: received not implemented target reference type!");
                return;
        }

        List<Unit> targets = new List<Unit>();
        TrinitySpellEffectInfo effect = GetEffect(effIndex);
        if (effect == null)
            return;

        float radius = effect.CalcRadius(Caster, this) * SpellValue.RadiusMod;
        ArenaManager.SearchTargets(targets, radius, center, referer, targetType.CheckType());

        if (targets.Count > 0)
        {
            // #TODO: Other special target selection goes here
            /*if (uint32 maxTargets = m_spellValue->MaxAffectedTargets)
                Trinity::Containers::RandomResizeList(targets, maxTargets);*/
            
            foreach(var unit in targets)
                AddUnitTarget(unit, effMask, false, true, center);
        }
    }

    private void HandleImmediate()
    {
        // process immediate effects (items, ground, etc.) also initialize some variables
        HandleImmediatePhase();

        /*for (std::vector<TargetInfo>::iterator ihit = m_UniqueTargetInfo.begin(); ihit != m_UniqueTargetInfo.end(); ++ihit)
            DoAllEffectOnTarget(&(*ihit));

        for (std::vector<GOTargetInfo>::iterator ihit = m_UniqueGOTargetInfo.begin(); ihit != m_UniqueGOTargetInfo.end(); ++ihit)
            DoAllEffectOnTarget(&(*ihit));*/

        // spell is finished, perform some last features of the spell here
        HandleFinishPhase();

        if (SpellState != SpellState.CASTING)
            Finish(true);
    }

    private void HandleImmediatePhase()
    {
        SpellAura = null;
        // initialize Diminishing Returns Data
        DiminishLevel = DiminishingLevels.LEVEL_1;
        DiminishGroup = DiminishingGroup.NONE;

        // handle effects with SPELL_EFFECT_HANDLE_HIT mode
        foreach (var effect in SpellEffects)
        {
            // don't do anything for empty effect
            if (effect == null)
                continue;

            // #TODO : call effect handlers to handle destination hit
            //HandleEffects(NULL, NULL, NULL, effect->EffectIndex, SPELL_EFFECT_HANDLE_HIT);
        }
    }

    private void HandleFinishPhase()
    {
        /*if (m_caster->m_movedPlayer)
        {
            // Take for real after all targets are processed
            if (m_needComboPoints)
                m_caster->m_movedPlayer->ClearComboPoints();

            // Real add combo points from effects
            if (m_comboPointGain)
                m_caster->m_movedPlayer->GainSpellComboPoints(m_comboPointGain);
        }

        if (m_caster->m_extraAttacks && HasEffect(SPELL_EFFECT_ADD_EXTRA_ATTACKS))
        {
            if (Unit * victim = ObjectAccessor::GetUnit(*m_caster, m_targets.GetOrigUnitTargetGUID()))
                m_caster->HandleProcExtraAttackFor(victim);
            else
                m_caster->m_extraAttacks = 0;
        }*/
    }

    public void InitiateExplicitTargets(SpellCastTargets targets)
    {
        Targets = targets;
        Targets.OrigTarget = Targets.UnitTarget;
        Targets.UnitTarget = Targets.UnitTarget;
        //Targets.Source = Caster.transform.position;
        //Targets.Dest = Targets.UnitTarget.transform.position;
    }

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

        Caster.Character.SpellHistory.HandleCooldowns(SpellInfo, this);

        if (CastTime == 0)
            HandleImmediate();
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