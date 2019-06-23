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

        private SpellCastTargets CastTargets { get; set; }
        private SpellUniqueTargets SelectedTargets { get; }

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

        internal Spell(Unit caster, SpellInfo info, SpellCastFlags spellFlags)
        {
            Logging.LogSpell($"Created new spell, current count: {++SpellManager.SpellAliveCount}");

            spellManager = caster.WorldManager.SpellManager;

            Caster = OriginalCaster = caster;
            SpellInfo = info;
            spellCastFlags = spellFlags;
            spellSchoolMask = info.SchoolMask;

            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                spellCastFlags = spellCastFlags | SpellCastFlags.IgnoreCastInProgress | SpellCastFlags.CastDirectly;

            CastTime = 0;
            CastTimeLeft = 0;

            CanReflect = SpellInfo.DamageClass == SpellDamageClass.Magic && !SpellInfo.HasAttribute(SpellAttributes.CantBeReflected) &&
                !SpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !SpellInfo.IsPassive() && !SpellInfo.IsPositive();

            EffectDamage = 0;
            EffectHealing = 0;

            SelectedTargets = new SpellUniqueTargets(this);
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

            SelectedTargets.Dispose();
            CastTargets.Dispose();
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
                foreach (SpellTargetEntry targetInfo in SelectedTargets.Entries)
                    if (!targetInfo.Processed)
                        return;

                Finish();
            }
        }

        internal SpellCastResult Prepare(SpellCastTargets targets)
        {
            ExecutionState = SpellExecutionState.Preparing;
            InitializeExplicitTargets(targets);

            SpellCastResult result = CheckCast(true);

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

        internal void Cancel()
        {
            if (ExecutionState == SpellExecutionState.Preparing || ExecutionState == SpellExecutionState.Casting)
                Finish();
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

            spellManager.Remove(this);
        }

        #region Target Selection

        /// <summary>
        /// Initializes client-provided targets, corrects and automatically attempts to set required target.
        /// </summary>
        private void InitializeExplicitTargets(SpellCastTargets targets)
        {
            CastTargets = targets;
            bool targetsUnits = SpellInfo.ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask);

            if (CastTargets.Target != null && !targetsUnits)
                CastTargets.Target = null;

            // try to select correct unit target if not provided by client
            if (CastTargets.Target == null && targetsUnits)
            {
                // try to use player selection as target, it has to be valid target for the spell
                if (Caster is Player playerCaster && playerCaster.Target is Unit playerTarget)
                    if (SpellInfo.CheckExplicitTarget(Caster, playerTarget) == SpellCastResult.Success)
                        CastTargets.Target = playerTarget;

                // didn't find anything, try to use self as target
                if (CastTargets.Target == null && SpellInfo.ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitAlly))
                    CastTargets.Target = Caster;
            }
        }

        /// <summary>
        /// Select targets based on spell effects.
        /// </summary>
        private void SelectSpellTargets()
        {
            SelectExplicitTargets();

            int processedAreaEffectsMask = 0;
            foreach (var effect in SpellInfo.Effects)
            {
                SelectEffectImplicitTargets(effect, effect.MainTargeting, ref processedAreaEffectsMask);
                SelectEffectImplicitTargets(effect, effect.SecondaryTargeting, ref processedAreaEffectsMask);

                switch (effect.ExplicitTargetType)
                {
                    case SpellExplicitTargetType.Target when CastTargets.Target != null:
                        SelectedTargets.AddTargetIfNotExists(CastTargets.Target);
                        break;
                    case SpellExplicitTargetType.Caster when Caster != null:
                        SelectedTargets.AddTargetIfNotExists(Caster);
                        break;
                }
            }
        }

        private void SelectExplicitTargets()
        {
            Unit target = CastTargets.Target;
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
                    CastTargets.Target = redirectTarget;
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
                foreach (var otherEffect in SpellInfo.Effects)
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
                    referer = CastTargets.Target;
                    break;
                case SpellTargetReferences.Last:
                {
                    for (int i = SelectedTargets.Entries.Count - 1; i >= 0; i--)
                    {
                        if (SelectedTargets.Entries[i].EffectMask.HasBit(effect.Index))
                        {
                            referer = SelectedTargets.Entries[i].Target;
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
                SelectedTargets.AddTargetIfNotExists(unit);
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
            if (strict && !spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreGcd) && Caster.SpellHistory.HasGlobalCooldown)
                return SpellCastResult.NotReady;

            // check if already casting
            if (Caster.SpellCast.IsCasting && !SpellInfo.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                return SpellCastResult.NotReady;
            
            SpellCastResult castResult = CheckRange();
            if (castResult != SpellCastResult.Success)
                return castResult;

            castResult = SpellInfo.CheckExplicitTarget(Caster, CastTargets.Target);
            if (castResult != SpellCastResult.Success)
                return castResult;

            castResult = SpellInfo.CheckTarget(Caster, CastTargets.Target, this, false);
            if (castResult != SpellCastResult.Success)
                return castResult;

            return SpellCastResult.Success;
        }

        private SpellCastResult CheckRange()
        {
            if (SpellInfo.ExplicitTargetType != SpellExplicitTargetType.Target)
                return SpellCastResult.Success;

            Unit target = CastTargets.Target;
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

        private SpellCastResult CheckAuras() { throw new NotImplementedException(); }

        #endregion

        #region Spell Processing

        private void HandleLaunch()
        {
            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCast, Caster, SpellInfo);

            // process immediate effects (items, ground, etc.) also initialize some variables
            HandleImmediatePhase();

            foreach (var targetInfo in SelectedTargets.Entries)
                DoAllEffectsOnTarget(targetInfo);

            Finish();
        }

        private void HandleImmediatePhase()
        {
            foreach (var effect in SpellInfo.Effects)
                DoEffectOnTarget(null, effect, SpellEffectHandleMode.Hit);
        }

        private void DoAllEffectsOnTarget(SpellTargetEntry entry)
        {
            if (entry.Processed)
                return;

            entry.Processed = true;
            if (entry.Target.IsAlive != entry.Alive)
                return;

            Unit caster = OriginalCaster ?? Caster;
            if (caster == null)
                return;

            SpellMissType missType = entry.MissCondition;

            EffectDamage = entry.Damage;
            EffectHealing = -entry.Damage;

            Unit spellHitTarget = null;
            if (missType == SpellMissType.None)
                spellHitTarget = entry.Target;
            else if (missType == SpellMissType.Reflect && entry.ReflectResult == SpellMissType.None)
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
                bool crit = entry.Crit;
                int addhealth = EffectHealing;
                if (crit)
                    addhealth = caster.SpellCriticalHealingBonus(SpellInfo, addhealth, null);

                int gain = caster.HealBySpell(entry.Target, SpellInfo, addhealth, crit);
                EffectHealing = gain;
            }
            else if (EffectDamage > 0)
            {
                SpellCastDamageInfo damageInfoInfo = new SpellCastDamageInfo(caster, entry.Target, SpellInfo.Id, spellSchoolMask);
                EffectDamage = caster.CalculateSpellDamageTaken(damageInfoInfo, EffectDamage, SpellInfo);
                caster.DamageBySpell(damageInfoInfo);
            }
        }

        private void DoEffectOnTarget(Unit unitTarget, SpellEffectInfo effect, SpellEffectHandleMode mode)
        {
            effect.Handle(this, unitTarget, mode);
        }

        private SpellMissType DoSpellHitOnUnit(Unit unit)
        {
            if (unit == null)
                return SpellMissType.Evade;

            foreach (var effect in SpellInfo.Effects)
                DoEffectOnTarget(unit, effect, SpellEffectHandleMode.HitTarget);

            return SpellMissType.None;
        }

        private void PrepareTargetProcessing() { throw new NotImplementedException(); }

        private void FinishTargetProcessing() { throw new NotImplementedException(); }

        private void TriggerGlobalCooldown() { throw new NotImplementedException(); }

        private void CancelGlobalCooldown() { throw new NotImplementedException(); }

        #endregion
    }
}