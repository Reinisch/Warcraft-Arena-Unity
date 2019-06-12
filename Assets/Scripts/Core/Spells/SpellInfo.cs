using System;
using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Info", menuName = "Game Data/Spells/Spell Info", order = 1)]
    public class SpellInfo : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private int id;
        [SerializeField, UsedImplicitly] private string spellName;

        [SerializeField, UsedImplicitly] private SpellDamageClass damageClass;
        [SerializeField, UsedImplicitly] private SpellDispelType spellDispel;
        [SerializeField, UsedImplicitly] private SpellMechanics mechanic;

        [SerializeField, EnumFlag, UsedImplicitly] private SpellSchoolMask schoolMask;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellPreventionType preventionType;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellAttributes attributes;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellExtraAttributes attributesExtra;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellCustomAttributes attributesCustom;

        [SerializeField, EnumFlag, UsedImplicitly] private EnityTypeMask targetEntityTypeMask;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellCastTargetFlags targets;
        [SerializeField, EnumFlag, UsedImplicitly] private AuraStateType casterAuraState;
        [SerializeField, EnumFlag, UsedImplicitly] private AuraStateType targetAuraState;
        [SerializeField, EnumFlag, UsedImplicitly] private AuraStateType excludeCasterAuraState;
        [SerializeField, EnumFlag, UsedImplicitly] private AuraStateType excludeTargetAuraState;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellRangeFlags rangedFlags;
        [SerializeField, EnumFlag, UsedImplicitly] private SpellInterruptFlags interruptFlags;

        [SerializeField, UsedImplicitly] private int duration;
        [SerializeField, UsedImplicitly] private int maxDuration;
        [SerializeField, UsedImplicitly] private int recoveryTime;
        [SerializeField, UsedImplicitly] private int categoryRecoveryTime;
        [SerializeField, UsedImplicitly] private int castTime;
        [SerializeField, UsedImplicitly] private int minCastTime;

        [SerializeField, UsedImplicitly] private float minRangeHostile;
        [SerializeField, UsedImplicitly] private float minRangeFriend;
        [SerializeField, UsedImplicitly] private float maxRangeHostile;
        [SerializeField, UsedImplicitly] private float maxRangeFriend;
        [SerializeField, UsedImplicitly] private float speed;
        [SerializeField, UsedImplicitly] private int stackAmount;
        [SerializeField, UsedImplicitly] private int maxAffectedTargets;

        [SerializeField, UsedImplicitly] private float procPerMinuteBase;
        [SerializeField, UsedImplicitly] private int procCooldown;
        [SerializeField, UsedImplicitly] private int procCharges;
        [SerializeField, UsedImplicitly] private int procChance;

        [SerializeField, UsedImplicitly] private List<SpellEffectInfo> spellEffectInfos = new List<SpellEffectInfo>();
        [SerializeField, UsedImplicitly] private List<SpellPowerEntry> spellPowerEntries = new List<SpellPowerEntry>();
        [SerializeField, UsedImplicitly] private SpellSoundSettings soundSettings;
        [SerializeField, UsedImplicitly] private List<SpellProcsPerMinuteModifier> procsPerMinuteModifiers;

        /// <summary>
        /// Compressed to 8 bits in Spell Events.
        /// </summary>
        public int Id => id;
        public string SpellName => spellName;

        public SpellDispelType SpellDispel => spellDispel;
        public SpellMechanics Mechanic => mechanic;
        public SpellDamageClass DamageClass => damageClass;

        public SpellSchoolMask SchoolMask => schoolMask;
        public SpellPreventionType PreventionType => preventionType;
        public SpellAttributes Attributes => attributes;
        public SpellExtraAttributes AttributesExtra => attributesExtra;
        public SpellCustomAttributes AttributesCustom => attributesCustom;

        public SpellCastTargetFlags Targets => targets;
        public AuraStateType CasterAuraState => casterAuraState;
        public AuraStateType TargetAuraState => targetAuraState;
        public EnityTypeMask TargetEntityTypeMask => targetEntityTypeMask;
        public AuraStateType ExcludeCasterAuraState => excludeCasterAuraState;
        public AuraStateType ExcludeTargetAuraState => excludeTargetAuraState;
        public SpellRangeFlags RangedFlags => rangedFlags;
        public SpellInterruptFlags InterruptFlags => interruptFlags;

        public int ProcChance => procChance;
        public int ProcCharges => procCharges;
        public int ProcCooldown => procCooldown;
        public float ProcPerMinuteBase => procPerMinuteBase;

        public List<SpellPowerEntry> PowerCosts => spellPowerEntries;
        public List<SpellEffectInfo> Effects => spellEffectInfos;
        public SpellSoundSettings SoundSettings => soundSettings;
        public List<SpellProcsPerMinuteModifier> ProcsPerMinuteModifiers => procsPerMinuteModifiers;

        public int Duration => duration;
        public int MaxDuration => maxDuration;
        public int RecoveryTime => recoveryTime;
        public int CategoryRecoveryTime => categoryRecoveryTime;
        public int CastTime => castTime;
        public int MinCastTime => minCastTime;

        public float MinRangeHostile => minRangeHostile;
        public float MinRangeFriend => minRangeFriend;
        public float MaxRangeHostile => maxRangeHostile;
        public float MaxRangeFriend => maxRangeFriend;
        public float Speed => speed;

        public int StackAmount => stackAmount;
        public int MaxAffectedTargets => maxAffectedTargets;

        public SpellCastTargetFlags ExplicitTargetMask { get; private set; }

        public void Initialize()
        {
            InitializeExplicitTargetMask();
            Effects.ForEach(effect => effect.Initialize(this));
        }

        public void Deinitialize()
        {
            ExplicitTargetMask = 0;
            Effects.ForEach(effect => effect.Deinitialize());
        }
    
        #region Spell info flags and properties

        public bool HasEffect(SpellEffectType effectType)
        {
            return Effects.Exists(effect => effect.EffectType == effectType);
        }

        public bool HasAura(AuraType aura)
        {
            return Effects.Exists(effect => effect.IsAura(aura));
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

        public bool NeedsExplicitUnitTarget()
        {
            return (ExplicitTargetMask & SpellCastTargetFlags.UnitMask) != 0;
        }

        public bool NeedsToBeTriggeredByCaster(SpellInfo triggeringSpell)
        {
            if (NeedsExplicitUnitTarget())
                return true;

            if (!triggeringSpell.IsChanneled())
                return false;

            SpellCastTargetFlags targetMask = 0;
            foreach(var effect in Effects)
                if (effect.MainTargeting.ReferenceType != SpellTargetReferences.Caster && effect.SecondaryTargeting.ReferenceType != SpellTargetReferences.Caster)
                    targetMask |= effect.ImplicitTargetFlags;

            return (targetMask & SpellCastTargetFlags.UnitMask) != 0;
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

        public SpellCastResult CheckLocation(uint mapId, uint zoneId, uint areaId, Player player = null)
        {
            throw new NotImplementedException();
        }

        public SpellCastResult CheckTarget(Unit caster, WorldEntity target, bool isImplicit = true)
        {
            if (HasAttribute(SpellAttributes.CantTargetSelf) && caster == target)
                return SpellCastResult.BadTargets;

            // check visibility - ignore stealth for implicit (area) targets
            if (!HasAttribute(SpellExtraAttributes.CanTargetInvisible) && !caster.CanSeeOrDetect(target, isImplicit))
                return SpellCastResult.BadTargets;

            Unit unitTarget = target as Unit;

            // creature/player specific target checks
            if (unitTarget != null)
            {
                if (caster != unitTarget)
                {
                    if (caster.EntityType == EntityType.Player)
                    {
                        if (HasAttribute(SpellCustomAttributes.Pickpocket))
                            if (unitTarget.EntityType == EntityType.Player)
                                return SpellCastResult.BadTargets;
                    }
                }
            }
            // other types of objects - always valid
            else
                return SpellCastResult.Success;

            // corpseOwner and unit specific target checks
            if (HasAttribute(SpellAttributes.OnlyTargetPlayers) && unitTarget.EntityType != EntityType.Player)
                return SpellCastResult.TargetNotPlayer;

            if (!CheckTargetCreatureType(unitTarget))
                return target.EntityType == EntityType.Player ? SpellCastResult.TargetIsPlayer : SpellCastResult.BadTargets;

            // check GM mode and GM invisibility - only for player casts (npc casts are controlled by AI) and negative spells
            if (unitTarget != caster && (caster.IsControlledByPlayer() || !IsPositive()) && unitTarget.EntityType == EntityType.Player)
            {
                var playerTarget = (Player) unitTarget ;
                if (!playerTarget.IsVisible)
                    return SpellCastResult.BmOrInvisgod;
            }

            if (unitTarget.HasUnitState(UnitState.InFlight) && !HasAttribute(SpellCustomAttributes.AllowInflightTarget))
                return SpellCastResult.BadTargets;

            if (TargetAuraState != 0 && !unitTarget.HasAuraState(TargetAuraState, this, caster))
                return SpellCastResult.TargetAurastate;

            if (ExcludeTargetAuraState != 0 && unitTarget.HasAuraState(ExcludeTargetAuraState, this, caster))
                return SpellCastResult.TargetAurastate;

            if (unitTarget.HasAuraType(AuraType.PreventResurrection))
                if (HasEffect(SpellEffectType.SelfResurrect) || HasEffect(SpellEffectType.Resurrect))
                    return SpellCastResult.TargetCannotBeResurrected;

            return SpellCastResult.Success;
        }

        public SpellCastResult CheckExplicitTarget(Unit caster, WorldEntity target)
        {
            SpellCastTargetFlags neededTargets = ExplicitTargetMask;
            if (target == null && neededTargets.HasAnyFlag(SpellCastTargetFlags.UnitMask))
                return SpellCastResult.Success;

            var unitTarget = target as Unit;
            if(unitTarget == null)
                return SpellCastResult.Success;

            if ((neededTargets & SpellCastTargetFlags.UnitMask) != 0)
            {
                if (neededTargets.HasTargetFlag(SpellCastTargetFlags.UnitEnemy) && caster.IsValidAttackTarget(unitTarget, this))
                    return SpellCastResult.Success;

                if (neededTargets.HasTargetFlag(SpellCastTargetFlags.UnitAlly) && caster.IsValidAssistTarget(unitTarget, this))
                    return SpellCastResult.Success;

                return SpellCastResult.BadTargets;
            }

            return SpellCastResult.Success;
        }

        public SpellCastResult CheckVehicle(Unit caster)
        {
            throw new NotImplementedException();
        }

        public bool CheckTargetCreatureType(Unit target)
        {
            if (target.IsMagnet())
                return true;

            uint creatureTypeMask = target.GetCreatureTypeMask();
            return TargetEntityTypeMask == 0 || creatureTypeMask == 0 || (creatureTypeMask & (int)TargetEntityTypeMask) > 0;
        }

        #endregion

        #region School, mechanics, spellDispel and states

        public AuraStateType GetAuraState()
        {
            // Enrage aura state
            if (SpellDispel == SpellDispelType.Enrage)
                return AuraStateType.Enrage;

            if (SchoolMask.HasFlag(SpellSchoolMask.Frost))
                if (Effects.Exists(effect => effect.IsAura() && (effect.AuraType == AuraType.ModStun || effect.AuraType == AuraType.ModRoot)))
                    return AuraStateType.Frozen;

            return AuraStateType.None;
        }

        public SpellMechanics GetEffectMechanic(int effIndex)
        {
            SpellEffectInfo effect = Effects[effIndex];
            if (effect.Mechanic != 0)
                return effect.Mechanic;

            return Mechanic != 0 ? Mechanic : SpellMechanics.None;
        }

        public uint GetEffectMechanicMask(int effIndex)
        {
            uint mask = 0;
            if (Mechanic != 0)
                mask |= (uint)(1 << (int)Mechanic);

            foreach (var spellEffectInfo in Effects)
                if (spellEffectInfo != null && spellEffectInfo.Index == effIndex && spellEffectInfo.Mechanic != 0)
                    mask |= (uint) (1 << (int) spellEffectInfo.Mechanic);

            return mask;
        }

        public uint GetSpellMechanicMaskByEffectMask(int effectMask)
        {
            uint mask = 0;
            if (Mechanic != 0)
                mask |= (uint)(1 << (int)Mechanic);

            foreach (var spellEffectInfo in Effects)
                if (spellEffectInfo != null && (effectMask & (1 << spellEffectInfo.Index)) != 0 && spellEffectInfo.Mechanic != 0)
                    mask |= (uint)(1 << (int)spellEffectInfo.Mechanic);

            return mask;
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
            {
                Player modOwner = caster.SpellModOwner;
                if (modOwner != null)
                    modOwner.ApplySpellMod(spell.SpellInfo, SpellModifierType.Range, ref range);
            }
            return range;
        }

        public int GetMaxTicks()
        {
            int dotDuration = Duration;
            if (dotDuration == 0)
                return 1;

            // 200% limit
            if (dotDuration > 30000)
                dotDuration = 30000;

            foreach (var effect in Effects)
                if (effect != null && effect.EffectType == SpellEffectType.ApplyAura)
                    switch (effect.AuraType)
                    {
                        case AuraType.PeriodicDamage:
                        case AuraType.PeriodicHeal:
                        case AuraType.PeriodicLeech:
                            if (effect.ApplyAuraPeriod != 0)
                                return dotDuration / effect.ApplyAuraPeriod;
                            break;
                    }

            return 6;
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

        public int GetRecoveryTime()
        {
            return RecoveryTime > CategoryRecoveryTime ? RecoveryTime : CategoryRecoveryTime;
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
                            powerCost += (int)caster.GetCreateMana().CalculatePercentage(power.PowerCostPercentage);
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

                Player modOwner = caster.SpellModOwner;

                // apply cost mod by spell
                if (modOwner != null)
                    modOwner.ApplySpellMod(this, SpellModifierType.Cost, ref powerCost);

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
            float ppm = ProcPerMinuteBase;
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
                        Player playerCaster = caster as Player;
                        if(playerCaster != null)
                            if (playerCaster.GetUintValue(EntityFields.CurrentSpecId) == mod.Parameter)
                                ppm *= 1.0f + mod.Value;
                        break;
                    }
                }
            }

            return ppm;
        }

        public static float CalcPPMHasteMod(SpellProcsPerMinuteModifier mod, Unit caster)
        {
            float haste = caster.GetFloatValue(EntityFields.ModHaste);
            float rangedHaste = caster.GetFloatValue(EntityFields.ModRangedHaste);
            float spellHaste = caster.GetFloatValue(EntityFields.UnitModCastHaste);
            float regenHaste = caster.GetFloatValue(EntityFields.ModHasteRegen);

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
            if (caster.EntityType != EntityType.Player)
                return 0.0f;

            float crit = caster.GetFloatValue(EntityFields.CritPercentage);
            float rangedCrit = caster.GetFloatValue(EntityFields.RangedCritPercentage);
            float spellCrit = caster.GetFloatValue(EntityFields.SpellCritPercentage);

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

        #region Loading/unloading helpers

        private void InitializeExplicitTargetMask()
        {
            SpellCastTargetFlags targetMask = Targets;

            // prepare target mask using effect target entries
            foreach (var effect in Effects)
            {
                if (effect.ExplicitTargetType != SpellExplicitTargetType.Explicit)
                    continue;

                targetMask |= CalculateExplicitTargetMask(effect.MainTargeting);
                targetMask |= CalculateExplicitTargetMask(effect.SecondaryTargeting);
            }

            ExplicitTargetMask = targetMask;
        }

        private SpellCastTargetFlags CalculateExplicitTargetMask(TargetingType targetingType)
        {
            if (targetingType == default)
                return 0;

            SpellCastTargetFlags targetMask = 0;
            switch (targetingType.ReferenceType)
            {
                case SpellTargetReferences.Source:
                    targetMask = SpellCastTargetFlags.SourceLocation;
                    break;
                case SpellTargetReferences.Dest:
                    targetMask = SpellCastTargetFlags.DestLocation;
                    break;
                case SpellTargetReferences.Target:
                    switch (targetingType.TargetEntities)
                    {
                        case SpellTargetEntities.GameEntity:
                            targetMask = SpellCastTargetFlags.GameEntity;
                            break;
                        case SpellTargetEntities.UnitAndDest:
                        case SpellTargetEntities.Unit:
                        case SpellTargetEntities.Dest:
                            switch (targetingType.SelectionCheckType)
                            {
                                case SpellTargetChecks.Enemy:
                                    targetMask = SpellCastTargetFlags.UnitEnemy;
                                    break;
                                case SpellTargetChecks.Ally:
                                    targetMask = SpellCastTargetFlags.UnitAlly;
                                    break;
                                default:
                                    targetMask = SpellCastTargetFlags.Unit;
                                    break;
                            }
                            break;
                    }
                    break;
            }

            return targetMask;
        }

        #endregion
    }
}