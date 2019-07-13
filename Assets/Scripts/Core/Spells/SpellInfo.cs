using System.Collections.Generic;
using Common;
using Core.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Info", menuName = "Game Data/Spells/Spell Info", order = 1)]
    public sealed class SpellInfo : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private int id;
        [SerializeField, UsedImplicitly] private string spellName;

        [SerializeField, EnumFlag, UsedImplicitly] private SpellCastTargetFlags explicitCastTargets;
        [SerializeField, UsedImplicitly] private SpellExplicitTargetType explicitTargetType;
        [SerializeField, UsedImplicitly] private SpellDamageClass damageClass;
        [SerializeField, UsedImplicitly] private SpellDispelType spellDispel;
        [SerializeField, UsedImplicitly] private SpellMechanics mechanic;

        [SerializeField, EnumFlag, UsedImplicitly] private SpellSchoolMask schoolMask;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellPreventionType preventionType;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellAttributes attributes;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellExtraAttributes attributesExtra;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellCustomAttributes attributesCustom;

        [SerializeField, EnumFlag, UsedImplicitly] private EnityTypeMask targetEntityTypeMask;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellRangeFlags rangedFlags;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellInterruptFlags interruptFlags;

        [SerializeField, UsedImplicitly] private int cooldownTime;
        [SerializeField, UsedImplicitly] private int categoryCooldownTime;
        [SerializeField, UsedImplicitly] private int globalCooldownTime;
        [SerializeField, UsedImplicitly] private int castTime;
        [SerializeField, UsedImplicitly] private int minCastTime;

        [SerializeField, UsedImplicitly] private float minRangeHostile;
        [SerializeField, UsedImplicitly] private float minRangeFriend;
        [SerializeField, UsedImplicitly] private float maxRangeHostile;
        [SerializeField, UsedImplicitly] private float maxRangeFriend;
        [SerializeField, UsedImplicitly] private float speed;
        [SerializeField, UsedImplicitly] private int stackAmount;
        [SerializeField, UsedImplicitly] private int maxAffectedTargets;

        [SerializeField, UsedImplicitly] private List<SpellEffectInfo> spellEffectInfos = new List<SpellEffectInfo>();
        [SerializeField, UsedImplicitly] private List<SpellPowerEntry> spellPowerEntries = new List<SpellPowerEntry>();
        [SerializeField, UsedImplicitly] private List<SpellProcsPerMinuteModifier> procsPerMinuteModifiers;
        [SerializeField, UsedImplicitly] private List<SpellCastCondition> targetingConditions;

        /// <summary>
        /// Compressed to 8 bits in Spell Events.
        /// </summary>
        public int Id => id;
        public string SpellName => spellName;

        public SpellExplicitTargetType ExplicitTargetType => explicitTargetType;
        public SpellCastTargetFlags ExplicitCastTargets => explicitCastTargets;
        public SpellDispelType SpellDispel => spellDispel;
        public SpellMechanics Mechanic => mechanic;
        public SpellDamageClass DamageClass => damageClass;

        public SpellSchoolMask SchoolMask => schoolMask;
        public SpellPreventionType PreventionType => preventionType;
        public SpellAttributes Attributes => attributes;
        public SpellExtraAttributes AttributesExtra => attributesExtra;
        public SpellCustomAttributes AttributesCustom => attributesCustom;

        public EnityTypeMask TargetEntityTypeMask => targetEntityTypeMask;
        public SpellRangeFlags RangedFlags => rangedFlags;
        public SpellInterruptFlags InterruptFlags => interruptFlags;

        public List<SpellPowerEntry> PowerCosts => spellPowerEntries;
        public List<SpellEffectInfo> Effects => spellEffectInfos;
        public List<SpellProcsPerMinuteModifier> ProcsPerMinuteModifiers => procsPerMinuteModifiers;

        public int CooldownTime => cooldownTime;
        public int CategoryCooldownTime => categoryCooldownTime;
        public int GlobalCooldownTime => globalCooldownTime;
        public int CastTime => castTime;
        public int MinCastTime => minCastTime;

        public float MinRangeHostile => minRangeHostile;
        public float MinRangeFriend => minRangeFriend;
        public float MaxRangeHostile => maxRangeHostile;
        public float MaxRangeFriend => maxRangeFriend;
        public float Speed => speed;

        public int StackAmount => stackAmount;
        public int MaxAffectedTargets => maxAffectedTargets;

        #region Spell info flags and properties

        public bool HasEffect(SpellEffectType effectType)
        {
            return Effects.Exists(effect => effect.EffectType == effectType);
        }

        public bool HasAura(AuraEffectType auraEffect)
        {
            return Effects.Exists(effect => effect.IsAura(auraEffect));
        }

        public bool HasAreaAuraEffect()
        {
            return Effects.Exists(effect => effect.IsAreaAuraEffect());
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

        public bool IsAffectingArea()
        {
            return Effects.Exists(effect => effect.IsTargetingArea() && effect.IsEffect(SpellEffectType.PersistentAreaAura) || effect.IsAreaAuraEffect());
        }

        public bool IsTargetingArea()
        {
            return Effects.Exists(effect => effect.IsTargetingArea());
        }

        public bool IsPassive()
        {
            return HasAttribute(SpellAttributes.Passive);
        }

        public bool IsStackableOnOneSlotWithDifferentCasters()
        {
            return StackAmount > 1 && !HasAttribute(SpellAttributes.StackForDiffCasters);
        }

        public bool IsDeathPersistent()
        {
            return HasAttribute(SpellAttributes.DeathPersistent);
        }

        public bool IsPositive()
        {
            return !HasAttribute(SpellCustomAttributes.Negative);
        }

        public bool IsPositiveEffect(int effIndex)
        {
            return !HasAttribute(SpellCustomAttributes.Negative);
        }

        public bool IsChanneled()
        {
            return HasAttribute(SpellAttributes.Channeled);
        }

        public bool CanPierceImmuneAura(SpellInfo aura)
        {
            // these spells pierce all avalible spells (Resurrection Sickness for example)
            if (HasAttribute(SpellAttributes.UnaffectedByInvulnerability))
                return true;

            // these spells (Cyclone for example) can pierce all...         
            if (!HasAttribute(SpellAttributes.UnaffectedBySchoolImmune) || aura == null)
                return false;

            // ...but not these (Divine shield, Ice block, Cyclone and Banish for example)
            return !(aura.Mechanic == SpellMechanics.ImmuneShield || aura.Mechanic == SpellMechanics.Invulnerability || aura.Mechanic == SpellMechanics.Banish);
        }

        public bool CanDispelAura(SpellInfo aura)
        {
            // These spells (like Mass SpellDispel) can dispell all auras, except death persistent ones (like Dungeon and Battleground Deserter)
            if (HasAttribute(SpellAttributes.UnaffectedByInvulnerability) && !aura.IsDeathPersistent())
                return true;

            // These auras (like Divine Shield) can't be dispelled
            if (aura.HasAttribute(SpellAttributes.UnaffectedByInvulnerability))
                return false;

            // These auras (Cyclone for example) are not dispelable
            if (aura.HasAttribute(SpellAttributes.UnaffectedBySchoolImmune))
                return false;

            return true;
        }

        public bool IsSingleTarget()
        {
            return HasAttribute(SpellExtraAttributes.SingleTargetSpell);
        }

        #endregion

        #region Usage checks

        public SpellCastResult CheckTarget(Unit caster, Unit target, Spell spell, bool isImplicit = true)
        {
            if (HasAttribute(SpellAttributes.CantTargetSelf) && caster == target)
                return SpellCastResult.BadTargets;

            if (!HasAttribute(SpellExtraAttributes.CanTargetInvisible) && !caster.CanSeeOrDetect(target, isImplicit))
                return SpellCastResult.BadTargets;

            if (HasAttribute(SpellCustomAttributes.Pickpocket) && caster is Player && target is Player && caster != target)
                return SpellCastResult.BadTargets;

            if (HasAttribute(SpellAttributes.OnlyTargetPlayers) && !(target is Player))
                return SpellCastResult.TargetNotPlayer;

            if (target != caster && (caster.IsControlledByPlayer || !IsPositive()) && target is Player player && !player.IsVisible)
                return SpellCastResult.BmOrInvisGod;

            if (target.HasState(UnitState.InFlight) && !HasAttribute(SpellCustomAttributes.AllowInFlightTarget))
                return SpellCastResult.BadTargets;

            if (target.HasAuraType(AuraEffectType.PreventResurrection))
                if (HasEffect(SpellEffectType.SelfResurrect) || HasEffect(SpellEffectType.Resurrect))
                    return SpellCastResult.TargetCannotBeResurrected;

            foreach (SpellCastCondition castCondition in targetingConditions)
                if (castCondition.With(caster, target, spell).IsApplicableAndInvalid)
                    return castCondition.FailedResult;

            return SpellCastResult.Success;
        }

        public SpellCastResult CheckExplicitTarget(Unit caster, Unit target)
        {
            if (ExplicitCastTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
            {
                if(target == null)
                    return SpellCastResult.BadTargets;

                if (ExplicitCastTargets.HasTargetFlag(SpellCastTargetFlags.UnitEnemy) && caster.IsHostileTo(target))
                    return SpellCastResult.Success;

                if (ExplicitCastTargets.HasTargetFlag(SpellCastTargetFlags.UnitAlly) && caster.IsFriendlyTo(target))
                    return SpellCastResult.Success;

                return SpellCastResult.BadTargets;
            }

            return SpellCastResult.Success;
        }

        #endregion

        #region Range, durations and cost

        public float GetMinRange(bool positive)
        {
            return positive ? MinRangeFriend : MinRangeHostile;
        }

        public float GetMaxRange(bool positive, Unit caster = null, Spell spell = null)
        {
            float range = positive ? MaxRangeFriend : MaxRangeHostile;
            if (caster != null && spell != null)
                caster.ApplySpellMod(spell.SpellInfo, SpellModifierType.Range, ref range);
            return range;
        }

        public int CalcCastTime(byte level = 0, Spell spell = null)
        {
            int resultCastTime = CastTime;
            if (resultCastTime <= 0)
                return 0;

            spell?.Caster.ModSpellCastTime(this, ref resultCastTime, spell);

            if (resultCastTime < MinCastTime)
                resultCastTime = MinCastTime;

            if (resultCastTime < 0)
                resultCastTime = 0;

            return resultCastTime > 0 ? resultCastTime : 0;
        }

        public List<SpellResourceCost> CalcPowerCost(Unit caster, SpellSchoolMask schoolMask)
        {
            var powers = PowerCosts;
            var costs = new List<SpellResourceCost>(PowerCosts.Count);
            int healthCost = 0;

            foreach (var power in powers)
            {
                // bse powerCost
                int powerCost = power.PowerCost;
                // percent cost from total amount
                if (power.PowerCostPercentage > 0)
                {
                    switch (power.SpellResourceType)
                    {
                        // health as power used
                        case SpellResourceType.Health:
                            powerCost += caster.MaxHealth.CalculatePercentage(power.PowerCostPercentage);
                            break;
                        case SpellResourceType.Mana:
                            powerCost += caster.BaseMana.CalculatePercentage(power.PowerCostPercentage);
                            break;
                        case SpellResourceType.Rage:
                        case SpellResourceType.Focus:
                        case SpellResourceType.Energy:
                            powerCost += caster.GetMaxPower(power.SpellResourceType).CalculatePercentage(power.PowerCostPercentage);
                            break;
                        case SpellResourceType.Runes:
                        case SpellResourceType.RunicPower:
                            Debug.unityLogger.LogWarning("Spells", $"CalculateManaCost for {power.SpellResourceType}: Not implemented yet!");
                            break;
                        default:
                            Debug.unityLogger.LogError("Spells", $"CalculateManaCost: Unknown power type '{power.SpellResourceType}' in spell {Id}");
                            continue;
                    }
                }

                caster.ApplySpellMod(this, SpellModifierType.Cost, ref powerCost);

                if (power.SpellResourceType == SpellResourceType.Health)
                {
                    healthCost += powerCost;
                    continue;
                }

                bool found = false;
                for (int i = 0; i < costs.Count; i++)
                {
                    if(costs[i].SpellResource == power.SpellResourceType)
                    {
                        costs[i] = new SpellResourceCost(costs[i].SpellResource, costs[i].Amount + powerCost);
                        found = true;
                    }
                }

                if (!found)
                    costs.Add(new SpellResourceCost(power.SpellResourceType, powerCost));
            }

            if (healthCost > 0)
                costs.Add(new SpellResourceCost(SpellResourceType.Health, healthCost));

            costs.RemoveAll(cost => cost.SpellResource != SpellResourceType.Runes && cost.Amount <= 0);

            return costs;
        }

        public float CalcProcPPM(Unit caster)
        {
            float ppm = 1.0f;
            if (caster == null)
                return ppm;

            foreach (var mod in ProcsPerMinuteModifiers)
            {
                switch (mod.Type)
                {
                    case SpellProcsPerMinuteModType.Haste:
                    {
                        ppm *= 1.0f + CalcPPMHasteMod(mod, caster);
                        break;
                    }
                    case SpellProcsPerMinuteModType.Crit:
                    {
                        ppm *= 1.0f + CalcPPMCritMod(mod, caster);
                        break;
                    }
                    case SpellProcsPerMinuteModType.Spec:
                    {
                        if(caster is Player player && player.SpecId == mod.Parameter)
                            ppm *= 1.0f + mod.Value;
                        break;
                    }
                }
            }

            return ppm;
        }

        public static float CalcPPMHasteMod(SpellProcsPerMinuteModifier mod, Unit caster)
        {
            float haste = caster.ModHaste;
            float rangedHaste = caster.ModRangedHaste;
            float spellHaste = caster.ModSpellHaste;
            float regenHaste = caster.ModRegenHaste;

            switch (mod.Parameter)
            {
                case 1:
                    return (1.0f / haste - 1.0f) * mod.Value;
                case 2:
                    return (1.0f / rangedHaste - 1.0f) * mod.Value;
                case 3:
                    return (1.0f / spellHaste - 1.0f) * mod.Value;
                case 4:
                    return (1.0f / regenHaste - 1.0f) * mod.Value;
                case 5:
                    return (1.0f / Mathf.Min(haste, rangedHaste, spellHaste, regenHaste) - 1.0f) * mod.Value;
            }

            return 0.0f;
        }

        public static float CalcPPMCritMod(SpellProcsPerMinuteModifier mod, Unit caster)
        {
            if (!(caster is Player))
                return 0.0f;

            float crit = caster.CritPercentage;
            float rangedCrit = caster.RangedCritPercentage;
            float spellCrit = caster.SpellCritPercentage;

            switch (mod.Parameter)
            {
                case 1:
                    return crit * mod.Value * 0.01f;
                case 2:
                    return rangedCrit * mod.Value * 0.01f;
                case 3:
                    return spellCrit * mod.Value * 0.01f;
                case 4:
                    return Mathf.Min(crit, rangedCrit, spellCrit) * mod.Value * 0.01f;
            }

            return 0.0f;
        }

        #endregion
    }
}