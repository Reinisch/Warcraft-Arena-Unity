using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public partial class Spell
    {
        private static int SpellAliveCount;

        private readonly SpellManager spellManager;
        private readonly SpellCastFlags spellCastFlags;
        private readonly MovementFlags casterMovementFlags;
        private readonly HashSet<Aura> appliedModifierAuras = new HashSet<Aura>();
        private readonly HashSet<Aura> chargeDroppedModifierAuras = new HashSet<Aura>();

        private int CastTimeLeft { get; set; }
        private int EffectDamage { get; set; }
        private int EffectHealing { get; set; }

        internal bool CanReflect { get; }
        internal SpellSchoolMask SchoolMask { get; }
        internal int CastTime { get; private set; }
        internal Unit Caster { get; private set; }
        internal Unit OriginalCaster { get; private set; }
        internal SpellExplicitTargets ExplicitTargets { get; }
        internal SpellImplicitTargets ImplicitTargets { get; }
        internal SpellInfo SpellInfo { get; private set; }
        internal SpellState SpellState { get; set; }
        internal SpellExecutionState ExecutionState { get; private set; }

        internal Spell(Unit caster, SpellInfo info, SpellCastingOptions options)
        {
            Logging.LogSpell($"Created new spell, current count: {++SpellAliveCount}");

            spellManager = caster.World.SpellManager;
            spellCastFlags = options.SpellFlags;
            casterMovementFlags = options.MovementFlags ?? caster.MovementInfo.Flags;
            SchoolMask = info.SchoolMask;

            CastTime = CastTimeLeft = EffectDamage = EffectHealing = 0;
            Caster = OriginalCaster = caster;
            SpellInfo = info;

            if (spellCastFlags.HasTargetFlag(SpellCastFlags.TriggeredByAura))
                spellCastFlags |= SpellCastFlags.IgnoreTargetCheck | SpellCastFlags.IgnoreRangeCheck;

            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                spellCastFlags |= SpellCastFlags.IgnoreCastInProgress | SpellCastFlags.CastDirectly;

            if (info.HasAttribute(SpellExtraAttributes.IgnoreGcd))
                spellCastFlags |= SpellCastFlags.IgnoreGcd;

            if (info.HasAttribute(SpellExtraAttributes.IgnoreCasterAuras))
                spellCastFlags |= SpellCastFlags.IgnoreCasterAuras;

            CanReflect = SpellInfo.DamageClass == SpellDamageClass.Magic && !SpellInfo.HasAttribute(SpellAttributes.CantBeReflected) &&
                !SpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !SpellInfo.IsPassive && !SpellInfo.IsPositive;

            ExplicitTargets = options.Targets ?? new SpellExplicitTargets();
            ImplicitTargets = new SpellImplicitTargets(this);

            spellManager.Add(this);
        }

        ~Spell()
        {
            Logging.LogSpell($"Finalized another spell, current count: {--SpellAliveCount}");
        }

        internal void Dispose()
        {
            SpellState = SpellState.Disposed;

            SpellInfo = null;
            Caster = OriginalCaster = null;
            appliedModifierAuras.Clear();
            chargeDroppedModifierAuras.Clear();

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
                        break;
                    }

                    bool mayBeInterruptedByMove = CastTime - CastTimeLeft > MovementUtils.SpellMovementInterruptThreshold;
                    if (mayBeInterruptedByMove && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileMoving) && Caster.MovementInfo.IsMoving)
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

        internal bool HasAppliedModifier(Aura aura) => appliedModifierAuras.Contains(aura);

        internal void AddAppliedModifierAura(Aura aura) => appliedModifierAuras.Add(aura);

        internal SpellCastResult Prepare()
        {
            ExecutionState = SpellExecutionState.Preparing;
            PrepareExplicitTarget();

            SpellCastResult result = ValidateCast();
            if (result != SpellCastResult.Success)
                return result;

            return Cast();
        }

        private SpellCastResult ValidateCast()
        {
            if (spellCastFlags.HasTargetFlag(SpellCastFlags.TriggeredByAura))
                return SpellCastResult.Success;

            if (Caster is Player player && !player.PlayerSpells.HasKnownSpell(SpellInfo))
                return SpellCastResult.NotKnown;

            // check death state
            if (!Caster.IsAlive && !SpellInfo.IsPassive && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileDead))
                return SpellCastResult.CasterDead;

            // check cooldowns to prevent cheating
            if (!SpellInfo.IsPassive && !Caster.SpellHistory.IsReady(SpellInfo))
                return SpellCastResult.NotReady;

            // check global cooldown
            if (!spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreGcd) && Caster.SpellHistory.HasGlobalCooldown)
                return SpellCastResult.NotReady;

            // check if already casting
            if (Caster.SpellCast.IsCasting && !SpellInfo.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                return SpellCastResult.NotReady;

            SpellCastResult castResult;
            if (!spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreTargetCheck))
            {
                castResult = SpellInfo.CheckExplicitTarget(Caster, ExplicitTargets.Target);
                if (castResult != SpellCastResult.Success)
                    return castResult;

                if (ExplicitTargets.Target != null)
                {
                    castResult = SpellInfo.CheckTarget(Caster, ExplicitTargets.Target, this, false);
                    if (castResult != SpellCastResult.Success)
                        return castResult;
                }
            }

            castResult = ValidateRange();
            if (castResult != SpellCastResult.Success)
                return castResult;

            castResult = ValidateAuras();
            if (castResult != SpellCastResult.Success)
                return castResult;

            return SpellCastResult.Success;
        }

        private SpellCastResult ValidateAuras()
        {
            if (spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreCasterAuras))
                return SpellCastResult.Success;

            bool usableWhileStunned = SpellInfo.HasAttribute(SpellExtraAttributes.UsableWhileStunned);
            bool usableWhileConfused = SpellInfo.HasAttribute(SpellExtraAttributes.UsableWhileConfused);
            bool usableWhileFeared = SpellInfo.HasAttribute(SpellExtraAttributes.UsableWhileFeared);

            if (Caster.HasFlag(UnitFlags.Stunned))
            {
                if (usableWhileStunned)
                {
                    SpellCastResult result = ValidateMechanics(AuraEffectType.StunState);
                    if (result != SpellCastResult.Success)
                        return result;
                }
                else if (!WillCancelStun())
                    return SpellCastResult.Stunned;
            }

            if (SpellInfo.PreventionType != 0)
            {
                if (SpellInfo.PreventionType.HasTargetFlag(SpellPreventionType.Pacify) && Caster.HasFlag(UnitFlags.Pacified) && !WillCancelPacify())
                    return SpellCastResult.Pacified;

                if (SpellInfo.PreventionType.HasTargetFlag(SpellPreventionType.Silence) && Caster.HasFlag(UnitFlags.Silenced) && !WillCancelSilence())
                    return SpellCastResult.Silenced;
            }

            if (Caster.HasFlag(UnitFlags.Fleeing))
            {
                if (usableWhileFeared)
                {
                    SpellCastResult result = ValidateMechanics(AuraEffectType.ModFear);
                    if (result != SpellCastResult.Success)
                        return result;
                }
                else if (!WillCancelFear())
                    return SpellCastResult.Fleeing;
            }

            if (Caster.HasFlag(UnitFlags.Confused))
            {
                if (usableWhileConfused)
                {
                    SpellCastResult result = ValidateMechanics(AuraEffectType.ConfusionState);
                    if (result != SpellCastResult.Success)
                        return result;
                }
                else if (!WillCancelConfuse())
                    return SpellCastResult.Confused;
            }

            return SpellCastResult.Success;

            bool WillCancelStun()
            {
                return SpellInfo.CanCancelAuraType(AuraEffectType.StunState, Caster) && SpellInfo.CanCancelAuraType(AuraEffectType.Strangulate, Caster);
            }

            bool WillCancelSilence()
            {
                return SpellInfo.CanCancelAuraType(AuraEffectType.Silence, Caster) && SpellInfo.CanCancelAuraType(AuraEffectType.SilencePacify, Caster);
            }

            bool WillCancelPacify()
            {
                return SpellInfo.CanCancelAuraType(AuraEffectType.Pacify, Caster) && SpellInfo.CanCancelAuraType(AuraEffectType.SilencePacify, Caster);
            }

            bool WillCancelFear()
            {
                return SpellInfo.CanCancelAuraType(AuraEffectType.ModFear, Caster);
            }

            bool WillCancelConfuse()
            {
                return SpellInfo.CanCancelAuraType(AuraEffectType.ConfusionState, Caster);
            }
        }

        private SpellCastResult ValidateRange()
        {
            if (spellCastFlags.HasTargetFlag(SpellCastFlags.IgnoreRangeCheck))
                return SpellCastResult.Success;

            switch (SpellInfo.ExplicitTargetType)
            {
                case SpellExplicitTargetType.None:
                    return SpellCastResult.Success;
                case SpellExplicitTargetType.Caster:
                    return SpellCastResult.Success;
                case SpellExplicitTargetType.Target:
                    return ValidateTargetRange();
                case SpellExplicitTargetType.Destination:
                    return ValidateDestinationRange();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SpellCastResult ValidateTargetRange()
            {
                if (ExplicitTargets.Target == null || ExplicitTargets.Target == Caster)
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

                    minRange = Caster.Spells.GetSpellMinRangeForTarget(target, SpellInfo) + meleeRange;
                    maxRange = Caster.Spells.GetSpellMaxRangeForTarget(target, SpellInfo);
                }

                maxRange += rangeMod;

                if (Vector3.Distance(Caster.Position, target.Position) > maxRange)
                    return SpellCastResult.OutOfRange;

                if (minRange > 0.0f && Vector3.Distance(Caster.Position, target.Position) < minRange)
                    return SpellCastResult.OutOfRange;

                return SpellCastResult.Success;
            }

            SpellCastResult ValidateDestinationRange()
            {
                if (!ExplicitTargets.Destination.HasValue)
                    return SpellCastResult.BadTargets;

                Vector3 targetPosition = ExplicitTargets.Destination.Value;

                float minRange = SpellInfo.GetMinRange(false);
                float maxRange = SpellInfo.GetMaxRange(false);

                if (Vector3.Distance(Caster.Position, targetPosition) > maxRange)
                    return SpellCastResult.OutOfRange;

                if (minRange > 0.0f && Vector3.Distance(Caster.Position, targetPosition) < minRange)
                    return SpellCastResult.OutOfRange;

                return SpellCastResult.Success;
            }
        }

        private SpellCastResult ValidateMechanics(AuraEffectType auraEffectType)
        {
            IReadOnlyList<AuraEffect> activeEffects = Caster.Auras.GetAuraEffects(auraEffectType);
            if (activeEffects == null)
                return SpellCastResult.Success;

            for (int i = 0; i < activeEffects.Count; i++)
            {
                SpellMechanicsFlags combinedEffectMechanics = activeEffects[i].Aura.SpellInfo.CombinedEffectMechanics;
                if (combinedEffectMechanics == 0)
                    continue;

                if (!SpellInfo.CastIgnoringMechanics.HasTargetFlag(combinedEffectMechanics))
                {
                    switch (auraEffectType)
                    {
                        case AuraEffectType.StunState:
                            return SpellCastResult.Stunned;
                        case AuraEffectType.ModFear:
                            return SpellCastResult.Fleeing;
                        case AuraEffectType.ConfusionState:
                            return SpellCastResult.Confused;
                        default:
                            return SpellCastResult.NotKnown;
                    }
                }
            }

            return SpellCastResult.Success;
        }

        private SpellCastResult Cast()
        {
            ExecutionState = SpellExecutionState.Casting;

            CastTime = Caster.Spells.ModifySpellCastTime(this, SpellInfo.CastTime);
            CastTimeLeft = CastTime;

            // cast if needed, if already casting launch instead, should only be possible with CanCastWhileCasting
            bool instantCast = CastTime <= 0.0f || Caster.SpellCast.IsCasting || spellCastFlags.HasTargetFlag(SpellCastFlags.CastDirectly);
            if (casterMovementFlags.IsMoving() && !instantCast && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileMoving))
                return SpellCastResult.Moving;

            Caster.SpellHistory.StartGlobalCooldown(SpellInfo);

            if (instantCast)
                Launch();

            return SpellCastResult.Success;
        }
        
        private SpellMissType ProcessSpellHit(Unit target)
        {
            if (target.IsImmuneToSpell(SpellInfo, Caster))
                return SpellMissType.Immune;

            for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                SpellInfo.Effects[effectIndex].Handle(this, effectIndex, target, SpellEffectHandleMode.HitStart);

            return SpellMissType.None;
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

            Unit hitTarget = targetEntry.Target;
            if (missType == SpellMissType.Reflect && targetEntry.ReflectResult == SpellMissType.None)
                hitTarget = Caster;

            missType = ProcessSpellHit(hitTarget);

            if (missType != SpellMissType.None)
                EffectDamage = 0;

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, Caster, hitTarget, SpellInfo, missType);

            if (EffectHealing > 0)
            {
                caster.Spells.HealBySpell(new SpellHealInfo(caster, targetEntry.Target, SpellInfo, (uint)EffectHealing, targetEntry.Crit));
            }
            else if (EffectDamage > 0)
            {
                caster.Spells.DamageBySpell(new SpellDamageInfo(caster, targetEntry.Target, SpellInfo, (uint)EffectDamage, targetEntry.Crit, SpellDamageType.Direct));
            }

            if (missType == SpellMissType.None)
                for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                    SpellInfo.Effects[effectIndex].Handle(this, effectIndex, hitTarget, SpellEffectHandleMode.HitFinal);
        }

        private void Launch()
        {
            ExecutionState = SpellExecutionState.Processing;
            Caster.SpellHistory.StartCooldown(SpellInfo);
            SelectImplicitTargets();

            for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                SpellInfo.Effects[effectIndex].Handle(this, effectIndex, Caster, SpellEffectHandleMode.Launch);

            ImplicitTargets.HandleLaunch(out bool isDelayed, out SpellProcessingToken processingToken);

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerSpellLaunch, Caster, SpellInfo, processingToken);

            DropModifierCharges();

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

            DropModifierCharges();

            spellManager.Remove(this);
        }

        private void DropModifierCharges()
        {
            foreach (Aura aura in appliedModifierAuras)
            {
                if (!chargeDroppedModifierAuras.Contains(aura))
                {
                    chargeDroppedModifierAuras.Add(aura);
                    aura.DropCharge();
                }
            }
        }

        private void PrepareExplicitTarget()
        {
            ExplicitTargets.Source = Caster.Position;

            // initializes client-provided targets, corrects and automatically attempts to set required target.
            bool targetsUnits = SpellInfo.ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask);

            if (ExplicitTargets.Target != null && !targetsUnits)
                ExplicitTargets.Target = null;

            if (SpellInfo.ExplicitTargetType == SpellExplicitTargetType.Caster)
                ExplicitTargets.Target = Caster;

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

            if (ExplicitTargets.Target != null && SpellInfo.HasAttribute(SpellCustomAttributes.LaunchSourceIsExplicit))
                ExplicitTargets.Source = ExplicitTargets.Target.Position;
        }

        private void SelectImplicitTargets()
        {
            // select explicit potentially redirected target
            SelectRedirectedTargets();

            // also select targets based on spell effects
            int processedAreaEffectsMask = 0;
            for (var effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
            {
                if (processedAreaEffectsMask.HasBit(effectIndex))
                    continue;

                SpellEffectInfo effect = SpellInfo.Effects[effectIndex];
                // avoid recalculating similar effects
                int effectMask = 1 << effectIndex;
                for (int otherEffectIndex = 0; otherEffectIndex < SpellInfo.Effects.Count; otherEffectIndex++)
                    if (effect.Targeting == SpellInfo.Effects[otherEffectIndex].Targeting)
                        effectMask |= 1 << otherEffectIndex;

                processedAreaEffectsMask |= effectMask;
                effect.Targeting.SelectTargets(this);
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
                            redirectTarget = Caster.Spells.GetMagicHitRedirectTarget(target, SpellInfo);
                            break;
                        case SpellDamageClass.Melee:
                        case SpellDamageClass.Ranged:
                            redirectTarget = Caster.Spells.GetMeleeHitRedirectTarget(target, SpellInfo);
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
    }
}