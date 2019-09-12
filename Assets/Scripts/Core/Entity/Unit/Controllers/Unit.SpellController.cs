using System.Collections.Generic;
using Common;
using UnityEngine;

using SpellModifierContainer = System.Collections.Generic.Dictionary<(Core.SpellModifierType, Core.SpellModifierApplicationType), System.Collections.Generic.List<Core.SpellModifier>>;
using SchoolImmunityContainer = System.Collections.Generic.Dictionary<Core.SpellSchoolMask, System.Collections.Generic.List<Core.SpellInfo>>;

namespace Core
{
    public abstract partial class Unit
    {
        internal class SpellController : IUnitBehaviour
        {
            private readonly SpellModifierContainer spellModifiers = new SpellModifierContainer();
            private readonly SchoolImmunityContainer schoolImmunities = new SchoolImmunityContainer();

            private Unit unit;

            public SpellCast Cast { get; private set; }
            public SpellHistory SpellHistory { get; private set; }

            public bool HasClientLogic => true;
            public bool HasServerLogic => true;

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                SpellHistory.DoUpdate(deltaTime);
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;

                SpellHistory = new SpellHistory(unit, unit.entityState);
                Cast = new SpellCast(unit, unit.entityState);
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                schoolImmunities.Clear();
                spellModifiers.Clear();
                SpellHistory.Detached();
                Cast.Detached();

                unit = null;
            }

            internal SpellCastResult CastSpell(SpellInfo spellInfo, SpellCastingOptions castOptions)
            {
                Spell spell = new Spell(unit, spellInfo, castOptions);

                SpellCastResult castResult = spell.Prepare();
                if (castResult != SpellCastResult.Success)
                {
                    unit.World.SpellManager.Remove(spell);
                    return castResult;
                }

                switch (spell.ExecutionState)
                {
                    case SpellExecutionState.Casting:
                        unit.SpellCast.HandleSpellCast(spell, SpellCast.HandleMode.Started);
                        break;
                    case SpellExecutionState.Processing:
                        return castResult;
                    case SpellExecutionState.Completed:
                        return castResult;
                }

                return SpellCastResult.Success;
            }

            internal void DamageBySpell(SpellDamageInfo damageInfo)
            {
                unit.Spells.CalculateSpellDamageTaken(ref damageInfo);

                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerDamageDone, damageInfo);

                unit.DealDamage(damageInfo.Target, (int)damageInfo.Damage, damageInfo.SpellDamageType);
            }

            internal void HealBySpell(SpellHealInfo healInfo)
            {
                unit.Spells.CalculateSpellHealingTaken(ref healInfo);

                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerHealingDone, healInfo);

