using System;
using System.Collections.Generic;
using Common;
using Core.AuraEffects;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public partial class Spell
    {
        private static int SpellAliveCount;

        private List<(SpellPowerType, int)> powerCosts = ListPoolContainer<(SpellPowerType, int)>.Take();

        private SpellValue spellValue;
        private readonly SpellManager spellManager;
        private readonly MovementFlags casterMovementFlags;
        private readonly HashSet<Aura> appliedModifierAuras = new HashSet<Aura>();
        private readonly HashSet<Aura> chargeDroppedModifierAuras = new HashSet<Aura>();
        private readonly HashSet<SpellModifier> appliedModifiers = new HashSet<SpellModifier>();
        private readonly List<SpellModifier> unappliedModifiers = new List<SpellModifier>();

        private int CastTimeLeft { get; set; }
        private int EffectDamage { get; set; }
        private int EffectHealing { get; set; }

        internal bool IsTriggered { get; }
        internal bool CanReflect { get; }
        internal int ConsumedComboPoints { get; set; }
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
            spellValue.CastFlags = options.SpellFlags;
            casterMovementFlags = options.MovementFlags ?? caster.Motion.MovementFlags;
            SchoolMask = info.SchoolMask;

            CastTime = CastTimeLeft = EffectDamage = EffectHealing = 0;
            Caster = OriginalCaster = caster;
            SpellInfo = info;

            IsTriggered = spellValue.CastFlags.HasTargetFlag(SpellCastFlags.TriggeredByAura);

            if (IsTriggered)
                spellValue.CastFlags |= SpellCastFlags.IgnoreTargetCheck | SpellCastFlags.IgnoreRangeCheck |
                    SpellCastFlags.IgnoreShapeShift | SpellCastFlags.IgnoreAuraInterruptFlags | SpellCastFlags.IgnoreCasterState;

            if (info.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                spellValue.CastFlags |= SpellCastFlags.IgnoreCastInProgress | SpellCastFlags.CastDirectly;

            if (info.HasAttribute(SpellExtraAttributes.IgnoreGcd))
                spellValue.CastFlags |= SpellCastFlags.IgnoreGcd;

            if (info.HasAttribute(SpellExtraAttributes.IgnoreCasterAuras))
                spellValue.CastFlags |= SpellCastFlags.IgnoreCasterAuras;

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
            appliedModifiers.Clear();
            unappliedModifiers.Clear();

            ListPoolContainer<(SpellPowerType, int)>.Return(powerCosts);
            powerCosts = null;

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
                    if (mayBeInterruptedByMove && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileMoving) && Caster.Motion.IsMoving)
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
                    Assert.IsTrue(SpellState == SpellState.Removing, $"Spell {SpellInfo.name} updated in invalid state: {ExecutionState} while not being removed!");
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

        internal bool IsIgnoringAuraState(AuraStateType auraStateType) => (spellValue.IgnoredAuraStates & auraStateType.AsFlag()) != 0;

        internal bool HasAppliedModifier(Aura aura) => appliedModifierAuras.Contains(aura);

        internal void ApplySpellValueModifier(SpellValueModifier valueModifier) => valueModifier.Modify(ref spellValue);

        internal void AddAppliedModifier(SpellModifier spellModifier)
        {
            appliedModifierAuras.Add(spellModifier.Aura);
            appliedModifiers.Add(spellModifier);
            unappliedModifiers.Remove(spellModifier);

            for (int i = 0; i < spellModifier.Aura.Effects.Count; i++)
                if (spellModifier.Aura.Effects[i] is AuraEffectSpellModifier spellModifierEffect)
                    if (!appliedModifiers.Contains(spellModifierEffect.SpellModifier))
                        unappliedModifiers.Add(spellModifierEffect.SpellModifier);
        }

        internal void HandleUnappliedModifers(Unit unit, SpellModifierType modifierType, SpellModifierApplicationType applicationType, ref float value)
        {
            switch (applicationType)
            {
                case SpellModifierApplicationType.Flat:
                    for (int i = unappliedModifiers.Count - 1; i >= 0; i--)
                    {
                        if (unappliedModifiers[i].Kind != (modifierType, applicationType))
                            continue;

                        if (!unit.Spells.IsAffectedBySpellModifier(this, unappliedModifiers[i]))
                            continue;

                        value += unappliedModifiers[i].Value;
                        appliedModifiers.Add(unappliedModifiers[i]);
                        unappliedModifiers.RemoveAt(i);
                    }
                    break;
                case SpellModifierApplicationType.Percent:
                    for (int i = unappliedModifiers.Count - 1; i >= 0; i--)
                    {
                        if (unappliedModifiers[i].Kind != (modifierType, applicationType))
                            continue;

                        if (!unit.Spells.IsAffectedBySpellModifier(this, unappliedModifiers[i]))
                            continue;

                        value *= 1.0f + 1.0f.ApplyPercentage(unappliedModifiers[i].Value);
                        appliedModifiers.Add(unappliedModifiers[i]);
                        unappliedModifiers.RemoveAt(i);
                    }
                    break;
                case SpellModifierApplicationType.SpellValue:
                    for (int i = unappliedModifiers.Count - 1; i >= 0; i--)
                    {
                        if (unappliedModifiers[i].Kind != (modifierType, applicationType))
                            continue;

                        if (!unit.Spells.IsAffectedBySpellModifier(this, unappliedModifiers[i]))
                            continue;

                        if (unappliedModifiers[i].AuraModifier.SpellValueModifier != null)
                            ApplySpellValueModifier(unappliedModifiers[i].AuraModifier.SpellValueModifier);

                        appliedModifiers.Add(unappliedModifiers[i]);
                        unappliedModifiers.RemoveAt(i);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(applicationType), applicationType, null);
            }
        }

        internal SpellCastResult Prepare()
        {
            ExecutionState = SpellExecutionState.Preparing;
            PrepareExplicitTarget();
            SpellInfo.CalculatePowerCosts(Caster, powerCosts, this);

            SpellCastResult result = ValidateCast();
            if (result != SpellCastResult.Success)
                return result;

            return Cast();
        }

        private SpellCastResult ValidateCast()
        {
            if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.TriggeredByAura))
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
            if (!spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreGcd) && Caster.SpellHistory.HasGlobalCooldown)
                return SpellCastResult.NotReady;

            // check if already casting
            if (Caster.SpellCast.IsCasting && !SpellInfo.HasAttribute(SpellExtraAttributes.CanCastWhileCasting))
                return SpellCastResult.NotReady;

            if (SpellInfo.HasAttribute(SpellAttributes.RequiresComboPoints) && !spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreComboPoints) && Caster.ComboPoints < 1)
                return SpellCastResult.NoComboPoints;

            SpellCastResult castResult;
            if (!spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreTargetCheck))
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

            castResult = ValidateCasterState();
            if (castResult != SpellCastResult.Success)
                return castResult;

            castResult = ValidatePowers();
            if (castResult != SpellCastResult.Success)
                return castResult;

            castResult = ValidateShapeShift();
            if (castResult != SpellCastResult.Success)
                return castResult;

            return SpellCastResult.Success;
        }

        private SpellCastResult ValidatePowers()
        {
            foreach ((SpellPowerType, int) cost in powerCosts)
            {
                if (cost.Item1 == SpellPowerType.Health && Caster.Health <= cost.Item2)
                    return SpellCastResult.NoPower;

                if (Caster.Attributes.Power(cost.Item1) < cost.Item2)
                {
                    switch (cost.Item1)
                    {
                        case SpellPowerType.Mana:
                            return SpellCastResult.NoMana;
                        case SpellPowerType.Rage:
                            return SpellCastResult.NoRage;
                        case SpellPowerType.Health:
                            return SpellCastResult.NoHealth;
                        case SpellPowerType.Energy:
                            return SpellCastResult.NoEnergy;
                        case SpellPowerType.ComboPoints:
                            return SpellCastResult.NoComboPoints;
                        case SpellPowerType.Focus:
                        case SpellPowerType.Runes:
                        case SpellPowerType.RunicPower:
                        case SpellPowerType.SoulShards:
                        case SpellPowerType.LunarPower:
                        case SpellPowerType.HolyPower:
                        case SpellPowerType.AlternatePower:
                        case SpellPowerType.Maelstrom:
                        case SpellPowerType.Chi:
                        case SpellPowerType.Insanity:
                        case SpellPowerType.BurningEmbers:
                        case SpellPowerType.DemonicFury:
                        case SpellPowerType.ArcaneCharges:
                        case SpellPowerType.Fury:
                        case SpellPowerType.Pain:
                            goto default;
                        default:
                            return SpellCastResult.NoPower;
                    }
                }
            }

            return SpellCastResult.Success;
        }

        private SpellCastResult ValidateCasterState()
        {
            if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreCasterState))
                return SpellCastResult.Success;

            if (SpellInfo.HasAttribute(SpellAttributes.CantBeUsedInCombat) && Caster.Combat.InCombat)
                return SpellCastResult.AffectingCombat;

            return SpellCastResult.Success;
        }

        private SpellCastResult ValidateAuras()
        {
            if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreCasterAuras))
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
            if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreRangeCheck))
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

                if (!Caster.IsWithinDistance(target, maxRange, true))
                    return SpellCastResult.OutOfRange;

                if (minRange > 0.0f && Caster.IsWithinDistance(target, minRange, true))
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

                if (Caster.ExactDistanceTo(targetPosition) > maxRange)
                    return SpellCastResult.OutOfRange;

                if (minRange > 0.0f && Caster.ExactDistanceTo(targetPosition) < minRange)
                    return SpellCastResult.OutOfRange;

                return SpellCastResult.Success;
            }
        }

        private SpellCastResult ValidateShapeShift()
        {
            if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreShapeShift))
                return SpellCastResult.Success;

            IReadOnlyList<AuraEffect> shapeShiftIgnoreEffects = Caster.Auras.GetAuraEffects(AuraEffectType.IgnoreShapeShift);
            if (shapeShiftIgnoreEffects != null) for (int i = 0; i < shapeShiftIgnoreEffects.Count; i++)
                if (shapeShiftIgnoreEffects[i].IsAffectingSpell(SpellInfo))
                    return SpellCastResult.Success;

            return SpellInfo.CheckShapeShift(Caster);
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
            bool instantCast = CastTime <= 0.0f || Caster.SpellCast.IsCasting || spellValue.CastFlags.HasTargetFlag(SpellCastFlags.CastDirectly);
            if (casterMovementFlags.IsMoving() && !instantCast && !SpellInfo.HasAttribute(SpellAttributes.CastableWhileMoving))
                return SpellCastResult.Moving;

            if (!spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreShapeShift) && SpellInfo.CanCancelForm(this))
                Caster.Auras.RemoveAurasWithEffect(AuraEffectType.ShapeShift);

            Caster.SpellHistory.StartGlobalCooldown(SpellInfo);

            if (!spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreAuraInterruptFlags) && !SpellInfo.HasAttribute(SpellCustomAttributes.DontBreakStealth))
                Caster.Auras.RemoveAurasWithInterrupt(AuraInterruptFlags.Cast);

            if (instantCast)
                Launch();

            return SpellCastResult.Success;
        }
        
        private SpellMissType ProcessSpellHit(SpellTargetEntry targetEntry)
        {
            if (targetEntry.Target.IsImmuneToSpell(SpellInfo, Caster))
                return SpellMissType.Immune;

            for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                if (targetEntry.EffectMask.HasBit(effectIndex))
                    SpellInfo.Effects[effectIndex].Handle(this, effectIndex, targetEntry.Target, SpellEffectHandleMode.HitStart);

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

            EffectDamage = 0;
            EffectHealing = 0;

            Unit hitTarget = targetEntry.Target;
            if (missType == SpellMissType.Reflect && targetEntry.ReflectResult == SpellMissType.None)
                hitTarget = Caster;

            missType = ProcessSpellHit(targetEntry);

            if (missType != SpellMissType.None)
                EffectDamage = 0;

            if (!spellValue.CastFlags.HasTargetFlag(SpellCastFlags.DontReportCastSuccess))
                EventHandler.ExecuteEvent(GameEvents.ServerSpellHit, Caster, hitTarget, SpellInfo, missType);

            Caster.Spells.ApplySpellTriggers(SpellTriggerFlags.DoneSpellHit, hitTarget, this);

            if (EffectHealing > 0)
                caster.Spells.HealBySpell(new SpellHealInfo(caster, targetEntry.Target, SpellInfo, (uint)EffectHealing, targetEntry.Crit));
            else if (EffectDamage > 0)
                caster.Spells.DamageBySpell(new SpellDamageInfo(caster, targetEntry.Target, SpellInfo, (uint)EffectDamage, targetEntry.Crit, SpellDamageType.Direct), this);

            if (missType == SpellMissType.None)
            {
                for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                    if (targetEntry.EffectMask.HasBit(effectIndex))
                        SpellInfo.Effects[effectIndex].Handle(this, effectIndex, hitTarget, SpellEffectHandleMode.HitFinal);
            }
            else if (missType == SpellMissType.Immune && SpellInfo.SomeEffectsIgnoreSpellImmunity)
            {
                for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                    if (targetEntry.EffectMask.HasBit(effectIndex) && SpellInfo.Effects[effectIndex].IgnoresSpellImmunity)
                        SpellInfo.Effects[effectIndex].Handle(this, effectIndex, hitTarget, SpellEffectHandleMode.HitFinal);
            }

            if (missType != SpellMissType.Evade && !caster.IsFriendlyTo(hitTarget) && !SpellInfo.IsPositive)
                caster.Combat.StartCombatWith(hitTarget);
        }

        private void Launch()
        {
            ExecutionState = SpellExecutionState.Processing;
            Caster.SpellHistory.HandleCooldown(SpellInfo);
            SelectImplicitTargets();

            for (int effectIndex = 0; effectIndex < SpellInfo.Effects.Count; effectIndex++)
                SpellInfo.Effects[effectIndex].Handle(this, effectIndex, Caster, SpellEffectHandleMode.Launch);

            ImplicitTargets.HandleLaunch(out bool isDelayed, out SpellProcessingToken processingToken);

            if (!spellValue.CastFlags.HasTargetFlag(SpellCastFlags.DontReportCastSuccess))
                EventHandler.ExecuteEvent(GameEvents.ServerSpellLaunch, Caster, SpellInfo, processingToken);

            DropModifierCharges();
            ConsumePowers();

            Caster.Spells.ApplySpellTriggers(SpellTriggerFlags.DoneSpellCast, Caster, this);

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

        private void ConsumePowers()
        {
            if (SpellInfo.HasAttribute(SpellAttributes.RequiresComboPoints))
            {
                if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnoreComboPoints))
                {
                    ConsumedComboPoints = Caster.MaxComboPoints;
                }
                else
                {
                    ConsumedComboPoints = Caster.ComboPoints;
                    Caster.Attributes.SetComboPoints(0);
                }
            }

            if (spellValue.CastFlags.HasTargetFlag(SpellCastFlags.IgnorePowerAndReagentCost))
                return;

            foreach ((SpellPowerType, int) powerCost in powerCosts)
            {
                if (powerCost.Item1 == SpellPowerType.Health)
                {
                    if (Caster.IsAlive)
                        Caster.ModifyHealth(-Mathf.Min(Caster.Health - 1, powerCost.Item2));

                    continue;
                }

                Caster.Attributes.ModifyPower(powerCost.Item1, -powerCost.Item2);
            }
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
                if (Caster is Player playerCaster && playerCaster.Target != null)
                    if (SpellInfo.CheckExplicitTarget(Caster, playerCaster.Target) == SpellCastResult.Success)
                        ExplicitTargets.Target = playerCaster.Target;

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
                effect.Targeting.SelectTargets(this, effectMask);
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