using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public partial class Spell
    {
        public List<SpellEffectInfo> Effects { get; private set; }
        public SpellInfo SpellInfo { get; private set; }
        public Unit Caster { get; private set; }
        public Unit OriginalCaster { get; set; }

        public ulong CastId { get; private set; }
        public ulong OriginalCasterGuid { get; set; }
   
        public bool SkipCheck { get; private set; }
        public bool ExecutedCurrently { get; set; }

        public SpellSchoolMask SpellSchoolMask { get; set; }
        public WeaponAttackType AttackType { get; set; }
        public SpellState SpellState { get; set; }
        public SpellCastFlags SpellCastFlags { get; set; }
        public SpellDiminishingLevel DiminishLevel { get; set; }
        public SpellDiminishingGroup DiminishGroup { get; set; }

        public Vector3 DestTarget { get; set; }
        public int SpellHealing { get; set; }
        public float Variance { get; set; }

        public int EffectDamage { get; set; }
        public int EffectHealing { get; set; }
        public Aura SpellAura { get; set; }
        public SpellInfo TriggeredByAuraSpell { get; private set; }

        public List<Aura> UsedSpellMods { get; set; }
        public uint SpellVisual { get; set; }
        public uint PreCastSpell { get; set; }
        public bool CanReflect { get; private set; }
        public bool IsDelayedInstantCast { get; set; }
        public float DelayMoment { get; private set; }
        public long DelayStart { get; set; }
        public byte DelayAtDamageCount { get; set; }

        public byte RuneState { get; set; }
        public float CastTime { get; private set; }
        public float CastTimer { get; private set; }
        public float ChanneledDuration { get; set; }
        public bool ImmediateHandled { get; set; }

        public int ChannelTargetEffectMask { get; set; }
        public List<SpellTargetInfo> UniqueTargetInfo { get; set; }
        public SpellCastTargets Targets { get; set; }
        public SpellCustomErrors CustomError { get; private set; }

        public bool IsNextMeleeSwingSpell => throw new NotImplementedException();
        public bool IsTriggered => SpellCastFlags != 0;
        public bool IsIgnoringCooldowns => (SpellCastFlags & SpellCastFlags.IgnoreSpellAndCategoryCd) != 0;
        public bool IsAutoActionResetSpell => throw new NotImplementedException();
        public bool IsInterruptable => !ExecutedCurrently;
        public bool IsNeedSendToClient => throw new NotImplementedException();
        public SpellSlotType CurrentContainer => throw new NotImplementedException();

        private bool HasGlobalCooldown => Caster.SpellHistory.HasGlobalCooldown;

        internal Spell(Unit caster, SpellInfo info, SpellCastFlags spellFlags, ulong originalCasterId, bool skipCheck = false)
        {
            SpellInfo = info;
            Caster = caster;
            Effects = info.Effects;

            CustomError = SpellCustomErrors.None;
            SkipCheck = skipCheck;
            ExecutedCurrently = false;
            DelayStart = 0;
            DelayAtDamageCount = 0;
            SpellSchoolMask = info.SchoolMask;

            OriginalCasterGuid = originalCasterId != 0 ? originalCasterId : caster.NetworkId;
            OriginalCaster = OriginalCasterGuid == caster.NetworkId ? caster : caster.WorldManager.UnitManager.Find(OriginalCasterGuid);

            if (OriginalCaster != null && !OriginalCaster.IsValid)
                OriginalCaster = null;

            SpellState = SpellState.None;
            SpellCastFlags = spellFlags;
            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                SpellCastFlags = SpellCastFlags | SpellCastFlags.IgnoreCastInProgress | SpellCastFlags.CastDirectly;

            DestTarget = Vector3.zero;
            Variance = 0.0f;
            DiminishLevel = SpellDiminishingLevel.Level1;
            DiminishGroup = SpellDiminishingGroup.None;
            SpellHealing = 0;
            CastId = 0;
            PreCastSpell = 0;
            TriggeredByAuraSpell = null;
            SpellAura = null;

            IsDelayedInstantCast = false;

            RuneState = 0;
            CastTime = 0;
            CastTimer = 0;
            ChanneledDuration = 0;
            ImmediateHandled = false;

            ChannelTargetEffectMask = 0;

            CanReflect = SpellInfo.DamageClass == SpellDamageClass.Magic && !SpellInfo.HasAttribute(SpellAttributes.CantBeReflected) &&
                !SpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !SpellInfo.IsPassive() && !SpellInfo.IsPositive();

            UniqueTargetInfo = new List<SpellTargetInfo>();

            DelayMoment = 0;
            EffectDamage = 0;
            EffectHealing = 0;
        }

        internal void DoUpdate(int diffTime)
        {
            if (SpellState == SpellState.Preparing)
            {
                if (CastTimer > diffTime)
                    CastTimer -= diffTime;
                else
                    CastTimer = 0;

                if (Mathf.Approximately(CastTimer, 0))
                    Cast(CastTime > 0);
            }
            else
                Finish(false);
        }

        internal SpellCastResult Prepare(SpellCastTargets targets, AuraEffect triggeredByAura = null)
        {
            InitiateExplicitTargets(targets);

            if (triggeredByAura != null)
                TriggeredByAuraSpell = triggeredByAura.SpellInfo;

            SpellState = SpellState.Preparing;

            SpellCastResult result = CheckCast(true);

            if (SpellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreTargetCheck) && result == SpellCastResult.BadTargets)
                result = SpellCastResult.Success;

            if (result != SpellCastResult.Success)
            {
                if (triggeredByAura != null && triggeredByAura.IsPeriodic() && !triggeredByAura.BaseAura.IsPassive())
                    triggeredByAura.BaseAura.SetDuration(0);

                Finish(false);
                return result;
            }

            CastTime = SpellInfo.CastTime;
            CastTimer = CastTime;

            if (Mathf.Approximately(CastTime, 0.0f))
                Cast(true);

            return result;
        }

        private void Cast(bool skipCheck = false)
        {
            SelectSpellTargets();

            Caster.SpellHistory.HandleCooldowns(SpellInfo);

            HandleImmediate();
        }

        private void Cancel()
        {
            throw new NotImplementedException();
        }

        private void Finish(bool ok = true)
        {
            SpellState = SpellState.Finished;
        }

        #region Target Selection

        private void InitiateExplicitTargets(SpellCastTargets targets)
        {
            Targets = targets;
            Targets.OrigUnitTarget = Targets.UnitTarget;
        
            // this function tries to correct spell explicit targets for spell
            // client doesn't send explicit targets correctly sometimes - we need to fix such spells serverside
            SpellCastTargetFlags neededTargets = SpellInfo.ExplicitTargetMask;
            WorldEntity target = Targets.Target;
            if (target != null)
            {
                // check if object spellTarget is valid with needed spellTarget flags
                // for unit case allow corpse spellTarget mask because player with not released corpse is a unit spellTarget
                if (target is Unit && !neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                    targets.RemoveEntityTarget();
            }
            else
            {
                // try to select correct unit spellTarget if not provided by client or by serverside cast
                if (neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                {
                    Unit unit = null;
                    var playerCaster = Caster as Player;
                    // try to use player selection as a spellTarget
                    if (playerCaster != null)
                    {
                        // selection has to be found and to be valid spellTarget for the spell
                        Unit selectedUnit = playerCaster.WorldManager.UnitManager.Find(playerCaster.Target);
                        if (selectedUnit != null && SpellInfo.CheckExplicitTarget(Caster, selectedUnit) == SpellCastResult.Success)
                            unit = selectedUnit;
                    }

                    // didn't find anything - let's use self as spellTarget
                    if (unit == null && neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitAlly))
                        unit = Caster;

                    Targets.UnitTarget = unit;
                }
            }

            // check if spell needs dst spellTarget
            // if spellTarget isn't set try to use unit spellTarget if provided, else use self
            if (neededTargets.HasTargetFlag(SpellCastTargetFlags.DestLocation))
            {
                if (!Targets.HasDest)
                    Targets.SetDst(targets.Target ?? Caster);
            }
            else
                Targets.RemoveDst();

            if (neededTargets.HasTargetFlag(SpellCastTargetFlags.SourceLocation))
            {
                if (!targets.HasSource)
                    Targets.SetSrc(Caster);
            }
            else
                Targets.RemoveSrc();
        }

        private void SelectSpellTargets()
        {
            SelectExplicitTargets();

            int processedAreaEffectsMask = 0;
            foreach (var effect in Effects)
            {
                if (effect.ImplicitTargetFlags.HasTargetFlag(SpellCastTargetFlags.Unit))
                    Targets.TargetMask |= SpellCastTargetFlags.Unit;       

                SelectEffectImplicitTargets(effect, effect.MainTargeting, ref processedAreaEffectsMask);
                SelectEffectImplicitTargets(effect, effect.SecondaryTargeting, ref processedAreaEffectsMask);
                SelectEffectTypeImplicitTargets(effect);

                if (Targets.HasDest)
                    AddDestTarget(Targets.Destination, effect.Index);

                if (!SpellInfo.IsChanneled())
                    continue;

                int mask = 1 << effect.Index;
                foreach(var targetInfo in UniqueTargetInfo)
                    if ((targetInfo.EffectMask & mask) != 0)
                    {
                        ChannelTargetEffectMask |= mask;
                        break;
                    }
            }

            if (Targets.HasDest)
            {
                float speed = Targets.SpeedXY;
                if (speed > 0.0f)
                    DelayMoment = Mathf.FloorToInt(Targets.Distance2D / speed * TimeUtils.InMilliseconds);
            }
        }

        private void SelectExplicitTargets()
        {
            Unit target = Targets.UnitTarget;
            if (target == null)
                return;

            if (SpellInfo.ExplicitTargetMask.HasAnyFlag(SpellCastTargetFlags.UnitEnemy) || SpellInfo.ExplicitTargetMask.HasTargetFlag(SpellCastTargetFlags.Unit) && !Caster.IsFriendlyTo(target))
            {
                Unit redirect;
                switch (SpellInfo.DamageClass)
                {
                    case SpellDamageClass.Magic:
                        redirect = Caster.GetMagicHitRedirectTarget(target, SpellInfo);
                        break;
                    case SpellDamageClass.Melee:
                    case SpellDamageClass.Ranged:
                        redirect = Caster.GetMeleeHitRedirectTarget(target, SpellInfo);
                        break;
                    default:
                        redirect = null;
                        break;
                }
                if (redirect != null && redirect != target)
                    Targets.UnitTarget = redirect;
            }
        }

        private void SelectEffectImplicitTargets(SpellEffectInfo effect, TargetingType targetingType, ref int processedEffectMask)
        {
            int effectMask = 1 << effect.Index;
            if (targetingType.IsAreaLookup)
            {
                if ((effectMask & processedEffectMask) > 0)
                    return;

                // avoid recalculating similar effects
                foreach (var otherEffect in Effects)
                    if (effect.MainTargeting == otherEffect.MainTargeting && effect.SecondaryTargeting == otherEffect.SecondaryTargeting)
                        if (Mathf.Approximately(effect.CalcRadius(Caster, this), otherEffect.CalcRadius(Caster, this)))
                            effectMask |= 1 << otherEffect.Index;

                processedEffectMask |= effectMask;
            }

            switch (targetingType.SelectionCategory)
            {
                case SpellTargetSelection.Channel:
                    SelectImplicitChannelTargets(effect, targetingType);
                    break;
                case SpellTargetSelection.Nearby:
                    SelectImplicitNearbyTargets(effect, targetingType, effectMask);
                    break;
                case SpellTargetSelection.Cone:
                    SelectImplicitConeTargets(effect, targetingType, effectMask);
                    break;
                case SpellTargetSelection.Area:
                    SelectImplicitAreaTargets(effect, targetingType, effectMask);
                    break;
                case SpellTargetSelection.Default:
                    switch (targetingType.TargetEntities)
                    {
                        case SpellTargetEntities.None:
                            break;
                        case SpellTargetEntities.Source:
                            switch (targetingType.ReferenceType)
                            {
                                case SpellTargetReferences.Caster:
                                    Targets.SetSrc(Caster);
                                    break;
                                default:
                                    throw new NotImplementedException($"Not implemented {targetingType.ReferenceType} for {targetingType.TargetEntities}");
                            }
                            break;
                        case SpellTargetEntities.Dest:
                            switch (targetingType.ReferenceType)
                            {
                                case SpellTargetReferences.Caster:
                                    SelectImplicitCasterDestTargets(effect, targetingType);
                                    break;
                                case SpellTargetReferences.Target:
                                    SelectImplicitTargetDestTargets(effect, targetingType);
                                    break;
                                case SpellTargetReferences.Dest:
                                    SelectImplicitDestDestTargets(effect, targetingType);
                                    break;
                                default:
                                    throw new NotImplementedException($"Not implemented {targetingType.ReferenceType} for {targetingType.TargetEntities}");
                            }
                            break;
                        case SpellTargetEntities.Unit:
                        case SpellTargetEntities.UnitAndDest:
                        case SpellTargetEntities.GameEntity:
                            break;
                    }
                    break;
            }
        }

        private void SelectImplicitChannelTargets(SpellEffectInfo effect, TargetingType targetingType)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitNearbyTargets(SpellEffectInfo effect, TargetingType targetingType, int effMask)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitConeTargets(SpellEffectInfo effect, TargetingType targetingType, int effMask)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitAreaTargets(SpellEffectInfo effect, TargetingType targetingType, int effMask)
        {
            Unit referer = null;
            switch (targetingType.ReferenceType)
            {
                case SpellTargetReferences.Source:
                case SpellTargetReferences.Dest:
                case SpellTargetReferences.Caster:
                    referer = Caster;
                    break;
                case SpellTargetReferences.Target:
                    referer = Targets.UnitTarget;
                    break;
                case SpellTargetReferences.Last:
                {
                    for (int i = UniqueTargetInfo.Count - 1; i >= 0; i--)
                    {
                        if ((UniqueTargetInfo[i].EffectMask & (1 << effect.Index)) > 0)
                        {
                            referer = Caster.Map.FindMapEntity<Unit>(UniqueTargetInfo[i].TargetId, unit => unit.IsAlive);
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

            Vector3 center;
            switch (targetingType.ReferenceType)
            {
                case SpellTargetReferences.Source:
                    center = new Vector3(Targets.Source.Position.x, Targets.Source.Position.y, Targets.Source.Position.z);
                    break;
                case SpellTargetReferences.Dest:
                    center = new Vector3(Targets.Destination.Position.x, Targets.Destination.Position.y, Targets.Destination.Position.z);
                    break;
                case SpellTargetReferences.Caster:
                case SpellTargetReferences.Target:
                case SpellTargetReferences.Last:
                    center = referer.Position;
                    break;
                default:
                    throw new NotImplementedException($"Not implemented AreaTargets with {targetingType.ReferenceType} for {targetingType.TargetEntities}");
            }

            List<Unit> targets = new List<Unit>();
            float radius = effect.CalcRadius(Caster, this);
            Caster.Map.SearchAreaTargets(targets, radius, center, referer, targetingType.SelectionCheckType);

            foreach (var unit in targets)
                AddUnitTarget(unit, effMask, false, true, new Vector3(center.x, center.y, center.z));
        }

        private void SelectImplicitCasterDestTargets(SpellEffectInfo effect, TargetingType targetingType)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitTargetDestTargets(SpellEffectInfo effect, TargetingType targetingType)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitDestDestTargets(SpellEffectInfo effect, TargetingType targetingType)
        {
            throw new NotImplementedException();
        }

        private void SelectEffectTypeImplicitTargets(SpellEffectInfo effect)
        {
            // add explicit spellTarget or self to the spellTarget list
            switch (effect.ExplicitTargetType)
            {
                case SpellExplicitTargetType.Explicit:
                    AddUnitTarget(Targets.UnitTarget ?? Caster, 1 << effect.Index, false);
                    break;
                case SpellExplicitTargetType.Caster:
                    AddUnitTarget(Caster, 1 << effect.Index, false);
                    break;
                case SpellExplicitTargetType.None:
                    return;
                default:
                    return;
            }
        }

        private void SearchAreaTargets(List<WorldEntity> targets, float range, Vector3 position, Unit referer, SpellTargetEntities entityType, SpellTargetChecks selectType) { throw new NotImplementedException(); }

        private void SearchChainTargets(List<WorldEntity> targets, int chainTargets, WorldEntity target, SpellTargetEntities entityType, SpellTargetChecks selectType, bool isChainHeal) { throw new NotImplementedException(); }

        private WorldEntity SearchNearbyTarget(float range, SpellTargetEntities entityType, SpellTargetChecks selectionType) { throw new NotImplementedException(); }

        private void AddUnitTarget(Unit target, int effectMask, bool checkIfValid = true, bool implicitTarget = true, Vector3 losPosition = default)
        {
            int validEffectMask = 0;
            foreach (var effect in Effects)
                if (effect != null && (effectMask & (1 << effect.Index)) != 0)
                    validEffectMask |= 1 << effect.Index;

            effectMask &= validEffectMask;

            // no effects left
            if (effectMask == 0)
                return;

            /*// Check for effect immune skip if immuned
        for (SpellEffectInfo const* effect : GetEffects())
            if (effect && spellTarget->IsImmunedToSpellEffect(m_spellInfo, effect->EffectIndex))
                effectMask &= ~(1 << effect->EffectIndex);*/

            // Lookup spellTarget in already in list
            var sameTargetInfo = UniqueTargetInfo.Find(unit => unit.TargetId == target.NetworkId);
            if (sameTargetInfo != null)
            {
                sameTargetInfo.EffectMask |= effectMask;             // Immune effects removed from mask
                return;
            }

            // This is new spellTarget calculate data for him

            // Get spell hit result on spellTarget
            SpellTargetInfo spellTargetInfo = new SpellTargetInfo();
            spellTargetInfo.TargetId = target.NetworkId;                          // Store spellTarget GUID
            spellTargetInfo.EffectMask = effectMask;                         // Store all effects not immune
            spellTargetInfo.Processed = false;                               // Effects not apply on spellTarget
            spellTargetInfo.Alive = target.IsAlive;
            spellTargetInfo.Damage = 0;
            spellTargetInfo.Crit = false;
            spellTargetInfo.ScaleAura = false;

            // Calculate hit result
            if (OriginalCaster != null)
            {
                spellTargetInfo.MissCondition = OriginalCaster.SpellHitResult(target, SpellInfo, CanReflect);
                if (SkipCheck && spellTargetInfo.MissCondition != SpellMissType.Immune)
                    spellTargetInfo.MissCondition = SpellMissType.None;
            }
            else
                spellTargetInfo.MissCondition = SpellMissType.Evade; //SPELL_MISS_NONE;

            // Spell have speed - need calculate incoming time
            // Incoming time is zero for self casts. At least I think so.
            if (SpellInfo.Speed > 0.0f && Caster != target)
            {
                // calculate spell incoming interval
                //float dist = Vector3.Distance(Caster.Position, spellTarget.Position);

                //if (dist < 5.0f)
                //    dist = 5.0f;

                spellTargetInfo.Delay = SpellInfo.Speed;

                // Calculate minimum incoming time
                if (DelayMoment == 0 || DelayMoment > spellTargetInfo.Delay)
                    DelayMoment = spellTargetInfo.Delay;
            }
            else
                spellTargetInfo.Delay = 0.0f;

            // If spellTarget reflect spell back to caster
            /*if (spellTargetInfo.missCondition == SPELL_MISS_REFLECT)
        {
            // Calculate reflected spell result on caster
            spellTargetInfo.reflectResult = m_caster->SpellHitResult(m_caster, m_spellInfo, m_canReflect);

            if (spellTargetInfo.reflectResult == SPELL_MISS_REFLECT)     // Impossible reflect again, so simply deflect spell
                spellTargetInfo.reflectResult = SPELL_MISS_PARRY;

            // Increase time interval for reflected spells by 1.5
            spellTargetInfo.timeDelay += spellTargetInfo.timeDelay >> 1;
        }
        else
            spellTargetInfo.reflectResult = SPELL_MISS_NONE;*/

            // Add spellTarget to list
            UniqueTargetInfo.Add(spellTargetInfo);
        }

        private void AddDestTarget(WorldEntity destination, int effIndex) { throw new NotImplementedException(); }

        #endregion

        #region Cast and Target Validation

        private SpellCastResult CheckCast(bool strict)
        {
            // check death state
            if (!Caster.IsAlive && !SpellInfo.IsPassive() && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileDead))
                return SpellCastResult.CasterDead;

            // check cooldowns to prevent cheating
            if (!SpellInfo.IsPassive() && !Caster.SpellHistory.IsReady(SpellInfo))
            {
                if (TriggeredByAuraSpell != null)
                    return SpellCastResult.DontReport;
                else
                    return SpellCastResult.NotReady;
            }

            // Check global cooldown
            if (strict && !SpellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreGcd) && HasGlobalCooldown)
                return SpellCastResult.NotReady;

            // Check for line of sight for spells with dest
            /*if (m_targets.HasDst())
        {
            float x, y, z;
            m_targets.GetDstPos()->GetPosition(x, y, z);

            if (!m_spellInfo->HasAttribute(SPELL_ATTR2_CAN_TARGET_NOT_IN_LOS) && !DisableMgr::IsDisabledFor(DISABLE_TYPE_SPELL, m_spellInfo->Id, NULL, SPELL_DISABLE_LOS) && !m_caster->IsWithinLOS(x, y, z))
                return SPELL_FAILED_LINE_OF_SIGHT;
        }*/

            SpellCastResult castResult = CheckRange(strict);
            if (castResult != SpellCastResult.Success)
                return castResult;

            return SpellCastResult.Success;
        }

        private SpellCastResult CheckRange(bool strict)
        {
            // Don't check for instant cast spells
            if (!strict && CastTime == 0)
                return SpellCastResult.Success;

            Unit target = Targets.UnitTarget;
            float minRange = 0.0f;
            float maxRange = 0.0f;
            float rangeMod = 0.0f;

            if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlags.Melee))
            {
                rangeMod = 3.0f + 4.0f / 3.0f;
            }
            else
            {
                float meleeRange = 0.0f;
                if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlags.Ranged))
                    meleeRange = 3.0f + 4.0f / 3.0f;

                minRange = Caster.GetSpellMinRangeForTarget(target, SpellInfo) + meleeRange;
                maxRange = Caster.GetSpellMaxRangeForTarget(target, SpellInfo);
            }

            maxRange += rangeMod;

            minRange *= minRange;
            maxRange *= maxRange;

            if (target != null && target != Caster)
            {
                if (Vector3.Distance(Caster.Position, target.Position) > maxRange)
                    return SpellCastResult.OutOfRange;

                if (minRange > 0.0f && Vector3.Distance(Caster.Position, target.Position) < minRange)
                    return SpellCastResult.OutOfRange;
            }

            return SpellCastResult.Success;
        }

        private SpellCastResult CheckPower() { throw new NotImplementedException(); }

        private SpellCastResult CheckCasterAuras() { throw new NotImplementedException(); }

        #endregion

        #region Spell Processing

        private void HandleLaunchPhase()
        {
            throw new NotImplementedException();
        }

        private void HandleImmediate()
        {
            // process immediate effects (items, ground, etc.) also initialize some variables
            HandleImmediatePhase();

            foreach (var targetInfo in UniqueTargetInfo)
                DoAllEffectsOnTarget(targetInfo);

            // spell is finished, perform some last features of the spell here
            HandleFinishPhase();

            if (SpellState != SpellState.Casting)
                Finish();
        }

        private void HandleImmediatePhase()
        {
            SpellAura = null;
            // initialize Diminishing Returns Data
            DiminishLevel = SpellDiminishingLevel.Level1;
            DiminishGroup = SpellDiminishingGroup.None;

            foreach (var effect in Effects)
                DoEffectOnTarget(null, effect, SpellEffectHandleMode.Hit);
        }

        private void HandleFinishPhase()
        {

        }

        private void DoAllEffectsOnTarget(SpellTargetInfo spellTarget)
        {
            if (spellTarget == null || spellTarget.Processed)
                return;

            spellTarget.Processed = true;
            int mask = spellTarget.EffectMask;

            Unit unit = Caster.NetworkId == spellTarget.TargetId ? Caster : Caster.Map.FindMapEntity<Unit>(spellTarget.TargetId);
            if (unit?.IsAlive != spellTarget.Alive)
                return;

            Unit caster = OriginalCaster ?? Caster;
            if (caster == null)
                return;

            SpellMissType missType = spellTarget.MissCondition;

            EffectDamage = spellTarget.Damage;
            EffectHealing = -spellTarget.Damage;
            SpellAura = null;

            Unit spellHitTarget = null;
            if (missType == SpellMissType.None)
                spellHitTarget = unit;
            else if (missType == SpellMissType.Reflect && spellTarget.ReflectResult == SpellMissType.None)
                spellHitTarget = Caster;

            if (spellHitTarget != null)
            {
                SpellMissType missInfo2 = DoSpellHitOnUnit(spellHitTarget, mask);

                if (missInfo2 != SpellMissType.None)
                    EffectDamage = 0;
                else
                    EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellHit, spellHitTarget, SpellInfo.Id);
            }

            if (EffectHealing > 0)
            {
                bool crit = spellTarget.Crit;
                int addhealth = SpellHealing;
                if (crit)
                    addhealth = caster.SpellCriticalHealingBonus(SpellInfo, addhealth, null);

                int gain = caster.HealBySpell(unit, SpellInfo, addhealth, crit);
                EffectHealing = gain;
            }
            else if (EffectDamage > 0)
            {
                SpellCastDamageInfo damageInfoInfo = new SpellCastDamageInfo(caster, unit, SpellInfo.Id, SpellSchoolMask, CastId);
                EffectDamage = caster.CalculateSpellDamageTaken(damageInfoInfo, EffectDamage, SpellInfo);
                caster.DamageBySpell(damageInfoInfo);
            }
        }

        private void DoAllEffectOnLaunchTarget(SpellTargetInfo spellTargetInfo)
        {
            throw new NotImplementedException();
        }
        
        private void DoEffectOnTarget(Unit unitTarget, SpellEffectInfo effect, SpellEffectHandleMode mode)
        {
            effect.Handle(this, unitTarget, mode);
        }

        private SpellMissType DoSpellHitOnUnit(Unit unit, int effectMask)
        {
            if (unit == null || effectMask == 0)
                return SpellMissType.Evade;

            foreach (var effect in Effects)
                if (effect != null && (effectMask & (1 << effect.Index)) > 0)
                    DoEffectOnTarget(unit, effect, SpellEffectHandleMode.HitTarget);

            return SpellMissType.None;
        }

        private void Delayed() { throw new NotImplementedException(); }

        private void DelayedChannel() { throw new NotImplementedException(); }

        private void PrepareTargetProcessing() { throw new NotImplementedException(); }

        private void FinishTargetProcessing() { throw new NotImplementedException(); }

        private void TriggerGlobalCooldown() { throw new NotImplementedException(); }

        private void CancelGlobalCooldown() { throw new NotImplementedException(); }

        private void ConsumeResources(bool isSpellHit) { throw new NotImplementedException(); }

        #endregion
    }
}