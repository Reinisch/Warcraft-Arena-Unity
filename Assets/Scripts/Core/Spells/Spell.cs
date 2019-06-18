using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public partial class Spell
    {
        private SpellManager SpellManager { get; }

        internal SpellState SpellState { get; set; }
        internal SpellExecutionState ExecutionState { get; set; }

        public List<SpellEffectInfo> Effects => SpellInfo.Effects;
        public SpellInfo SpellInfo { get; private set; }
        public Unit Caster { get; private set; }
        public Unit OriginalCaster { get; set; }

        public SpellSchoolMask SpellSchoolMask { get; set; }
        public SpellCastFlags SpellCastFlags { get; set; }
        public SpellDiminishingLevel DiminishLevel { get; set; }
        public SpellDiminishingGroup DiminishGroup { get; set; }

        public Vector3 DestTarget { get; set; }
        public int SpellHealing { get; set; }
        public float Variance { get; set; }

        public int EffectDamage { get; set; }
        public int EffectHealing { get; set; }
        public Aura SpellAura { get; set; }

        public bool CanReflect { get; private set; }
        public float DelayMoment { get; private set; }

        public int CastTime { get; private set; }
        public int CastTimeLeft { get; private set; }

        public List<SpellTargetInfo> UniqueTargetInfo { get; set; }
        public SpellCastTargets Targets { get; set; }

        internal Spell(Unit caster, SpellInfo info, SpellCastFlags spellFlags)
        {
            Logging.LogSpell($"Created new spell, current count: {++SpellManager.SpellAliveCount}");

            SpellManager = caster.WorldManager.SpellManager;

            Caster = OriginalCaster = caster;
            SpellInfo = info;
            SpellCastFlags = spellFlags;
            SpellSchoolMask = info.SchoolMask;

            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                SpellCastFlags = SpellCastFlags | SpellCastFlags.IgnoreCastInProgress | SpellCastFlags.CastDirectly;

            DestTarget = Vector3.zero;
            Variance = 0.0f;
            DiminishLevel = SpellDiminishingLevel.Level1;
            DiminishGroup = SpellDiminishingGroup.None;
            SpellHealing = 0;
            SpellAura = null;

            CastTime = 0;
            CastTimeLeft = 0;

            CanReflect = SpellInfo.DamageClass == SpellDamageClass.Magic && !SpellInfo.HasAttribute(SpellAttributes.CantBeReflected) &&
                !SpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !SpellInfo.IsPassive() && !SpellInfo.IsPositive();

            DelayMoment = 0;
            EffectDamage = 0;
            EffectHealing = 0;

            UniqueTargetInfo = new List<SpellTargetInfo>();
        }

        ~Spell()
        {
            Logging.LogSpell($"Finalized another spell, current count: {--SpellManager.SpellAliveCount}");
        }

        internal void Dispose()
        {
            SpellState = SpellState.Disposed;

            SpellInfo = null;
            Caster = null;
            OriginalCaster = null;

            Targets.Dispose();
            Targets = null;
        }

        internal void DoUpdate(int diffTime)
        {
            if (ExecutionState == SpellExecutionState.Casting)
            {
                CastTimeLeft -= diffTime;

                if (CastTimeLeft <= 0)
                {
                    Caster.SpellCast.HandleSpellCast(this, SpellCast.HandleMode.Finished);
                    Launch();
                }
                else if (!SpellInfo.HasAttribute(SpellAttributes.CastableWhileMoving) && Caster.MovementInfo.IsMoving)
                {
                    Caster.SpellCast.HandleSpellCast(this, SpellCast.HandleMode.Finished);
                    Cancel();
                }
            }
            else
            {
                foreach (SpellTargetInfo targetInfo in UniqueTargetInfo)
                    if (!targetInfo.Processed)
                        return;

                Finish();
            }
        }

        internal SpellCastResult Prepare(SpellCastTargets targets)
        {
            ExecutionState = SpellExecutionState.Preparing;
            InitiateExplicitTargets(targets);

            SpellCastResult result = CheckCast(true);

            if (SpellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreTargetCheck) && result == SpellCastResult.BadTargets)
                result = SpellCastResult.Success;

            if (result != SpellCastResult.Success)
                return result;

            if (SpellInfo.CastTime > 0.0f)
                Cast();
            else
                Launch();

            return result;
        }

        internal bool Cancel()
        {
            if (ExecutionState == SpellExecutionState.Preparing || ExecutionState == SpellExecutionState.Casting)
            {
                Finish();

                return true;
            }

            return false;
        }

        private void Cast()
        {
            ExecutionState = SpellExecutionState.Casting;

            CastTime = SpellInfo.CastTime;
            CastTimeLeft = CastTime;

            // cannot cast two spells at the same time, launch instead, should only be possible with CanCastWhileCasting
            if (Caster.SpellCast.IsCasting)
                Launch();
        }

        private void Launch()
        {
            ExecutionState = SpellExecutionState.Processing;

            SelectSpellTargets();

            Caster.SpellHistory.HandleCooldowns(SpellInfo);

            HandleLaunch();
        }

        private void Finish()
        {
            ExecutionState = SpellExecutionState.Completed;

            SpellManager.Remove(this);
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
                AddUnitTarget(unit);
        }

        private void SelectImplicitCasterDestTargets(SpellEffectInfo effect, TargetingType targetingType)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitTargetDestTargets(SpellEffectInfo effect, TargetingType targetingType)
        {
            throw new NotImplementedException();
        }

        private void SelectEffectTypeImplicitTargets(SpellEffectInfo effect)
        {
            // add explicit spellTarget or self to the spellTarget list
            switch (effect.ExplicitTargetType)
            {
                case SpellExplicitTargetType.Explicit:
                    AddUnitTarget(Targets.UnitTarget ?? Caster);
                    break;
                case SpellExplicitTargetType.Caster:
                    AddUnitTarget(Caster);
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

        private void AddUnitTarget(Unit target)
        {
            // lookup spell target that may already exist
            var sameTargetInfo = UniqueTargetInfo.Find(unit => unit.TargetId == target.NetworkId);
            if (sameTargetInfo != null)
                return;

            // create info for new spell target
            SpellTargetInfo spellTargetInfo = new SpellTargetInfo();
            spellTargetInfo.TargetId = target.NetworkId;
            spellTargetInfo.Processed = false;
            spellTargetInfo.Alive = target.IsAlive;
            spellTargetInfo.Damage = 0;
            spellTargetInfo.Crit = false;
            spellTargetInfo.ScaleAura = false;

            // calculate hit result
            if (OriginalCaster != null)
            {
                spellTargetInfo.MissCondition = OriginalCaster.SpellHitResult(target, SpellInfo, CanReflect);
                if (spellTargetInfo.MissCondition != SpellMissType.Immune)
                    spellTargetInfo.MissCondition = SpellMissType.None;
            }
            else
                spellTargetInfo.MissCondition = SpellMissType.Evade;

            // calculate hit delay for spells with speed
            if (SpellInfo.Speed > 0.0f && Caster != target)
            {
                float distance = Mathf.Clamp(Vector3.Distance(Caster.Position, target.Position), StatUtils.DefaultCombatReach, float.MaxValue);
                spellTargetInfo.Delay = distance / SpellInfo.Speed;
            }
            else
                spellTargetInfo.Delay = 0.0f;

            UniqueTargetInfo.Add(spellTargetInfo);
        }

        #endregion

        #region Cast and Target Validation

        private SpellCastResult CheckCast(bool strict)
        {
            // check death state
            if (!Caster.IsAlive && !SpellInfo.IsPassive() && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileDead))
                return SpellCastResult.CasterDead;

            // check cooldowns to prevent cheating
            if (!SpellInfo.IsPassive() && !Caster.SpellHistory.IsReady(SpellInfo))
                return SpellCastResult.NotReady;

            // check global cooldown
            if (strict && !SpellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreGcd) && Caster.SpellHistory.HasGlobalCooldown)
                return SpellCastResult.NotReady;

            // check if already casting
            if (Caster.SpellCast.IsCasting && !SpellInfo.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                return SpellCastResult.NotReady;
            
            SpellCastResult castResult = CheckRange(strict);
            if (castResult != SpellCastResult.Success)
                return castResult;

            return SpellCastResult.Success;
        }

        private SpellCastResult CheckRange(bool strict)
        {
            // don't check for instant cast spells
            if (!strict && Mathf.Approximately(CastTime, 0.0f))
                return SpellCastResult.Success;

            Unit target = Targets.UnitTarget;
            float minRange = 0.0f;
            float maxRange = 0.0f;
            float rangeMod = 0.0f;

            if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlags.Melee))
                rangeMod = 3.0f + 4.0f / 3.0f;
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

        private void HandleLaunch()
        {
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCast, Caster, SpellInfo);

            // process immediate effects (items, ground, etc.) also initialize some variables
            HandleImmediatePhase();

            foreach (var targetInfo in UniqueTargetInfo)
                DoAllEffectsOnTarget(targetInfo);

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

        private void DoAllEffectsOnTarget(SpellTargetInfo spellTarget)
        {
            if (spellTarget == null || spellTarget.Processed)
                return;

            spellTarget.Processed = true;
            Unit unit = Caster.NetworkId == spellTarget.TargetId ? Caster : Caster.Map.FindMapEntity<Unit>(spellTarget.TargetId);
            if (unit?.IsAlive != spellTarget.Alive)
                return;

            Unit caster = OriginalCaster ?? Caster;
            if (caster == null)
                return;

            SpellMissType missType = spellTarget.MissCondition;

            EffectDamage = spellTarget.Damage;
            EffectHealing = -spellTarget.Damage;

            Unit spellHitTarget = null;
            if (missType == SpellMissType.None)
                spellHitTarget = unit;
            else if (missType == SpellMissType.Reflect && spellTarget.ReflectResult == SpellMissType.None)
                spellHitTarget = Caster;

            if (spellHitTarget != null)
            {
                SpellMissType missInfo2 = DoSpellHitOnUnit(spellHitTarget);

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
                SpellCastDamageInfo damageInfoInfo = new SpellCastDamageInfo(caster, unit, SpellInfo.Id, SpellSchoolMask);
                EffectDamage = caster.CalculateSpellDamageTaken(damageInfoInfo, EffectDamage, SpellInfo);
                caster.DamageBySpell(damageInfoInfo);
            }
        }

        private void DoAllEffectOnLaunchTarget(SpellTargetInfo spellTargetInfo)
        {
        }
        
        private void DoEffectOnTarget(Unit unitTarget, SpellEffectInfo effect, SpellEffectHandleMode mode)
        {
            effect.Handle(this, unitTarget, mode);
        }

        private SpellMissType DoSpellHitOnUnit(Unit unit)
        {
            if (unit == null)
                return SpellMissType.Evade;

            foreach (var effect in Effects)
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