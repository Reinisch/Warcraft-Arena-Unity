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
    public int Healing { get; set; }
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
    public float CastTimer { get; set; }                            // Initialized only in Spell::prepare
    public List<PowerCostData> PowerCost { get; set; }              // Calculated spell cost initialized only in Spell::prepare

    public List<TrinitySpellEffectInfo> SpellEffects { get; private set; }
    public List<TargetInfo> UniqueTargets { get; set; }
    public SpellCastTargets Targets { get; set; }

    public void EffectDistract(SpellEffIndex effIndex) { }
    public void EffectPull(SpellEffIndex effIndex) { }
    public void EffectSchoolDMG(int effIndex)
    {

    }
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

        SpellState = SpellState.None;
        TriggerCastFlags = triggerFlags;

        UnitTarget = null;
        DestTarget = null;
        Damage = 0;
        Variance = 0.0f;
        EffectHandleMode = SpellEffectHandleMode.Launch;
        EffectInfo = null;

        DiminishLevel = DiminishingLevels.Level1;
        DiminishGroup = DiminishingGroup.None;

        EffectDamage = 0;
        EffectHealing = 0;
        SpellAura = null;

        SpellVisual = SpellInfo.VisualId;
        CanReflect = SpellInfo.DamageClass == SpellDamageClass.Magic && !SpellInfo.HasAttribute(SpellAttributes.CantBeReflected)
            && !SpellInfo.IsPassive() && !SpellInfo.IsPositive();

        UniqueTargets = new List<TargetInfo>();
        SpellEffects = new List<TrinitySpellEffectInfo>(SpellInfo.SpellEffectInfos);
}

    private void ExecuteEffect(SpellEffectType type, int index)
    {
        switch(type)
        {
            case SpellEffectType.SchoolDamage:
                EffectSchoolDMG(index);
                break;
            default:
                break;
        }
    }

    public void HandleEffects(Unit unitTarget, int i, SpellEffectHandleMode mode)
    {
        EffectHandleMode = mode;
        UnitTarget = unitTarget;

        EffectInfo = GetEffect(i);

        if (EffectInfo == null)
        {
            Debug.LogErrorFormat("Spell: {0} HandleEffects at EffectIndex: {1} missing effect", SpellInfo.Id, i);
            return;
        }

        Debug.LogFormat("Spell: {0} Effect: {1}", SpellInfo.Id, EffectInfo.Effect);

        Damage += CalculateDamage(i, UnitTarget, Variance);

        ExecuteEffect(EffectInfo.Effect, i);
    }

    public TrinitySpellEffectInfo GetEffect(int effIndex)
    {
        if (SpellEffects.Count < effIndex)
            return null;

        return SpellEffects[effIndex];
    }

    public bool IsIgnoringCooldowns()
    {
        return TriggerCastFlags.HasFlag(TriggerCastFlags.IgnoreSpellAndCategoryCd);
    }

    public bool HasGlobalCooldown()
    {
        return Caster.Character.SpellHistory.HasGlobalCooldown();
    }



    public void InitiateExplicitTargets(SpellCastTargets targets)
    {
        Targets = targets;
        Targets.OrigTarget = Targets.UnitTarget;
        Targets.UnitTarget = Targets.UnitTarget;
        if (Caster != null)
            Targets.Source = Caster.transform.position;
        if (Targets.UnitTarget != null)
            Targets.Dest = Targets.UnitTarget.transform.position;
        //Targets.Source = Caster.transform.position;
        //Targets.Dest = Targets.UnitTarget.transform.position;
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
            case TargetSelections.Nearby:
            case TargetSelections.Cone:
            case TargetSelections.Area:
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
            case TargetSelections.Channel:
                //SelectImplicitChannelTargets(effIndex, targetType);
                break;
            case TargetSelections.Nearby:
                //SelectImplicitNearbyTargets(effIndex, targetType, effectMask);
                break;
            case TargetSelections.Cone:
                //SelectImplicitConeTargets(effIndex, targetType, effectMask);
                break;
            case TargetSelections.Area:
                SelectImplicitAreaTargets(effIndex, targetType, effectMask);
                break;
            case TargetSelections.Default:
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
            default:
                break;
        }
    }

    private void SelectImplicitAreaTargets(int effIndex, TargetTypes targetType, int effMask)
    {
        Unit referer = null;
        switch (targetType.ReferenceType())
        {
            case TargetReferences.Source:
            case TargetReferences.Dest:
            case TargetReferences.Caster:
                referer = Caster;
                break;
            case TargetReferences.Target:
                referer = Targets.UnitTarget;
                break;
            case TargetReferences.Last:
            {
                for (int i = UniqueTargets.Count - 1; i >= 0; i--)
                {
                    if ((UniqueTargets[i].EffectMask & (1 << effIndex)) > 0)
                    {
                        referer = ArenaManager.ArenaUnits.Find(unit => unit.Character.Id == UniqueTargets[i].TargetId && unit.IsAlive());
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
            case TargetReferences.Source:
                center = Targets.Source;
                break;
            case TargetReferences.Dest:
                center = Targets.Dest;
                break;
            case TargetReferences.Caster:
            case TargetReferences.Target:
            case TargetReferences.Last:
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
        ArenaManager.SearchAreaTargets(targets, radius, center, referer, targetType.CheckType());

        if (targets.Count > 0)
        {
            // #TODO: Other special target selection goes here
            /*if (uint32 maxTargets = m_spellValue->MaxAffectedTargets)
                Trinity::Containers::RandomResizeList(targets, maxTargets);*/
            
            foreach(var unit in targets)
                AddUnitTarget(unit, effMask, false, true, center);
        }
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
        targetInfo.Processed = false;                              // Effects not apply on target
        targetInfo.Alive = target.IsAlive();
        targetInfo.Damage = 0;
        targetInfo.Crit = false;
        targetInfo.ScaleAura = false;

        // Calculate hit result
        if (OriginalCaster != null)
        {
            targetInfo.MissCondition = OriginalCaster.SpellHitResult(target, SpellInfo, CanReflect);
            if (SkipCheck && targetInfo.MissCondition != SpellMissInfo.Immune)
                targetInfo.MissCondition = SpellMissInfo.None;
        }
        else
            targetInfo.MissCondition = SpellMissInfo.Evade; //SPELL_MISS_NONE;

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



    private SpellMissInfo DoSpellHitOnUnit(Unit unit, int effectMask)
    {
        if (unit == null || effectMask == 0)
            return SpellMissInfo.Evade;

        // For delayed spells immunity may be applied between missile launch and hit - check immunity for that case
        /*if (SpellInfo.Speed && (unit->IsImmunedToDamage(m_spellInfo) || unit->IsImmunedToSpell(m_spellInfo)))
            return SPELL_MISS_IMMUNE;*/

        // disable effects to which unit is immune
        //SpellMissInfo returnVal = SpellMissInfo.IMMUNE;
        /*foreach (var effect in SpellEffects)
        if (effect != null && (effectMask & (1 << effect.EffectIndex)) > 0)
            if (unit->IsImmunedToSpellEffect(m_spellInfo, effect->EffectIndex))
                effectMask &= ~(1 << effect->EffectIndex);

        if (!effectMask)
            return returnVal;*/

        int auraEffmask = 0;
        foreach (var effect in SpellEffects)
        if (effect != null && ((effectMask & (1 << effect.EffectIndex)) > 0 && effect.IsUnitOwnedAuraEffect()))
                auraEffmask |= 1 << effect.EffectIndex;

        // Get Data Needed for Diminishing Returns, some effects may have multiple auras, so this must be done on spell hit, not aura add
        /*DiminishGroup = GetDiminishingReturnsGroupForSpell(m_spellInfo);
        if (DiminishGroup && aura_effmask)
        {
            DiminishLevel = unit->GetDiminishing(m_diminishGroup);
            DiminishingReturnsType type = GetDiminishingReturnsGroupType(m_diminishGroup);
            // Increase Diminishing on unit, current informations for actually casts will use values above
            if ((type == DRTYPE_PLAYER &&
                (unit->GetCharmerOrOwnerPlayerOrPlayerItself() || (unit->GetTypeId() == TYPEID_UNIT && unit->ToCreature()->GetCreatureTemplate()->flags_extra & CREATURE_FLAG_EXTRA_ALL_DIMINISH))) ||
                type == DRTYPE_ALL)
                unit->IncrDiminishing(m_diminishGroup);
        }*/

        /*if (auraEffmask > 0)
        {
            // Select rank for aura with level requirements only in specific cases
            // Unit has to be target only of aura effect, both caster and target have to be players, target has to be other than unit target
            TrinitySpellInfo aurSpellInfo = SpellInfo;
            int[] basePoints = new int[SpellHelper.MaxSpellEffects];

            if (OriginalCaster != null)
            {
                bool refresh = false;
                SpellAura = Aura::TryRefreshStackOrCreate(aurSpellInfo, CastId, effectMask, unit,
                    OriginalCaster, (aurSpellInfo == SpellInfo) ? SpellValue.EffectBasePoints : basePoints,
                    ObjectGuid::Empty, &refresh, m_castItemLevel);
                if (SpellAura != null)
                {
                    // Set aura stack amount to desired value
                    if (SpellValue.AuraStackAmount > 1)
                    {
                        if (!refresh)
                            SpellAura.SetStackAmount(m_spellValue->AuraStackAmount);
                        else
                            SpellAura.ModStackAmount(m_spellValue->AuraStackAmount);
                    }

                    // Now Reduce spell duration using data received at spell hit
                    float duration = SpellAura->GetMaxDuration();
                    int limitduration = m_diminishGroup ? GetDiminishingReturnsLimitDuration(aurSpellInfo) : 0;
                    float diminishMod = unit->ApplyDiminishingToDuration(m_diminishGroup, duration, m_originalCaster, m_diminishLevel, limitduration);

                    // unit is immune to aura if it was diminished to 0 duration
                    if (diminishMod == 0.0f)
                    {
                        m_spellAura->Remove();
                        bool found = false;
                        for (SpellEffectInfo const* effect : GetEffects())
                        if (effect && (effectMask & (1 << effect->EffectIndex) && effect->Effect != SPELL_EFFECT_APPLY_AURA))
                            found = true;
                        if (!found)
                            return SPELL_MISS_IMMUNE;
                    }
                    else
                    {
                        ((UnitAura*)m_spellAura)->SetDiminishGroup(m_diminishGroup);

                        bool positive = m_spellAura->GetSpellInfo()->IsPositive();
                        if (AuraApplication * aurApp = m_spellAura->GetApplicationOfTarget(m_originalCaster->GetGUID()))
                            positive = aurApp->IsPositive();

                        duration = m_originalCaster->ModSpellDuration(aurSpellInfo, unit, duration, positive, effectMask);

                        if (duration > 0)
                        {
                            // Haste modifies duration of channeled spells
                            if (m_spellInfo->IsChanneled())
                                m_originalCaster->ModSpellDurationTime(aurSpellInfo, duration, this);
                            else if (m_spellInfo->HasAttribute(SPELL_ATTR5_HASTE_AFFECT_DURATION))
                            {
                                int32 origDuration = duration;
                                duration = 0;
                                for (SpellEffectInfo const* effect : GetEffects())
                                if (effect)
                                    if (AuraEffect const* eff = m_spellAura->GetEffect(effect->EffectIndex))
                                        if (int32 period = eff->GetPeriod())  // period is hastened by UNIT_MOD_CAST_SPEED
                                            duration = std::max(std::max(origDuration / period, 1) * period, duration);

                                // if there is no periodic effect
                                if (!duration)
                                    duration = int32(origDuration * m_originalCaster->GetFloatValue(UNIT_MOD_CAST_SPEED));
                            }
                        }

                        if (duration != m_spellAura->GetMaxDuration())
                        {
                            m_spellAura->SetMaxDuration(duration);
                            m_spellAura->SetDuration(duration);
                        }
                        SpellAura._RegisterForTargets();
                    }
                }
            }
        }*/

        foreach (var effect in SpellEffects)
        if (effect != null && (effectMask & (1 << effect.EffectIndex)) > 0)
            HandleEffects(unit, effect.EffectIndex, SpellEffectHandleMode.HitTarget);

        return SpellMissInfo.None;
    }

    private void DoAllEffectsOnTarget(TargetInfo target)
    {
        if (target == null || target.Processed)
            return;

        target.Processed = true;                               // Target checked in apply effects procedure

        // Get mask of effects for target
        int mask = target.EffectMask;

        Unit unit = Caster.Character.Id == target.TargetId ? Caster : ArenaManager.FindUnit(target.TargetId);

        if (!unit)
            return;

        if (unit.IsAlive() != target.Alive)
            return;

        // Get original caster (if exist) and calculate damage/healing from him data
        Unit caster = OriginalCaster != null ? OriginalCaster : Caster;

        // Skip if m_originalCaster not avaiable
        if (caster == null)
            return;

        SpellMissInfo missInfo = target.MissCondition;

        // Need init unitTarget by default unit (can changed in code on reflect)
        // Or on missInfo != SPELL_MISS_NONE unitTarget undefined (but need in trigger subsystem)
        UnitTarget = unit;

        // Reset damage/healing counter
        Damage = target.Damage;
        Healing = -target.Damage;

        SpellAura = null; // Set aura to null for every target-make sure that pointer is not used for unit without aura applied

        Unit spellHitTarget = null;

        if (missInfo == SpellMissInfo.None)                     // In case spell hit target, do all effect on that target
            spellHitTarget = unit;
        else if (missInfo == SpellMissInfo.Reflect)             // In case spell reflect from target, do all effect on caster (if hit)
            if (target.ReflectResult == SpellMissInfo.None)     // If reflected spell hit caster -> do all effect on him
                spellHitTarget = Caster;

        if (spellHitTarget)
        {
            SpellMissInfo missInfo2 = DoSpellHitOnUnit(spellHitTarget, mask);

            if (missInfo2 != SpellMissInfo.None)
            {
                Damage = 0;
                spellHitTarget = null;
            }
        }

        // All calculated do it!
        // Do healing and triggers
        if (Healing > 0)
        {
            bool crit = target.Crit;
            int addhealth = Healing;
            //if (crit)
            //    addhealth = caster->SpellCriticalHealingBonus(SpellInfo, addhealth, NULL);

            //int32 gain = caster->HealBySpell(unitTarget, m_spellInfo, addhealth, crit);
            //Healing = gain;
        }
        // Do damage and triggers
        else if (Damage > 0)
        {
            // Fill base damage struct (unitTarget - is real spell target)
            SpellDamage damageInfo = new SpellDamage(caster, UnitTarget, SpellInfo.Id, SpellSchoolMask, CastId);

            // Add bonuses and fill damageInfo struct
            caster.CalculateSpellDamageTaken(damageInfo, Damage, SpellInfo, target.Crit);
            //caster.DealDamageMods(damageInfo.Target, damageInfo.damage, &damageInfo.absorb);

            Damage = damageInfo.Damage;

            caster.DealSpellDamage(damageInfo);
        }
        // Passive spell hits/misses or active spells only misses (only triggers)
        else
        {
            // Fill base damage struct (unitTarget - is real spell target)
            /*SpellNonMeleeDamage damageInfo(caster, unitTarget, m_spellInfo->Id, m_spellSchoolMask);
            procEx |= createProcExtendMask(&damageInfo, missInfo);
            // Do triggers for unit (reflect triggers passed on hit phase for correct drop charge)
            if (canEffectTrigger && missInfo != SPELL_MISS_REFLECT)
                caster->ProcDamageAndSpell(unit, procAttacker, procVictim, procEx, 0, m_attackType, m_spellInfo, m_triggeredByAuraSpell);*/
        }

        if (spellHitTarget)
        {
            // Needs to be called after dealing damage/healing to not remove breaking on damage auras
            //DoTriggersOnSpellHit(spellHitTarget, mask);

            //CallScriptAfterHitHandlers();
        }
    }

    private void HandleImmediate()
    {
        // process immediate effects (items, ground, etc.) also initialize some variables
        HandleImmediatePhase();

        foreach (var targetInfo in UniqueTargets)
            DoAllEffectsOnTarget(targetInfo);

        // spell is finished, perform some last features of the spell here
        HandleFinishPhase();

        if (SpellState != SpellState.Casting)
            Finish(true);
    }

    private void HandleImmediatePhase()
    {
        SpellAura = null;
        // initialize Diminishing Returns Data
        DiminishLevel = DiminishingLevels.Level1;
        DiminishGroup = DiminishingGroup.None;

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



    public void Prepare(SpellCastTargets targets, AuraEffect triggeredByAura = null)
    {
        InitiateExplicitTargets(targets);

        if(triggeredByAura != null)
            TriggeredByAuraSpell = triggeredByAura.SpellInfo;

        SpellState = SpellState.Preparing;

        SpellCastResult result = CheckCast(true);

        CastTime = SpellInfo.CastTime != null ? SpellInfo.CastTime.CastTime : 0;
        CastTimer = CastTime;

        if (CastTime == 0)
            Cast(true);
    }

    public void Cancel()
    {

    }

    public void Update(float timeDelta)
    {
        if (SpellState == SpellState.Preparing)
        {
            if (CastTimer > timeDelta)
                CastTimer -= timeDelta;
            else
                CastTimer = 0;

            if (CastTimer == 0)
                Cast(CastTime > 0);
        }
        else
            Finish(false);
    }

    public void Cast(bool skipCheck = false)
    {
        SpellManager.ApplySpellVisuals(Caster, SpellInfo);

        SpellManager.ApplySpellCastSound(Caster, SpellInfo);

        SelectSpellTargets();

        Caster.Character.SpellHistory.HandleCooldowns(SpellInfo, this);

        HandleImmediate();
    }

    public void Finish(bool ok = true)
    {
        SpellState = SpellState.Finished;
    }

    public void TakePower()
    {

    }
    

    public SpellCastResult CheckRange(bool strict)
    {
        // Don't check for instant cast spells
        if (!strict && CastTime == 0)
            return SpellCastResult.SpellCastOk;

        Unit target = Targets.UnitTarget;
        float minRange = 0.0f;
        float maxRange = 0.0f;
        float rangeMod = 0.0f;

        if (SpellInfo.Range != null)
        {
            if (SpellInfo.Range.Flags.HasFlag(SpellRangeFlag.Melee))
            {
                rangeMod = 3.0f + 4.0f / 3.0f;
            }
            else
            {
                float meleeRange = 0.0f;
                if (SpellInfo.Range.Flags.HasFlag(SpellRangeFlag.Ranged))
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
                return SpellCastResult.OutOfRange;

            if (minRange > 0.0f && Vector3.Distance(Caster.transform.position, target.transform.position) < minRange)
                return SpellCastResult.OutOfRange;
        }

        return SpellCastResult.SpellCastOk;
    }

    public SpellCastResult CheckCast(bool strict)
    {
        // check death state
        if (!Caster.IsAlive() && !SpellInfo.IsPassive())
            return SpellCastResult.CasterDead;

        // check cooldowns to prevent cheating
        if (!SpellInfo.IsPassive())
        {
            if (!Caster.Character.SpellHistory.IsReady(SpellInfo, IsIgnoringCooldowns()))
            {
                if (TriggeredByAuraSpell != null)
                    return SpellCastResult.DontReport;
                else
                    return SpellCastResult.NotReady;
            }
        }

        // Check global cooldown
        if (strict && !(TriggerCastFlags.HasFlag(TriggerCastFlags.IgnoreGcd) && HasGlobalCooldown()))
            return SpellCastResult.NotReady;

        // Check for line of sight for spells with dest
        /*if (m_targets.HasDst())
        {
            float x, y, z;
            m_targets.GetDstPos()->GetPosition(x, y, z);

            if (!m_spellInfo->HasAttribute(SPELL_ATTR2_CAN_TARGET_NOT_IN_LOS) && !DisableMgr::IsDisabledFor(DISABLE_TYPE_SPELL, m_spellInfo->Id, NULL, SPELL_DISABLE_LOS) && !m_caster->IsWithinLOS(x, y, z))
                return SPELL_FAILED_LINE_OF_SIGHT;
        }*/

        SpellCastResult castResult = SpellCastResult.SpellCastOk;

        // Triggered spells also have range check
        /// @todo determine if there is some flag to enable/disable the check
        castResult = CheckRange(strict);
        if (castResult != SpellCastResult.SpellCastOk)
            return castResult;

        return SpellCastResult.Success;
    }

    public int CalculateDamage(int effectIndex, Unit target, float var = 0)
    {
        return Caster.CalculateSpellDamage(target, SpellInfo, effectIndex, SpellValue.EffectBasePoints[effectIndex], var);
    }
}