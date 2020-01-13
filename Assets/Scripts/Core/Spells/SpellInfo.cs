using System.Collections.Generic;
using Common;
using Core.AuraEffects;
using Core.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Info", menuName = "Game Data/Spells/Spell Info", order = 1)]
    public sealed class SpellInfo : ScriptableUniqueInfo<SpellInfo>
    {
        [SerializeField, UsedImplicitly] private SpellInfoContainer container;
        [SerializeField, UsedImplicitly] private SpellExplicitTargetType explicitTargetType;
        [SerializeField, UsedImplicitly] private SpellDamageClass damageClass;
        [SerializeField, UsedImplicitly] private SpellDispelType spellDispel;
        [SerializeField, UsedImplicitly] private SpellMechanics mechanic;

        [SerializeField, EnumFlag, UsedImplicitly] private SpellCastTargetFlags explicitCastTargets;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellSchoolMask schoolMask;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellPreventionType preventionType;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellAttributes attributes;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellExtraAttributes attributesExtra;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellCustomAttributes attributesCustom;

        [SerializeField, EnumFlag, UsedImplicitly] private EnityTypeMask targetEntityTypeMask;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellRangeFlags rangedFlags;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellInterruptFlags interruptFlags;
        [SerializeField, UsedImplicitly, EnumFlag] private SpellMechanicsFlags castIgnoringMechanics;

        [SerializeField, UsedImplicitly] private int cooldownTime;
        [SerializeField, UsedImplicitly] private int categoryCooldownTime;
        [SerializeField, UsedImplicitly] private int globalCooldownTime;
        [SerializeField, UsedImplicitly] private int castTime;
        [SerializeField, UsedImplicitly] private int charges;
        [SerializeField, UsedImplicitly] private int minCastTime;

        [SerializeField, UsedImplicitly] private float minRangeHostile;
        [SerializeField, UsedImplicitly] private float minRangeFriend;
        [SerializeField, UsedImplicitly] private float maxRangeHostile;
        [SerializeField, UsedImplicitly] private float maxRangeFriend;
        [SerializeField, UsedImplicitly] private float speed;

        [SerializeField, UsedImplicitly] private List<SpellEffectInfo> spellEffectInfos;
        [SerializeField, UsedImplicitly] private List<SpellPowerCostInfo> spellPowerCostInfos;
        [SerializeField, UsedImplicitly] private List<SpellCastCondition> targetingConditions;
        [SerializeField, UsedImplicitly] private List<ShapeShiftForm> shapeShiftAlwaysCastable;
        [SerializeField, UsedImplicitly] private List<ShapeShiftForm> shapeShiftNeverCastable;

        [UsedImplicitly] private HashSet<ShapeShiftForm> shapeShiftAlwaysCastableSet = new HashSet<ShapeShiftForm>();
        [UsedImplicitly] private HashSet<ShapeShiftForm> shapeShiftNeverCastableSet = new HashSet<ShapeShiftForm>();
        [UsedImplicitly] private SpellMechanicsFlags combinedEffectMechanics;
        [UsedImplicitly] private float maxTargetingRadius;
        [UsedImplicitly] private bool someEffectsIgnoreSpellImmunity;

        protected override ScriptableUniqueInfoContainer<SpellInfo> Container => container;
        protected override SpellInfo Data => this;

        /// <summary>
        /// Compressed to 8 bits in <seealso cref="SpellCastRequestEvent"/> and other spell events.
        /// </summary>
        public new int Id => base.Id;

        public SpellExplicitTargetType ExplicitTargetType => explicitTargetType;
        public SpellCastTargetFlags ExplicitCastTargets => explicitCastTargets;
        public SpellDispelType SpellDispel => spellDispel;
        public SpellMechanics Mechanic => mechanic;
        public SpellMechanicsFlags CastIgnoringMechanics => castIgnoringMechanics;
        public SpellMechanicsFlags CombinedEffectMechanics => combinedEffectMechanics;
        public SpellDamageClass DamageClass => damageClass;

        public SpellSchoolMask SchoolMask => schoolMask;
        public SpellPreventionType PreventionType => preventionType;
        public SpellAttributes Attributes => attributes;
        public SpellExtraAttributes AttributesExtra => attributesExtra;
        public SpellCustomAttributes AttributesCustom => attributesCustom;

        public EnityTypeMask TargetEntityTypeMask => targetEntityTypeMask;
        public SpellRangeFlags RangedFlags => rangedFlags;
        public SpellInterruptFlags InterruptFlags => interruptFlags;

        public List<SpellPowerCostInfo> PowerCosts => spellPowerCostInfos;
        public List<SpellEffectInfo> Effects => spellEffectInfos;

        public int CooldownTime => cooldownTime;
        public int CategoryCooldownTime => categoryCooldownTime;
        public int GlobalCooldownTime => globalCooldownTime;
        public int CastTime => castTime;
        public int MinCastTime => minCastTime;
        public int Charges => charges;

        public float MinRangeHostile => minRangeHostile;
        public float MinRangeFriend => minRangeFriend;
        public float MaxRangeHostile => maxRangeHostile;
        public float MaxRangeFriend => maxRangeFriend;
        public float MaxTargetingRadius => maxTargetingRadius;
        public float Speed => speed;

        public bool IsUsingCharges => charges != 0;
        public bool IsPassive => HasAttribute(SpellAttributes.Passive);
        public bool IsDeathPersistent => HasAttribute(SpellAttributes.DeathPersistent);
        public bool IsPositive => !HasAttribute(SpellCustomAttributes.Negative);
        public bool IsSingleTarget => HasAttribute(SpellExtraAttributes.SingleTargetSpell);
        public bool IsAffectingArea => Effects.Exists(effect => effect.IsTargetingArea() && effect.IsEffect(SpellEffectType.PersistentAreaAura) || effect.IsAreaAuraEffect());
        public bool IsTargetingArea => Effects.Exists(effect => effect.IsTargetingArea());
        public bool SomeEffectsIgnoreSpellImmunity => someEffectsIgnoreSpellImmunity;

        protected override void OnRegister()
        {
            base.OnRegister();

            combinedEffectMechanics = Mechanic.AsFlag();
            maxTargetingRadius = 0.0f;
            shapeShiftAlwaysCastableSet = new HashSet<ShapeShiftForm>(shapeShiftAlwaysCastable);
            shapeShiftNeverCastableSet = new HashSet<ShapeShiftForm>(shapeShiftNeverCastable);

            foreach (SpellEffectInfo spellEffectInfo in spellEffectInfos)
            {
                if (spellEffectInfo is EffectApplyAura auraApplyEffect)
                    for (int index = 0; index < auraApplyEffect.AuraInfo.AuraEffects.Count; index++)
                        combinedEffectMechanics |= auraApplyEffect.AuraInfo.AuraEffects[index].Mechanics.AsFlag();

                if (spellEffectInfo.Targeting is SpellTargetingArea areaTargeting)
                    maxTargetingRadius = Mathf.Max(areaTargeting.MaxRadius, maxTargetingRadius);

                if (spellEffectInfo.IgnoresSpellImmunity)
                    someEffectsIgnoreSpellImmunity = true;
            }
        }

        protected override void OnUnregister()
        {
            shapeShiftAlwaysCastableSet.Clear();
            shapeShiftNeverCastableSet.Clear();
            combinedEffectMechanics = Mechanic.AsFlag();
            maxTargetingRadius = 0.0f;
            someEffectsIgnoreSpellImmunity = false;

            base.OnUnregister();
        }

        public bool HasEffect(SpellEffectType effectType)
        {
            return Effects.Exists(effect => effect.EffectType == effectType);
        }

        public bool HasAttribute(SpellAttributes attribute)
        {
            return (Attributes & attribute) != 0;
        }

        public bool HasAttribute(SpellExtraAttributes attribute)
        {
            return (AttributesExtra & attribute) != 0;
        }

        public bool HasAttribute(SpellCustomAttributes attribute)
        {
            return (AttributesCustom & attribute) != 0;
        }

        public bool CanPierceImmuneAura(SpellInfo spellInfo)
        {
            // these spells pierce all avalible spells
            if (HasAttribute(SpellAttributes.UnaffectedByInvulnerability))
                return true;

            // these spells can pierce all        
            if (!HasAttribute(SpellAttributes.UnaffectedBySchoolImmune))
                return false;

            // but not these (Divine shield, Ice block, Cyclone and Banish for example)
            return !(spellInfo.Mechanic == SpellMechanics.ImmuneShield || spellInfo.Mechanic == SpellMechanics.Invulnerability || spellInfo.Mechanic == SpellMechanics.Banish);
        }

        public bool CanDispelAura(SpellInfo auraSpellInfo)
        {
            // unaffected by invulnerability spells can dispel any non death persistent aura
            if (HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !auraSpellInfo.IsDeathPersistent)
                return true;

            // unaffected by invulnerability auras can not be dispelled
            if (auraSpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability))
                return false;

            // auras ignoring school immunity can not be dispelled
            if (auraSpellInfo.HasAttribute(SpellAttributes.UnaffectedBySchoolImmune))
                return false;

            return true;
        }

        public bool CanCancelAuraType(AuraEffectType auraEffectType, Unit caster)
        {
            IReadOnlyList<AuraEffect> activeEffects = caster.Auras.GetAuraEffects(auraEffectType);
            if (activeEffects == null)
                return true;

            for (int i = 0; i < activeEffects.Count; i++)
            {
                if (CanCancelAura(activeEffects[i]))
                    continue;

                return false;
            }

            return true;
        }

        public bool CanCancelAura(AuraEffect auraEffect)
        {
            if (!HasAttribute(SpellAttributes.DispelAurasOnImmunity))
                return false;

            if (auraEffect.Aura.SpellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability))
                return false;

            foreach (SpellEffectInfo effectInfo in spellEffectInfos)
            {
                if (!(effectInfo is EffectApplyAura applyAuraEffect))
                    continue;

                for (int index = 0; index < applyAuraEffect.AuraInfo.AuraEffects.Count; index++)
                {
                    AuraEffectInfo auraEffectInfo = applyAuraEffect.AuraInfo.AuraEffects[index];
                    switch (auraEffectInfo)
                    {
                        case AuraEffectInfoSchoolImmunity schoolImmunity:
                            if (auraEffect.Aura.SpellInfo.HasAttribute(SpellAttributes.UnaffectedBySchoolImmune))
                                continue;

                            if (schoolImmunity.SchoolMask.HasAnyFlag(SchoolMask))
                                return true;
                            break;
                    }
                }
            }

            return false;
        }

        public bool CanCancelForm(Spell spell)
        {
            if (spell.Caster.ShapeShiftForm == ShapeShiftForm.None)
                return false;

            if (!HasAttribute(SpellExtraAttributes.CastableOnlyNonShapeShifted))
                return false;

            if (shapeShiftAlwaysCastableSet.Contains(spell.Caster.ShapeShiftForm))
                return false;

            bool impossibleToCancel = spell.Caster.ShapeShiftSpellInfo.HasAttribute(SpellAttributes.CantCancel);
            bool mayNotCancel = HasAttribute(SpellExtraAttributes.CastCantCancelShapeShift);
            return !impossibleToCancel && !mayNotCancel;
        }

        public SpellCastResult CheckTarget(Unit caster, Unit target, Spell spell, bool isImplicit = true)
        {
            if (HasAttribute(SpellAttributes.CantTargetSelf) && caster == target)
                return SpellCastResult.BadTargets;

            if (HasAttribute(SpellCustomAttributes.Pickpocket) && caster is Player && target is Player && caster != target)
                return SpellCastResult.BadTargets;

            if (HasAttribute(SpellAttributes.OnlyTargetPlayers) && !(target is Player))
                return SpellCastResult.TargetNotPlayer;

            if (target != caster && (caster.IsControlledByPlayer || !IsPositive) && target is Player player && !player.IsVisible)
                return SpellCastResult.BmOrInvisGod;

            if (target.HasState(UnitControlState.InFlight) && !HasAttribute(SpellCustomAttributes.AllowInFlightTarget))
                return SpellCastResult.BadTargets;

            if (target.HasAuraType(AuraEffectType.PreventResurrection))
                if (HasEffect(SpellEffectType.SelfResurrect) || HasEffect(SpellEffectType.Resurrect))
                    return SpellCastResult.TargetCannotBeResurrected;

            foreach (SpellCastCondition castCondition in targetingConditions)
                if (castCondition.IsApplicableAndInvalid(caster, target, spell))
                    return castCondition.FailedResult;

            return SpellCastResult.Success;
        }

        public SpellCastResult CheckExplicitTarget(Unit caster, Unit target)
        {
            if (ExplicitTargetType != SpellExplicitTargetType.Target)
                return SpellCastResult.Success;

            if (ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
            {
                if(target == null)
                    return SpellCastResult.BadTargets;

                if (target.IsDead && !HasAttribute(SpellAttributes.CanTargetDead))
                    return SpellCastResult.TargetDead;

                if (!HasAttribute(SpellExtraAttributes.CanTargetInvisible) && !caster.CanSeeOrDetect(target))
                    return SpellCastResult.BadTargets;

                if (ExplicitCastTargets.HasTargetFlag(SpellCastTargetFlags.UnitEnemy) && caster.IsHostileTo(target))
                    return SpellCastResult.Success;

                if (ExplicitCastTargets.HasTargetFlag(SpellCastTargetFlags.UnitAlly) && caster.IsFriendlyTo(target))
                    return SpellCastResult.Success;

                return SpellCastResult.BadTargets;
            }

            return SpellCastResult.Success;
        }

        public SpellCastResult CheckShapeShift(Unit caster)
        {
            if (shapeShiftNeverCastableSet.Contains(caster.ShapeShiftForm))
                return SpellCastResult.NotThisShapeShift;

            if (shapeShiftAlwaysCastableSet.Contains(caster.ShapeShiftForm))
                return SpellCastResult.Success;

            if (caster.ShapeShiftForm != ShapeShiftForm.None)
            {
                if (HasAttribute(SpellExtraAttributes.CastableOnlyShapeShifted))
                    return SpellCastResult.NotThisShapeShift;

                bool impossibleToCancel = caster.ShapeShiftSpellInfo.HasAttribute(SpellAttributes.CantCancel);
                bool mayNotCancel = HasAttribute(SpellExtraAttributes.CastCantCancelShapeShift);

                if (HasAttribute(SpellExtraAttributes.CastableOnlyNonShapeShifted) && (impossibleToCancel || mayNotCancel))
                    return SpellCastResult.NotOutOfShapeShift;
            }
            else if (HasAttribute(SpellExtraAttributes.CastableOnlyShapeShifted))
                return SpellCastResult.NotShapeShifted;

            return SpellCastResult.Success;
        }

        public float GetMinRange(bool positive)
        {
            return positive ? MinRangeFriend : MinRangeHostile;
        }

        public float GetMaxRange(bool positive, Unit caster = null, Spell spell = null)
        {
            float range = positive ? MaxRangeFriend : MaxRangeHostile;
            if (caster != null && spell != null)
                range = caster.Spells.ApplySpellModifier(spell, SpellModifierType.Range, range);
            return range;
        }

        public void CalculatePowerCosts(Unit caster, List<(SpellPowerType, int)> powerCosts, Spell spell = null)
        {
            foreach (SpellPowerCostInfo powerCostInfo in spellPowerCostInfos)
            {
                int powerCost = powerCostInfo.PowerCost;
                if (powerCostInfo.PowerCostPercentage > 0)
                {
                    switch (powerCostInfo.SpellPowerType)
                    {
                        case SpellPowerType.Health:
                            powerCost += caster.MaxHealth.ApplyPercentage(powerCostInfo.PowerCostPercentage);
                            break;
                        case SpellPowerType.Mana:
                            powerCost += caster.Attributes.MaxPowerWithNoMods(SpellPowerType.Mana).ApplyPercentage(powerCostInfo.PowerCostPercentage);
                            break;
                        case SpellPowerType.Rage:
                        case SpellPowerType.Focus:
                        case SpellPowerType.Energy:
                            powerCost += caster.Attributes.MaxPower(powerCostInfo.SpellPowerType).ApplyPercentage(powerCostInfo.PowerCostPercentage);
                            break;
                        default:
                            continue;
                    }
                }

                if (spell != null && caster is Player player)
                    powerCost = (int)player.Spells.ApplySpellModifier(spell, SpellModifierType.Cost, powerCost);

                powerCosts.Add((powerCostInfo.SpellPowerType, powerCost));
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Populate Passive"), UsedImplicitly]
        private void PopulatePassive()
        {
            explicitTargetType = SpellExplicitTargetType.Caster;
            damageClass = SpellDamageClass.None;
            spellDispel = SpellDispelType.None;
            mechanic = SpellMechanics.None;

            explicitCastTargets = SpellCastTargetFlags.UnitAlly;
            schoolMask = 0;
            preventionType = 0;
            attributes = SpellAttributes.Passive;
            attributesExtra = SpellExtraAttributes.DoesNotTriggerGcd | SpellExtraAttributes.IgnoreGcd | SpellExtraAttributes.NotStealable;
            attributesCustom = SpellCustomAttributes.CastWithoutAnimation;

            targetEntityTypeMask = EnityTypeMask.Unit;
            rangedFlags = SpellRangeFlags.Default;
            interruptFlags = 0;
            castIgnoringMechanics = 0;

            cooldownTime = 0;
            categoryCooldownTime = 0;
            globalCooldownTime = 0;
            castTime = 0;
            minCastTime = 0;

            minRangeHostile = 0;
            minRangeFriend = 0;
            maxRangeHostile = 0;
            maxRangeFriend = 0;
            speed = 0;
        }
#endif
    }
}