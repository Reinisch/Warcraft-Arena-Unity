using UnityEngine;

namespace Core
{
    public enum SelectAggroTarget
    {
        Random = 0, // just selects a random target
        TopAggro, // selects targes from top aggro to bottom
        BottomAggro, // selects targets from bottom aggro to top
        Nearest,
        Farthest
    }

    /// <summary>
    /// Default predicate function to select target based on distance, player and/or aura criteria.
    /// </summary>
    public class DefaultTargetSelector
    {
        public Unit Me { get; }
        public float Dist { get; }
        public bool PlayerOnly { get; }
        public int Aura { get; }

        /// <param name="unit">The reference unit.</param>
        /// <param name="dist">
        ///     If 0: ignored, if more then 0: maximum distance to the reference unit, if less then 0: minimum
        ///     distance to the reference unit.
        /// </param>
        /// <param name="playerOnly">Only for players.</param>
        /// <param name="aura">
        ///     If 0: ignored, if more then 0: the target shall have the aura, if less then 0, the target shall NOT
        ///     have the aura.
        /// </param>
        public DefaultTargetSelector(Unit unit, float dist, bool playerOnly, int aura)
        {
            Me = unit;
            Dist = dist;
            PlayerOnly = playerOnly;
            Aura = aura;
        }

        public bool TargetSelector(Unit target)
        {
            if (Me == null)
                return false;

            if (target == null)
                return false;

            if (PlayerOnly && target.TypeId != EntityType.Player)
                return false;

            if (Dist > 0.0f && !Me.IsWithinCombatRange(target, Dist))
                return false;

            if (Dist < 0.0f && Me.IsWithinCombatRange(target, -Dist))
                return false;

            if (Aura == 0)
                return true;

            if (Aura > 0)
            {
                if (!target.HasAura((uint) Aura))
                    return false;
            }
            else
            {
                if (target.HasAura((uint) -Aura))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Target selector for spell casts checking range, auras and attributes.
    /// </summary>
    public class SpellTargetSelector
    {
        public Unit Caster { get; }
        public SpellInfo SpellInfo { get; }

        public SpellTargetSelector(Unit caster, int spellId)
        {
            Caster = caster;
            SpellInfo = BalanceManager.SpellInfos[spellId];
        }

        public bool TargetSelector(Unit target)
        {
            if (target == null)
                return false;

            if (SpellInfo.CheckTarget(Caster, target) != SpellCastResult.Success)
                return false;

            var minRange = 0.0f;
            var maxRange = 0.0f;
            var rangeMod = 0.0f;
            if ((SpellInfo.RangedFlags & SpellRangeFlag.Melee) > 0)
            {
                rangeMod = Caster.GetCombatReach() + 4.0f / 3.0f;
                rangeMod += target.GetCombatReach();
                rangeMod = Mathf.Max(rangeMod, UnitHelper.NominalMeleeRange);
            }
            else
            {
                var meleeRange = 0.0f;
                if ((SpellInfo.RangedFlags & SpellRangeFlag.Ranged) > 0)
                {
                    meleeRange = Caster.GetCombatReach() + 4.0f / 3.0f;
                    meleeRange += target.GetCombatReach();
                    meleeRange = Mathf.Max(meleeRange, UnitHelper.NominalMeleeRange);
                }

                minRange = Caster.GetSpellMinRangeForTarget(target, SpellInfo) + meleeRange;
                maxRange = Caster.GetSpellMaxRangeForTarget(target, SpellInfo);

                rangeMod = Caster.GetCombatReach();
                rangeMod += target.GetCombatReach();

                if (minRange > 0.0f && (SpellInfo.RangedFlags & SpellRangeFlag.Ranged) == 0)
                    minRange += rangeMod;
            }

            if (Caster.IsMoving() && target.IsMoving() && !Caster.IsWalking() && !target.IsWalking() &&
                ((SpellInfo.RangedFlags & SpellRangeFlag.Melee) > 0 || target.TypeId == EntityType.Player))
                rangeMod += 5.0f / 3.0f;

            maxRange += rangeMod;

            minRange *= minRange;
            maxRange *= maxRange;

            if (target != Caster)
            {
                if (Caster.GetExactDistSq(target.WorldLocation) > maxRange)
                    return false;

                if (minRange > 0.0f && Caster.GetExactDistSq(target.WorldLocation) < minRange)
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Very simple target selector, will just skip main target.
    /// </summary>
    public class NonTankTargetSelector
    {
        public Unit Source { get; }
        public bool PlayerOnly { get; }

        public NonTankTargetSelector(Unit source, bool playerOnly = true)
        {
            Source = source;
            PlayerOnly = playerOnly;
        }

        public bool TargetSelector(Unit target)
        {
            if (target == null)
                return false;

            if (PlayerOnly && target.TypeId != EntityType.Player)
                return false;

            var currentVictim = Source.ThreatManager.CurrentVictim;
            if (currentVictim != null)
                return target.Guid != currentVictim.UnitGuid;

            return target != Source.GetVictim();
        }
    }
}