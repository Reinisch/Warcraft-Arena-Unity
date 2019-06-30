using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public partial class Spell
    {
        private readonly SpellManager spellManager;
        private readonly SpellSchoolMask spellSchoolMask;
        private readonly SpellCastFlags spellCastFlags;

        private SpellExplicitTargets ExplicitTargets { get; }
        private SpellImplicitTargets ImplicitTargets { get; }

        private int CastTimeLeft { get; set; }
        private int EffectDamage { get; set; }
        private int EffectHealing { get; set; }

        internal bool CanReflect { get; }
        internal int CastTime { get; private set; }
        internal Unit Caster { get; private set; }
        internal Unit OriginalCaster { get; private set; }
        internal SpellInfo SpellInfo { get; private set; }
        internal SpellState SpellState { get; set; }
        internal SpellExecutionState ExecutionState { get; private set; }

        internal Spell(Unit caster, SpellInfo info, SpellExplicitTargets explicitTargets, SpellCastFlags spellFlags)
        {
            Logging.LogSpell($"Created new spell, current count: {++SpellManager.SpellAliveCount}");

            spellManager = caster.WorldManager.SpellManager;
            spellCastFlags = spellFlags;
            spellSchoolMask = info.SchoolMask;

            CastTime = CastTimeLeft = EffectDamage = EffectHealing = 0;
            Caster = OriginalCaster = caster;
            SpellInfo = info;

            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                spellCastFlags = spellCastFlags | SpellCastFlags.IgnoreCastInProgress | SpellCastFlags.CastDirectly;

            CanReflect = SpellInfo.DamageClass == SpellDamageClass.Magic && !SpellInfo.HasAttribute(SpellAttributes.CantBeReflected) &&
                !SpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !SpellInfo.IsPassive() && !SpellInfo.IsPositive();

            ExplicitTargets = explicitTargets ?? new SpellExplicitTargets();
            ImplicitTargets = new SpellImplicitTargets(this);

            spellManager.Add(this);
        }

        ~Spell()
        {
            Logging.LogSpell($"Finalized another spell, current count: {--SpellManager.SpellAliveCount}");
        }

        internal void Dispose()
        {
            SpellState = SpellState.Disposed;

            SpellInfo = null;
            Caster = OriginalCaster = null;

            ImplicitTargets.Dispose();
            ExplicitTargets.Dispose();
        }

        internal void Cancel()
        {
            if (ExecutionState == SpellExecutionState.Preparing || ExecutionState == SpellExecutionState.Casting)
                Finish();
        }

        internal void DoUpdate(int deltaTime)
        {
            switch (ExecutionState)
            {
                case SpellExecutionState.Casting:
                    CastTimeLeft -= deltaTime;
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
                    break;
                case SpellExecutionState.Processing:
                    bool hasUnprocessed = false;
                    foreach (SpellTargetEntry targetInfo in ImplicitTargets.Entries)
                    {
                        if (targetInfo.Processed)
                            continue;

                        targetInfo.Delay -= deltaTime;

                        if (targetInfo.Delay <= 0)
                            ProcessTarget(targetInfo);
                        else
                            hasUnprocessed = true;
                    }
                    
                    if(!hasUnprocessed)
                        Finish();
                    break;
                case SpellExecutionState.Completed:
                    goto default;
                case SpellExecutionState.Preparing:
                    goto default;
                default:
                    Assert.IsTrue(SpellState == SpellState.Removing, $"Spell {SpellInfo.SpellName} updated in invalid state: {ExecutionState} while not being removed!");
                    break;
            }
        }

        internal void HandleUnitDetach(Unit detachedUnit)
        {
            switch (ExecutionState)
            {
                case SpellExecutionState.Casting when SpellInfo.ExplicitTargetType == SpellExplicitTargetType.Target && ExplicitTargets.Target == detachedUnit:
                    Caster.SpellCast.HandleSpellCast(this, SpellCast.HandleMode.Finished);
                    Cancel();
                    break;
                case SpellExecutionState.Processing when Caster == detachedUnit:
                    Finish();
                    break;
                case SpellExecutionState.Processing when ImplicitTargets.Contains(detachedUnit):
                    ImplicitTargets.RemoveTargetIfExists(detachedUnit);
                    break;
            }
        }

        internal SpellCastResult Prepare()
        {
            ExecutionState = SpellExecutionState.Preparing;
            PrepareExplicitTarget();

            SpellCastResult result = ValidateCast();

            if (spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreTargetCheck) && result == SpellCastResult.BadTargets)
                result = SpellCastResult.Success;

            if (result != SpellCastResult.Success)
                return result;

            if (SpellInfo.CastTime > 0.0f)
                Cast();
            else
                Launch();

            return result;
        }

        private void ProcessTarget(SpellTargetEntry targetEntry)
        {
            if (targetEntry.Processed)
                return;

            targetEntry.Processed = true;
            if (targetEntry.Target.IsAlive != targetEntry.Alive)
                return;

            Unit caster = OriginalCaster ?? Caster;
            if (caster == null)
                return;

            SpellMissType missType = targetEntry.MissCondition;

            EffectDamage = targetEntry.Damage;
            EffectHealing = -targetEntry.Damage;

            Unit hitTarget = null;
            if (missType == SpellMissType.None)
                hitTarget = targetEntry.Target;
            else if (missType == SpellMissType.Reflect && targetEntry.ReflectResult == SpellMissType.None)
                hitTarget = Caster;

            if (hitTarget != null)
            {
                for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                {
                    SpellInfo.Effects[effectIndex].Handle(this, effectIndex, hitTarget, SpellEffectHandleMode.HitTarget);
                }

                if (missType != SpellMissType.None)
                    EffectDamage = 0;

                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, Caster, hitTarget, SpellInfo, missType);
            }

            if (EffectHealing > 0)
            {
                bool crit = targetEntry.Crit;
                int addhealth = EffectHealing;
                if (crit)
                    addhealth = caster.SpellCriticalHealingBonus(SpellInfo, addhealth, null);

                int gain = caster.HealBySpell(targetEntry.Target, SpellInfo, addhealth, crit);
                EffectHealing = gain;
            }
            else if (EffectDamage > 0)
            {
                SpellCastDamageInfo damageInfoInfo = new SpellCastDamageInfo(caster, targetEntry.Target, SpellInfo.Id, spellSchoolMask);
                EffectDamage = caster.CalculateSpellDamageTaken(damageInfoInfo, EffectDamage, SpellInfo);
                caster.DamageBySpell(damageInfoInfo);
            }
        }
        
        private void Cast()
        {
            ExecutionState = SpellExecutionState.Casting;

            CastTime = SpellInfo.CastTime;
            CastTimeLeft = CastTime;

            // cannot cast two spells at the same time, launch instead, should only be possible with CanCastWhileCasting
            if (Caster.SpellCast.IsCasting || spellCastFlags.HasTargetFlag(SpellCastFlags.CastDirectly))
                Launch();
        }

        private void Launch()
        {
            ExecutionState = SpellExecutionState.Processing;
            Caster.SpellHistory.HandleCooldowns(SpellInfo);
            SelectImplicitTargets();

            for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                SpellInfo.Effects[effectIndex].Handle(this, effectIndex, Caster, SpellEffectHandleMode.Launch);

            ImplicitTargets.HandleLaunch(out bool isDelayed, out SpellProcessingToken processingToken);

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellLaunch, Caster, SpellInfo, processingToken);

            if (!isDelayed)
            {
                foreach (var targetInfo in ImplicitTargets.Entries)
                    ProcessTarget(targetInfo);

                Finish();
            }
        }

        private void Finish()
        {
            ExecutionState = SpellExecutionState.Completed;

            spellManager.Remove(this);
        }

        private SpellCastResult ValidateCast()
        {
            // check death state
            if (!Caster.IsAlive && !SpellInfo.IsPassive() && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileDead))
                return SpellCastResult.CasterDead;

            // check cooldowns to prevent cheating
            if (!SpellInfo.IsPassive() && !Caster.SpellHistory.IsReady(SpellInfo))
                return SpellCastResult.NotReady;

            // check global cooldown
            if (!spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreGcd) && Caster.SpellHistory.HasGlobalCooldown)
                return SpellCastResult.NotReady;

            // check if already casting
            if (Caster.SpellCast.IsCasting && !SpellInfo.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                return SpellCastResult.NotReady;

            SpellCastResult castResult = ValidateRange();
            if (castResult != SpellCastResult.Success)
                return castResult;

            castResult = SpellInfo.CheckExplicitTarget(Caster, ExplicitTargets.Target);
            if (castResult != SpellCastResult.Success)
                return castResult;

            if (ExplicitTargets.Target != null)
            {
                castResult = SpellInfo.CheckTarget(Caster, ExplicitTargets.Target, this, false);
                if (castResult != SpellCastResult.Success)
                    return castResult;
            }

            return SpellCastResult.Success;
        }

        private SpellCastResult ValidateRange()
        {
            if (SpellInfo.ExplicitTargetType != SpellExplicitTargetType.Target || ExplicitTargets.Target == null || ExplicitTargets.Target == Caster)
                return SpellCastResult.Success;

            Unit target = ExplicitTargets.Target;

            float minRange = 0.0f;
            float maxRange = 0.0f;
            float rangeMod = 0.0f;

            if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlags.Melee))
                rangeMod = StatUtils.NominalMeleeRange;
            else
            {
                float meleeRange = 0.0f;
                if (SpellInfo.RangedFlags.HasTargetFlag(SpellRangeFlags.Ranged))
                    meleeRange = StatUtils.MinMeleeReach;

                minRange = Caster.GetSpellMinRangeForTarget(target, SpellInfo) + meleeRange;
                maxRange = Caster.GetSpellMaxRangeForTarget(target, SpellInfo);
            }

            maxRange += rangeMod;

            if (Vector3.Distance(Caster.Position, target.Position) > maxRange)
                return SpellCastResult.OutOfRange;

            if (minRange > 0.0f && Vector3.Distance(Caster.Position, target.Position) < minRange)
                return SpellCastResult.OutOfRange;

            return SpellCastResult.Success;
        }
        
        private void PrepareExplicitTarget()
        {
            // initializes client-provided targets, corrects and automatically attempts to set required target.
            bool targetsUnits = SpellInfo.ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask);

            if (ExplicitTargets.Target != null && !targetsUnits)
                ExplicitTargets.Target = null;

            // try to select correct unit target if not provided by client
            if (ExplicitTargets.Target == null && targetsUnits)
            {
                // try to use player selection as target, it has to be valid target for the spell
                if (Caster is Player playerCaster && playerCaster.Target is Unit playerTarget)
                    if (SpellInfo.CheckExplicitTarget(Caster, playerTarget) == SpellCastResult.Success)
                        ExplicitTargets.Target = playerTarget;

                // didn't find anything, try to use self as target
                if (ExplicitTargets.Target == null && SpellInfo.ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitAlly))
                    ExplicitTargets.Target = Caster;
            }
        }

        private void SelectImplicitTargets()
        {
            // select explicit potentially redirected target
            SelectRedirectedTargets();

            // also select targets based on spell effects
            int processedAreaEffectsMask = 0;
            for (var effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
            {
                var effect = SpellInfo.Effects[effectIndex];
                SelectImplicitTargetsForEffect(effect, effectIndex, effect.MainTargeting, ref processedAreaEffectsMask);
                SelectImplicitTargetsForEffect(effect, effectIndex, effect.SecondaryTargeting, ref processedAreaEffectsMask);

                // select implicit target from explicit effect target type
                switch (effect.ExplicitTargetType)
                {
                    case SpellExplicitTargetType.Target when ExplicitTargets.Target != null:
                        ImplicitTargets.AddTargetIfNotExists(ExplicitTargets.Target);
                        break;
                    case SpellExplicitTargetType.Caster when Caster != null:
                        ImplicitTargets.AddTargetIfNotExists(Caster);
                        break;
                }
            }

            void SelectRedirectedTargets()
            {
                Unit target = ExplicitTargets.Target;
                if (target == null)
                    return;

                if (SpellInfo.ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitEnemy) && Caster.IsHostileTo(target))
                {
                    Unit redirectTarget;
                    switch (SpellInfo.DamageClass)
                    {
                        case SpellDamageClass.Magic:
                            redirectTarget = Caster.GetMagicHitRedirectTarget(target, SpellInfo);
                            break;
                        case SpellDamageClass.Melee:
                        case SpellDamageClass.Ranged:
                            redirectTarget = Caster.GetMeleeHitRedirectTarget(target, SpellInfo);
                            break;
                        default:
                            redirectTarget = null;
                            break;
                    }
                    if (redirectTarget != null && redirectTarget != target)
                        ExplicitTargets.Target = redirectTarget;
                }
            }
        }

        private void SelectImplicitTargetsForEffect(SpellEffectInfo effect, int effectIndex, TargetingType targetingType, ref int processedEffectMask)
        {
            int effectMask = 1 << effectIndex;
            if (targetingType.IsAreaLookup)
            {
                if ((effectMask & processedEffectMask) > 0)
                    return;

                // avoid recalculating similar effects
                for (int otherEffectIndex = 0; otherEffectIndex < SpellInfo.Effects.Count; otherEffectIndex++)
                {
                    var otherEffect = SpellInfo.Effects[otherEffectIndex];
                    if (effect.MainTargeting == otherEffect.MainTargeting && effect.SecondaryTargeting == otherEffect.SecondaryTargeting)
                        if (Mathf.Approximately(effect.CalcRadius(Caster, this), otherEffect.CalcRadius(Caster, this)))
                            effectMask |= 1 << otherEffectIndex;
                }

                processedEffectMask |= effectMask;
            }

            switch (targetingType.SelectionCategory)
            {
                case SpellTargetSelection.Nearby:
                    SelectImplicitTargetsNearby(effect, effectIndex, targetingType, effectMask);
                    break;
                case SpellTargetSelection.Cone:
                    SelectImplicitTargetsInCone(effect, effectIndex, targetingType, effectMask);
                    break;
                case SpellTargetSelection.Area:
                    SelectImplicitTargetsInArea(effect, effectIndex, targetingType, effectMask);
                    break;
                case SpellTargetSelection.Default:
                    break;
            }
        }

        private void SelectImplicitTargetsNearby(SpellEffectInfo effect, int effectIndex, TargetingType targetingType, int effMask)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitTargetsInCone(SpellEffectInfo effect, int effectIndex, TargetingType targetingType, int effMask)
        {
            throw new NotImplementedException();
        }

        private void SelectImplicitTargetsInArea(SpellEffectInfo effect, int effectIndex, TargetingType targetingType, int effMask)
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
                    referer = ExplicitTargets.Target;
                    break;
                case SpellTargetReferences.Last:
                    {
                        for (int i = ImplicitTargets.Entries.Count - 1; i >= 0; i--)
                        {
                            if (ImplicitTargets.Entries[i].EffectMask.HasBit(effectIndex))
                            {
                                referer = ImplicitTargets.Entries[i].Target;
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
                case SpellTargetReferences.Dest:
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
                ImplicitTargets.AddTargetIfNotExists(unit);
        }
    }
}