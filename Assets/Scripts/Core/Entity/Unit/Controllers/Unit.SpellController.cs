using Common;
using UnityEngine;

namespace Core
{
    public abstract partial class Unit
    {
        internal class SpellController : IUnitBehaviour
        {
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

            internal int DamageBySpell(SpellCastDamageInfo damageInfoInfo)
            {
                Unit victim = damageInfoInfo.Target;
                if (victim == null || !victim.IsAlive)
                    return 0;

                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, unit, victim, damageInfoInfo.Damage, damageInfoInfo.HitInfo == HitType.CriticalHit);

                return unit.DealDamage(victim, damageInfoInfo.Damage);
            }

            internal int HealBySpell(Unit target, SpellInfo spellInfo, int healAmount, bool critical = false)
            {
                return unit.DealHeal(target, healAmount);
            }

            internal int CalculateSpellDamageTaken(SpellCastDamageInfo damageInfoInfo, int damage, SpellInfo spellInfo)
            {
                if (damage < 0)
                    return 0;

                Unit victim = damageInfoInfo.Target;
                if (victim == null || !victim.IsAlive)
                    return 0;

                SpellSchoolMask damageSchoolMask = damageInfoInfo.SchoolMask;

                if (damage > 0)
                {
                    int absorb = damageInfoInfo.Absorb;
                    int resist = damageInfoInfo.Resist;
                    CalcAbsorbResist(victim, damageSchoolMask, SpellDamageType.Direct, damage, ref absorb, ref resist, spellInfo);
                    damageInfoInfo.Absorb = absorb;
                    damageInfoInfo.Resist = resist;
                    damage -= damageInfoInfo.Absorb + damageInfoInfo.Resist;
                }
                else
                    damage = 0;

                return damageInfoInfo.Damage = damage;
            }

            internal SpellMissType SpellHitResult(Unit victim, SpellInfo spellInfo, bool canReflect = false)
            {
                if (victim.IsImmuneToSpell(spellInfo, unit))
                    return SpellMissType.Immune;

                if (unit == victim)
                    return SpellMissType.None;

                // all positive spells can`t miss
                if (spellInfo.IsPositive() && !unit.IsHostileTo(victim))
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

            internal int SpellDamageBonusDone(Unit victim, SpellInfo spellInfo, float damage, SpellDamageType damageType, SpellEffectInfo effect, uint stack = 1) { return 0; }

            internal void ApplySpellModifier(SpellInfo spellInfo, SpellModifierType modifierType, ref int value) { }

            internal void ApplySpellModifier(SpellInfo spellInfo, SpellModifierType modifierType, ref float value) { }

            internal float SpellDamageBonusTaken(Unit caster, SpellInfo spellInfo, float damage, SpellDamageType damageType, SpellEffectInfo effect, uint stack = 1)
            {
                return damage;
            }

            internal uint SpellHealingBonusDone(Unit victim, SpellInfo spellInfo, uint healAmount, SpellDamageType damageType, SpellEffectInfo effect, uint stack = 1) { return 0; }

            internal uint SpellHealingBonusTaken(Unit caster, SpellInfo spellInfo, uint healAmount, SpellDamageType damageType, SpellEffectInfo effect, uint stack = 1) { return 0; }

            internal float SpellHealingPercentDone(Unit victim, SpellInfo spellInfo) { return 0.0f; }

            internal bool IsSpellCrit(Unit victim, SpellInfo spellInfo, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }

            internal int SpellCriticalHealingBonus(SpellInfo spellInfo, int damage, Unit victim) { return 0; }

            internal bool IsImmunedToDamage(SpellInfo spellInfo) { return false; }

            internal bool IsImmuneToSpell(SpellInfo spellInfo, Unit caster) { return false; }

            internal bool IsImmuneToAura(AuraInfo auraInfo, Unit caster) { return false; }

            internal bool IsImmuneToAuraEffect(AuraEffectInfo auraEffectInfo, Unit caster) { return false; }

            internal void CalcAbsorbResist(Unit victim, SpellSchoolMask schoolMask, SpellDamageType damageType, int damage, ref int absorb, ref int resist, SpellInfo spellInfo = null) { }

            internal void CalcHealAbsorb(Unit victim, SpellInfo spellInfo, ref int healAmount, ref int absorb) { }

            internal float ApplyEffectModifiers(SpellInfo spellInfo, float value) { return value; }

            internal int ModifyAuraDuration(AuraInfo auraInfo, Unit target, int duration) { return duration; }

            internal int ModifySpellCastTime(SpellInfo spellInfo, int castTime, Spell spell = null) { return castTime; }
        }
    }
}