                unit.DealHeal(healInfo.Target, (int)healInfo.Heal);
            }

            internal void CalculateSpellDamageTaken(ref SpellDamageInfo damageInfo)
            {
                if (damageInfo.Damage == 0 || !damageInfo.Target.IsAlive)
                    return;

                Unit caster = damageInfo.Caster;
                Unit target = damageInfo.Target;
                SpellInfo spellInfo = damageInfo.SpellInfo;

                damageInfo.UpdateDamage(caster.Spells.SpellDamageBonusDone(target, spellInfo, damageInfo.Damage, damageInfo.SpellDamageType));
                damageInfo.UpdateDamage(target.Spells.SpellDamageBonusTaken(caster, spellInfo, damageInfo.Damage, damageInfo.SpellDamageType));

                if (!spellInfo.HasAttribute(SpellExtraAttributes.FixedDamage) && damageInfo.HasCrit)
                {
                    uint criticalDamage = CalculateSpellCriticalDamage(spellInfo, damageInfo.Damage);
                    damageInfo.UpdateOriginalDamage(criticalDamage);
                }

                HandleAbsorb(ref damageInfo);
            }

            internal void CalculateSpellHealingTaken(ref SpellHealInfo healInfo)
            {
                if (healInfo.Heal == 0 || !healInfo.Target.IsAlive)
                    return;

                Unit healer = healInfo.Healer;
                Unit target = healInfo.Target;
                SpellInfo spellInfo = healInfo.SpellInfo;

                healInfo.UpdateBase(healer.Spells.SpellHealingBonusDone(target, spellInfo, healInfo.Heal));
                healInfo.UpdateBase(target.Spells.SpellHealingBonusTaken(healer, spellInfo, healInfo.Heal));

                if (healInfo.HasCrit)
                {
                    uint criticalHeal = CalculateSpellCriticalHealing(healInfo.Heal);
                    healInfo.UpdateBase(criticalHeal);
                }

                HandleAbsorb(ref healInfo);
            }

            internal SpellMissType SpellHitResult(Unit victim, SpellInfo spellInfo, bool canReflect = false)
            {
                if (unit == victim)
                    return SpellMissType.None;

                // all positive spells can`t miss
                if (spellInfo.IsPositive && !unit.IsHostileTo(victim))
                    return SpellMissType.None;

                return SpellMissType.None;
            }

            internal float GetSpellMinRangeForTarget(Unit target, SpellInfo spellInfo)
            {
                if (Mathf.Approximately(spellInfo.MinRangeFriend, spellInfo.MinRangeHostile))
                    return spellInfo.GetMinRange(false);
                if (target == null)
                    return spellInfo.GetMinRange(true);
                return spellInfo.GetMinRange(!unit.IsHostileTo(target));
            }

            internal float GetSpellMaxRangeForTarget(Unit target, SpellInfo spellInfo)
            {
                if (Mathf.Approximately(spellInfo.MaxRangeFriend, spellInfo.MaxRangeHostile))
                    return spellInfo.GetMaxRange(false);
                if (target == null)
                    return spellInfo.GetMaxRange(true);
                return spellInfo.GetMaxRange(!unit.IsHostileTo(target));
            }

            internal Unit GetMagicHitRedirectTarget(Unit victim, SpellInfo spellInfo) { return null; }

            internal Unit GetMeleeHitRedirectTarget(Unit victim, SpellInfo spellInfo = null) { return null; }
            
            internal uint SpellDamageBonusDone(Unit victim, SpellInfo spellInfo, uint damage, SpellDamageType damageType, uint stack = 1)
            {
                return damage;
            }

            internal uint SpellDamageBonusTaken(Unit caster, SpellInfo spellInfo, uint damage, SpellDamageType damageType, uint stack = 1)
            {
                return damage;
            }

            internal uint SpellHealingBonusDone(Unit target, SpellInfo spellInfo, uint healAmount)
            {
                return healAmount;
            }

            internal uint SpellHealingBonusTaken(Unit caster, SpellInfo spellInfo, uint healAmount)
            {
                return healAmount;
            }

            internal bool IsSpellCrit(Unit victim, SpellInfo spellInfo, SpellSchoolMask schoolMask)
            {
                float critChance = CalculateSpellCriticalChance(victim, spellInfo, schoolMask);
                return RandomUtils.CheckSuccessPercent(critChance);
            }

            private float CalculateSpellCriticalChance(Unit victim, SpellInfo spellInfo, SpellSchoolMask schoolMask)
            {
                if (spellInfo.HasAttribute(SpellAttributes.CantCrit) || spellInfo.DamageClass == SpellDamageClass.None)
                    return 0.0f;

                if (spellInfo.HasAttribute(SpellAttributes.AlwaysCrit))
                    return 100.0f;

                float critChance = unit.CritPercentage;
                if (victim == null)
                    return Mathf.Max(critChance, 0.0f);

                switch (spellInfo.DamageClass)
                {
                    case SpellDamageClass.Magic:
                        if (!spellInfo.IsPositive)
                            critChance += victim.Auras.TotalAuraModifier(AuraEffectType.ModAttackerSpellCritChance);
                        goto default;
                    case SpellDamageClass.Melee:
                        if (!spellInfo.IsPositive)
                            critChance += victim.Auras.TotalAuraModifier(AuraEffectType.ModAttackerMeleeCritChance);
                        goto default;
                    case SpellDamageClass.Ranged:
                        if (!spellInfo.IsPositive)
                            critChance += victim.Auras.TotalAuraModifier(AuraEffectType.ModAttackerRangedCritChance);
                        goto default;
                    default:
                        critChance += victim.Auras.TotalAuraModifierForCaster(AuraEffectType.ModAttackerSpellCritChanceForCaster, unit.Id);
                        critChance += victim.Auras.TotalAuraModifier(AuraEffectType.ModAttackerSpellAndWeaponCritChance);
                        break;
                }

                return Mathf.Max(critChance, 0.0f);
            }

            internal uint CalculateSpellCriticalHealing(uint healing)
            {
                return (uint)(2 * healing * unit.TotalAuraMultiplier(AuraEffectType.ModCriticalHealingAmount));
            }

            internal uint CalculateSpellCriticalDamage(SpellInfo spellInfo, uint damage)
            {
                uint critBonus = 0;
                float critModifier = 0.0f;

                switch (spellInfo.DamageClass)
                {
                    case SpellDamageClass.Melee:
                    case SpellDamageClass.Ranged:
                        critBonus = damage / 2;
                        break;
                    case SpellDamageClass.Magic:
                    case SpellDamageClass.None:
                        critBonus = damage;
                        break;
                }

                critModifier += (unit.TotalAuraMultiplier(AuraEffectType.ModifyCritDamageBonus) - 1.0f) * 100.0f;
                critModifier = Mathf.Clamp(critModifier, -100.0f, float.MaxValue);

                if (!Mathf.Approximately(critModifier, 0.0f))
                    critBonus = critBonus.AddPercentage(critModifier);

                return System.Math.Max(0, damage + critBonus);
            }

            internal void HandleAbsorb(ref SpellDamageInfo damageInfo)
            {
                if (damageInfo.Target.IsDead || damageInfo.Damage == 0)
                    return;

                Unit target = damageInfo.Target;
                IReadOnlyList<AuraEffect> absorbEffects = target.GetAuraEffects(AuraEffectType.AbsorbDamage);
                if (absorbEffects == null)
                    return;

                var absorbEffectCopies = new List<AuraEffect>(absorbEffects);
                for (int index = 0; index < absorbEffectCopies.Count; index++)
                {
                    AuraEffect absorbEffect = absorbEffectCopies[index];
                    if (!absorbEffect.Aura.ApplicationsByTargetId.ContainsKey(target.Id))
                        continue;
                    if (absorbEffect.Value <= 0.0f)
                        continue;

                    uint availableAbsorb = (uint)Mathf.CeilToInt(absorbEffect.Value);
                    uint effectiveAbsorb = System.Math.Min(availableAbsorb, damageInfo.Damage);
                    damageInfo.AbsorbDamage(effectiveAbsorb);
                    absorbEffect.ModifyValue(-effectiveAbsorb);

                    if (absorbEffect.Value <= 0.0f)
                        absorbEffect.Aura.Remove(AuraRemoveMode.Spell);
                }
            }

            internal void HandleAbsorb(ref SpellHealInfo healInfo)
            {
                if (healInfo.Target.IsDead || healInfo.Heal == 0)
                    return;

                Unit target = healInfo.Target;
                IReadOnlyList<AuraEffect> absorbEffects = target.GetAuraEffects(AuraEffectType.AbsorbHeal);
                if (absorbEffects == null)
                    return;

                var absorbEffectCopies = new List<AuraEffect>(absorbEffects);
                for (int index = 0; index < absorbEffectCopies.Count; index++)
                {
                    AuraEffect absorbEffect = absorbEffectCopies[index];
                    if (!absorbEffect.Aura.ApplicationsByTargetId.ContainsKey(target.Id))
                        continue;
                    if (absorbEffect.Value <= 0.0f)
                        continue;

                    uint availableAbsorb = (uint)Mathf.CeilToInt(absorbEffect.Value);
                    uint effectiveAbsorb = System.Math.Min(availableAbsorb, healInfo.Heal);
                    healInfo.AbsorbHeal(effectiveAbsorb);
                    absorbEffect.ModifyValue(-effectiveAbsorb);

                    if (absorbEffect.Value <= 0.0f)
                        absorbEffect.Aura.Remove(AuraRemoveMode.Spell);
                }
            }

            internal void HandleSpellModifier(SpellModifier modifier, bool apply)
            {
                spellModifiers.HandleEntry(modifier.Kind, modifier, apply);
            }

            internal bool IsImmunedToDamage(SpellInfo spellInfo) { return false; }

            internal bool IsImmunedToDamage(AuraInfo auraInfo) { return false; }

            internal bool IsImmuneToSpell(SpellInfo spellInfo, Unit caster)
            {
                if (spellInfo.HasAttribute(SpellAttributes.UnaffectedByInvulnerability))
                    return false;

                if (spellInfo.SchoolMask != 0)
                {
                    SpellSchoolMask immunedSchools = 0;
                    foreach (var schoolImmunityEntry in schoolImmunities)
                    {
                        if (!schoolImmunityEntry.Key.HasAnyFlag(spellInfo.SchoolMask))
                            continue;

                        foreach (SpellInfo immunitySpells in schoolImmunityEntry.Value)
                            if (!immunitySpells.IsPositive || !spellInfo.IsPositive || !unit.IsFriendlyTo(caster))
                                if (!spellInfo.CanPierceImmuneAura(immunitySpells))
                                    immunedSchools |= schoolImmunityEntry.Key;
                    }

                    if (immunedSchools.HasTargetFlag(spellInfo.SchoolMask))
                        return true;
                }

                return false;
            }

            internal bool IsImmuneToAura(AuraInfo auraInfo, Unit caster) { return false; }

            internal bool IsImmuneToAuraEffect(AuraEffectInfo auraEffectInfo, Unit caster) { return false; }

            internal bool IsAffectedBySpellModifier(Spell spell, SpellModifier modifier)
            {
                if (modifier.Aura.AuraInfo.UsesCharges && modifier.Aura.Charges == 0 && !spell.HasAppliedModifier(modifier.Aura))
                    return false;

                return !spell.SpellInfo.HasAttribute(SpellAttributes.IgnoreSpellModifiers);
            }

            internal float ApplyEffectModifiers(SpellInfo spellInfo, float value) { return value; }

            internal float ApplySpellModifier(Spell spell, SpellModifierType modifierType, float baseValue)
            {
                float valueMultiplier = 1.0f;
                float valueModifier = 0.0f;

                if (spellModifiers.TryGetValue((modifierType, SpellModifierApplicationType.Flat), out List<SpellModifier> flatModifiers))
                {
                    foreach (SpellModifier modifier in flatModifiers)
                    {
                        if (!IsAffectedBySpellModifier(spell, modifier))
                            continue;

                        valueModifier += modifier.Value;
                        spell.AddAppliedModifierAura(modifier.Aura);
                    }
                }

                float flatValue = baseValue + valueModifier;
                if (Mathf.Approximately(flatValue, 0.0f))
                    return 0.0f;

                if (spellModifiers.TryGetValue((modifierType, SpellModifierApplicationType.Percent), out List<SpellModifier> percentModifiers))
                {
                    foreach (SpellModifier modifier in percentModifiers)
                    {
                        if (!IsAffectedBySpellModifier(spell, modifier))
                            continue;

                        valueMultiplier *= 1.0f + 1.0f.ApplyPercentage(modifier.Value);
                        spell.AddAppliedModifierAura(modifier.Aura);

                        if (Mathf.Approximately(valueMultiplier, 0.0f))
                            return 0.0f;
                    }
                }

                return flatValue * valueMultiplier;
            }

            internal int ModifyAuraDuration(AuraInfo auraInfo, Unit target, int duration) { return duration; }

            internal int ModifySpellCastTime(Spell spell, int castTime)
            {
                int resultCastTime = castTime;
                if (resultCastTime <= 0)
                    return 0;

                resultCastTime = Mathf.RoundToInt(castTime * unit.Attributes.ModHaste.Value);

                if (resultCastTime < spell.SpellInfo.MinCastTime)
                    resultCastTime = spell.SpellInfo.MinCastTime;

                if (resultCastTime < 0)
                    resultCastTime = 0;

                resultCastTime = (int)spell.Caster.Spells.ApplySpellModifier(spell, SpellModifierType.InstantCast, resultCastTime);

                return resultCastTime;
            }

            internal void ModifySchoolImmunity(SpellInfo spellInfo, SpellSchoolMask schoolMask, bool apply)
            {
                schoolImmunities.HandleEntry(schoolMask, spellInfo, apply);
            }
        }
    }
}
