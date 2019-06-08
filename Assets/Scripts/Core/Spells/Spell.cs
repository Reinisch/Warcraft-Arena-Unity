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
        public TriggerCastFlags TriggerCastFlags { get; set; }
        public DiminishingLevels DiminishLevel { get; set; }
        public DiminishingGroup DiminishGroup { get; set; }

        public GameEntity GameEntityTarget { get; set; }
        public Vector3 DestTarget { get; set; }
        public int SpellDamage { get; set; }
        public int SpellHealing { get; set; }
        public float Variance { get; set; }

        public ProcFlags ProcAttacker { get; set; }
        public ProcFlags ProcVictim { get; set; }

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
        public List<PowerCostData> PowerCost { get; set; }

        public int ChannelTargetEffectMask { get; set; }
        public List<TargetInfo> UniqueTargetInfo { get; set; }
        public List<GameEntityTargetInfo> UniqueGameEntityTargetInfo { get; set; }
        public SpellCastTargets Targets { get; set; }

        public SpellCustomErrors CustomError { get; private set; }

        public bool IsNextMeleeSwingSpell => throw new NotImplementedException();
        public bool IsTriggered => (TriggerCastFlags & TriggerCastFlags.FullMask) != 0;
        public bool IsIgnoringCooldowns => (TriggerCastFlags & TriggerCastFlags.IgnoreSpellAndCategoryCd) != 0;
        public bool IsChannelActive => Caster.GetUintValue(EntityFields.ChannelSpell) != 0;
        public bool IsAutoActionResetSpell => throw new NotImplementedException();
        public bool IsInterruptable => !ExecutedCurrently;
        public bool IsNeedSendToClient => throw new NotImplementedException();
        public CurrentSpellTypes CurrentContainer => throw new NotImplementedException();

        public List<HitTriggerSpell> HitTriggerSpells { get; private set; }
        private bool HasGlobalCooldown => Caster.SpellHistory.HasGlobalCooldown();

        public Spell(Unit caster, SpellInfo info, TriggerCastFlags triggerFlags, ulong originalCasterId, bool skipCheck = false)
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
            TriggerCastFlags = triggerFlags;
            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                TriggerCastFlags = TriggerCastFlags | TriggerCastFlags.IgnoreCastInProgress | TriggerCastFlags.CastDirectly;

            GameEntityTarget = null;
            DestTarget = Vector3.zero;
            Variance = 0.0f;
            DiminishLevel = DiminishingLevels.Level1;
            DiminishGroup = DiminishingGroup.None;
            SpellDamage = 0;
            SpellHealing = 0;
            ProcAttacker = 0;
            ProcVictim = 0;
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

            UniqueTargetInfo = new List<TargetInfo>();
            UniqueGameEntityTargetInfo = new List<GameEntityTargetInfo>();

            DelayMoment = 0;
            EffectDamage = 0;
            EffectHealing = 0;
        }

        public void DoUpdate(int diffTime)
        {
            if (SpellState == SpellState.Preparing)
            {
                if (CastTimer > diffTime)
                    CastTimer -= diffTime;
                else
                    CastTimer = 0;

                if (CastTimer == 0)
                    Cast(CastTime > 0);
            }
            else
                Finish(false);
        }

        public void Prepare(SpellCastTargets targets, AuraEffect triggeredByAura = null)
        {
            InitiateExplicitTargets(targets);

            if (triggeredByAura != null)
                TriggeredByAuraSpell = triggeredByAura.SpellInfo;

            SpellState = SpellState.Preparing;

            SpellCastResult result = CheckCast(true);

            if ((TriggerCastFlags & TriggerCastFlags.IgnoreTargetCheck) != 0 && result == SpellCastResult.BadTargets)
                result = SpellCastResult.Success;
            if (result != SpellCastResult.Success)
            {
                if (triggeredByAura != null && triggeredByAura.IsPeriodic() && !triggeredByAura.BaseAura.IsPassive())
                {
                    triggeredByAura.BaseAura.SetDuration(0);
                }

                if (Caster.EntityType == EntityType.Player)
                {
                    var player = (Player)Caster;
                    player.RestoreSpellMods(this);
                    // cleanup after mod system
                    // triggered spell pointer can be not removed in some cases
                    player.SetSpellModTakingSpell(this, false);
                }
                //SendCastResult(result);

                Finish(false);
                return;
            }

            CastTime = SpellInfo.CastTime;
            CastTimer = CastTime;

            if (Mathf.Approximately(CastTime, 0.0f))
                Cast(true);
        }

        private void Cast(bool skipCheck = false)
        {
            SelectSpellTargets();

            Caster.SpellHistory.HandleCooldowns(SpellInfo, this);

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
                // check if object target is valid with needed target flags
                // for unit case allow corpse target mask because player with not released corpse is a unit target
                if (target is Unit && !neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                    targets.RemoveEntityTarget();
                if (target is GameEntity && !neededTargets.HasAnyFlag(SpellCastTargetFlags.GameEntity))
                    targets.RemoveEntityTarget();
            }
            else
            {
                // try to select correct unit target if not provided by client or by serverside cast
                if (neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                {
                    Unit unit = null;
                    var playerCaster = Caster as Player;
                    // try to use player selection as a target
                    if (playerCaster != null)
                    {
                        // selection has to be found and to be valid target for the spell
                        Unit selectedUnit = playerCaster.WorldManager.UnitManager.Find(playerCaster.GetTarget());
                        if (selectedUnit != null && SpellInfo.CheckExplicitTarget(Caster, selectedUnit) == SpellCastResult.Success)
                            unit = selectedUnit;
                    }

                    // didn't find anything - let's use self as target
                    if (unit == null && neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitAlly))
                        unit = Caster;

                    Targets.UnitTarget = unit;
                }
            }

            // check if spell needs dst target
            // if target isn't set try to use unit target if provided, else use self
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
                case TargetSelections.Channel:
                    SelectImplicitChannelTargets(effect, targetingType);
                    break;
                case TargetSelections.Nearby:
                    SelectImplicitNearbyTargets(effect, targetingType, effectMask);
                    break;
                case TargetSelections.Cone:
                    SelectImplicitConeTargets(effect, targetingType, effectMask);
                    break;
                case TargetSelections.Area:
                    SelectImplicitAreaTargets(effect, targetingType, effectMask);
                    break;
                case TargetSelections.Default:
                    switch (targetingType.TargetEntities)
                    {
                        case TargetEntities.None:
                            break;
                        case TargetEntities.Source:
                            switch (targetingType.ReferenceType)
                            {
                                case TargetReferences.Caster:
                                    Targets.SetSrc(Caster);
                                    break;
                                default:
                                    throw new NotImplementedException($"Not implemented {targetingType.ReferenceType} for {targetingType.TargetEntities}");
                            }
                            break;
                        case TargetEntities.Dest:
                            switch (targetingType.ReferenceType)
                            {
                                case TargetReferences.Caster:
                                    SelectImplicitCasterDestTargets(effect, targetingType);
                                    break;
                                case TargetReferences.Target:
                                    SelectImplicitTargetDestTargets(effect, targetingType);
                                    break;
                                case TargetReferences.Dest:
                                    SelectImplicitDestDestTargets(effect, targetingType);
                                    break;
                                default:
                                    throw new NotImplementedException($"Not implemented {targetingType.ReferenceType} for {targetingType.TargetEntities}");
                            }
                            break;
                        case TargetEntities.Unit:
                        case TargetEntities.UnitAndDest:
                        case TargetEntities.GameEntity:
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
                case TargetReferences.Source:
                    center = new Vector3(Targets.Source.Position.x, Targets.Source.Position.y, Targets.Source.Position.z);
                    break;
                case TargetReferences.Dest:
                    center = new Vector3(Targets.Destination.Position.x, Targets.Destination.Position.y, Targets.Destination.Position.z);
                    break;
                case TargetReferences.Caster:
                case TargetReferences.Target:
                case TargetReferences.Last:
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
            // select spell implicit targets based on effect type
            if (effect.ExplicitTargetType == 0)
                return;

            SpellCastTargetFlags targetMask = effect.GetMissingTargetMask();
            if (targetMask == 0)
                return;

            WorldEntity target = null;
            switch (effect.ExplicitTargetType)
            {
                // add explicit object target or self to the target map
                case ExplicitTargetTypes.Explicit:
                    // player which not released his spirit is Unit, but target flag for it is CorpseMask
                    if (targetMask.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                        target = Targets.UnitTarget ?? Caster;
               
                    if (targetMask.HasAnyFlag(SpellCastTargetFlags.GameEntity))
                        target = Targets.GameEntityTarget;
                    break;
                // add self to the target map
                case ExplicitTargetTypes.Caster:
                    if (targetMask.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                        target = Caster;
                    break;
            }

            if (target is Unit)
                AddUnitTarget((Unit)target, 1 << effect.Index, false);
            else if (target is GameEntity)
                AddGameEntityTarget((GameEntity)target, 1 << effect.Index);
        }

        private void SearchAreaTargets(List<WorldEntity> targets, float range, Vector3 position, Unit referer, TargetEntities entityType, TargetChecks selectType) { throw new NotImplementedException(); }

        private void SearchChainTargets(List<WorldEntity> targets, int chainTargets, WorldEntity target, TargetEntities entityType, TargetChecks selectType, bool isChainHeal) { throw new NotImplementedException(); }

        private WorldEntity SearchNearbyTarget(float range, TargetEntities entityType, TargetChecks selectionType) { throw new NotImplementedException(); }

        private GameEntity SearchSpellFocus() { throw new NotImplementedException(); }

        private void AddUnitTarget(Unit target, int effectMask, bool checkIfValid = true, bool implicitTarget = true, Vector3 losPosition = default)
        {
            int validEffectMask = 0;
            foreach (var effect in Effects)
                if (effect != null && (effectMask & (1 << effect.Index)) != 0 && CheckEffectTarget(target, effect, losPosition))
                    validEffectMask |= 1 << effect.Index;

            effectMask &= validEffectMask;

            // no effects left
            if (effectMask == 0)
                return;

            /*// Check for effect immune skip if immuned
        for (SpellEffectInfo const* effect : GetEffects())
            if (effect && target->IsImmunedToSpellEffect(m_spellInfo, effect->EffectIndex))
                effectMask &= ~(1 << effect->EffectIndex);*/

            // Lookup target in already in list
            var sameTargetInfo = UniqueTargetInfo.Find(unit => unit.TargetId == target.NetworkId);
            if (sameTargetInfo != null)
            {
                sameTargetInfo.EffectMask |= effectMask;             // Immune effects removed from mask
                return;
            }

            // This is new target calculate data for him

            // Get spell hit result on target
            TargetInfo targetInfo = new TargetInfo();
            targetInfo.TargetId = target.NetworkId;                          // Store target GUID
            targetInfo.EffectMask = effectMask;                         // Store all effects not immune
            targetInfo.Processed = false;                               // Effects not apply on target
            targetInfo.Alive = target.IsAlive;
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
                //float dist = Vector3.Distance(Caster.Position, target.Position);

                //if (dist < 5.0f)
                //    dist = 5.0f;

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
            UniqueTargetInfo.Add(targetInfo);
        }

        private void AddGameEntityTarget(GameEntity target, int effectMask) { throw new NotImplementedException(); }

        private void AddDestTarget(WorldEntity destination, int effIndex) { throw new NotImplementedException(); }

        #endregion

        #region Cast and Target Validation

        private SpellCastResult CheckCast(bool strict)
        {
            // check death state
            if (!Caster.IsAlive && !SpellInfo.IsPassive())
                return SpellCastResult.CasterDead;

            // check cooldowns to prevent cheating
            if (!SpellInfo.IsPassive())
            {
                if (!Caster.SpellHistory.IsReady(SpellInfo, IsIgnoringCooldowns))
                {
                    if (TriggeredByAuraSpell != null)
                        return SpellCastResult.DontReport;
                    else
                        return SpellCastResult.NotReady;
                }
            }

            // Check global cooldown
            if (strict && !TriggerCastFlags.HasTargetFlag(TriggerCastFlags.IgnoreGcd) && HasGlobalCooldown)
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

            if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlag.Melee))
            {
                rangeMod = 3.0f + 4.0f / 3.0f;
            }
            else
            {
                float meleeRange = 0.0f;
                if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlag.Ranged))
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

        private SpellCastResult CheckPetCast(Unit target) { throw new NotImplementedException(); }

        private bool CheckEffectTarget(Unit target, SpellEffectInfo effect, Vector3 losPosition) { throw new NotImplementedException(); }

        private bool CheckEffectTarget(GameEntity target, SpellEffectInfo effect) { throw new NotImplementedException(); }

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

        private long HandleDelayed(long offset)
        {
            throw new NotImplementedException();
        }

        private void HandleImmediatePhase()
        {
            SpellAura = null;
            // initialize Diminishing Returns Data
            DiminishLevel = DiminishingLevels.Level1;
            DiminishGroup = DiminishingGroup.None;

            foreach (var effect in Effects)
                DoEffectOnTarget(null, effect, SpellEffectHandleMode.Hit);
        }

        private void HandleFinishPhase()
        {

        }

        private void DoAllEffectsOnTarget(TargetInfo target)
        {
            if (target == null || target.Processed)
                return;

            target.Processed = true;
            int mask = target.EffectMask;

            Unit unit = Caster.NetworkId == target.TargetId ? Caster : Caster.Map.FindMapEntity<Unit>(target.TargetId);
            if (unit?.IsAlive != target.Alive)
                return;

            Unit caster = OriginalCaster ?? Caster;
            if (caster == null)
                return;

            SpellMissInfo missInfo = target.MissCondition;

            EffectDamage = target.Damage;
            EffectHealing = -target.Damage;

            SpellAura = null;

            Unit spellHitTarget = null;
            if (missInfo == SpellMissInfo.None)
                spellHitTarget = unit;
            else if (missInfo == SpellMissInfo.Reflect && target.ReflectResult == SpellMissInfo.None)
                spellHitTarget = Caster;

            if (spellHitTarget != null)
            {
                SpellMissInfo missInfo2 = DoSpellHitOnUnit(spellHitTarget, mask);

                if (missInfo2 != SpellMissInfo.None)
                {
                    EffectDamage = 0;
                    spellHitTarget = null;
                }
                else
                    EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellHit, spellHitTarget, SpellInfo.Id);
            }

            // All calculated do it!
            // Do healing and triggers
            if (EffectHealing > 0)
            {
                bool crit = target.Crit;
                int addhealth = SpellHealing;
                if (crit)
                    addhealth = caster.SpellCriticalHealingBonus(SpellInfo, addhealth, null);

                int gain = caster.HealBySpell(unit, SpellInfo, addhealth, crit);
                EffectHealing = gain;
            }
            // Do damage and triggers
            else if (EffectDamage > 0)
            {
                // Fill base damage struct (unitTarget - is real spell target)
                SpellNonMeleeDamage damageInfo = new SpellNonMeleeDamage(caster, unit, SpellInfo.Id, SpellSchoolMask, CastId);

                // Add bonuses and fill damageInfo struct
                caster.CalculateSpellDamageTaken(damageInfo, EffectDamage, SpellInfo, WeaponAttackType.BaseAttack, target.Crit);
                //caster.DealDamageMods(damageInfo.Target, damageInfo.damage, &damageInfo.absorb);

                EffectDamage = damageInfo.Damage;

                caster.DealSpellDamage(damageInfo, false);
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

            if (spellHitTarget != null)
            {
                // Needs to be called after dealing damage/healing to not remove breaking on damage auras
                //DoTriggersOnSpellHit(spellHitTarget, mask);

                //CallScriptAfterHitHandlers();
            }
        }

        private void DoAllEffectOnLaunchTarget(TargetInfo targetInfo, ref float multiplier)
        {
            throw new NotImplementedException();
        }
        
        private void DoEffectOnTarget(Unit unitTarget, SpellEffectInfo effect, SpellEffectHandleMode mode)
        {
            //Debug.LogFormat($"Spell: {SpellInfo.Id} Handling effect: {effect.EffectType}");

            SpellDamage += CalculateDamage(effect.Index, unitTarget);
            effect.Handle(this, unitTarget, mode);
        }

        private void DoTriggersOnSpellHit(Unit unit, int effMask)
        {
            throw new NotImplementedException();
        }

        private SpellMissInfo DoSpellHitOnUnit(Unit unit, int effectMask)
        {
            if (unit == null || effectMask == 0)
                return SpellMissInfo.Evade;

            foreach (var effect in Effects)
                if (effect != null && (effectMask & (1 << effect.Index)) > 0)
                    DoEffectOnTarget(unit, effect, SpellEffectHandleMode.HitTarget);

            return SpellMissInfo.None;
        }

        private int CalculateDamage(int effectIndex, Unit target)
        {
            return Caster.CalculateSpellDamage(target, SpellInfo, effectIndex, 100);
        }


        private void Delayed() { throw new NotImplementedException(); }

        private void DelayedChannel() { throw new NotImplementedException(); }

        private void PrepareTargetProcessing() { throw new NotImplementedException(); }

        private void FinishTargetProcessing() { throw new NotImplementedException(); }

        private void TriggerGlobalCooldown() { throw new NotImplementedException(); }

        private void CancelGlobalCooldown() { throw new NotImplementedException(); }


        private void TakePower() { throw new NotImplementedException(); }

        private void TakeRunePower(bool didHit) { throw new NotImplementedException(); }

        private void TakeReagents() { throw new NotImplementedException(); }

        private void TakeCastItem() { throw new NotImplementedException(); }

        #endregion

        #region Spellcast Results

        private static void SendCastResult(Player caster, SpellInfo spellInfo, int spellVisual, Guid castCount, SpellCastResult result, SpellCustomErrors customError = SpellCustomErrors.None) { throw new NotImplementedException(); }

        private void SendCastResult(SpellCastResult result) { throw new NotImplementedException(); }

        private void SendPetCastResult(SpellCastResult result) { throw new NotImplementedException(); }

        private void SendSpellStart() { throw new NotImplementedException(); }

        private void SendSpellGo() { throw new NotImplementedException(); }

        private void SendSpellCooldown() { throw new NotImplementedException(); }

        private void SendSpellExecuteLog() { throw new NotImplementedException(); }

        private void ExecuteLogEffectTakeTargetPower(int effIndex, Unit target, int powerType, int points, float amplitude) { throw new NotImplementedException(); }

        private void ExecuteLogEffectExtraAttacks(int effIndex, Unit victim, int numAttacks) { throw new NotImplementedException(); }

        private void ExecuteLogEffectInterruptCast(int effIndex, Unit victim, int spellId) { throw new NotImplementedException(); }

        private void ExecuteLogEffectDurabilityDamage(int effIndex, Unit victim, int itemId, int amount) { throw new NotImplementedException(); }

        private void ExecuteLogEffectOpenLock(int effIndex, Entity obj) { throw new NotImplementedException(); }

        private void ExecuteLogEffectCreateItem(int effIndex, int entry) { throw new NotImplementedException(); }

        private void ExecuteLogEffectDestroyItem(int effIndex, int entry) { throw new NotImplementedException(); }

        private void ExecuteLogEffectSummonObject(int effIndex, WorldEntity obj) { throw new NotImplementedException(); }

        private void ExecuteLogEffectUnsummonObject(int effIndex, WorldEntity obj) { throw new NotImplementedException(); }

        private void ExecuteLogEffectResurrect(int effIndex, Unit target) { throw new NotImplementedException(); }

        private void SendInterrupted(int result) { throw new NotImplementedException(); }

        private void SendChannelUpdate(int time) { throw new NotImplementedException(); }

        private void SendChannelStart(int duration) { throw new NotImplementedException(); }

        private void SendResurrectRequest(Player target) { throw new NotImplementedException(); }

        #endregion
    }
}