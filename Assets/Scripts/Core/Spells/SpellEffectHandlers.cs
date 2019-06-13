using System;
using UnityEngine;
using Common;

namespace Core
{
    public partial class Spell
    {
        public void EffectNone(SpellEffectInfo effect)
        {
            Debug.Log("Spells: Handled EffectNone!");
        }

        public void EffectUnused(SpellEffectInfo effect)
        {
            Debug.Log("Spells: Handled EffectUnused!");
        }

        public void EffectDistract(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            // Check for possible target
            if (target == null || target.IsInCombat)
                return;

            // target must be OK to do this
            if (target.HasUnitState(UnitState.Confused | UnitState.Stunned | UnitState.Fleeing))
                return;

            target.SetFacingTo(DestTarget);
            target.ClearUnitState(UnitState.Moving);
        }

        public void EffectPull(SpellEffectInfo effect)
        {
            EffectNone(effect);
        }

        public void EffectEnvironmentalDamage(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            if (target == null || !target.IsAlive)
                return;

            int absorb = 0;
            int resist = 0;
            Caster.CalcAbsorbResist(target, SpellInfo.SchoolMask, SpellDamageType.Pure, 100, ref absorb, ref resist, SpellInfo);
        }

        public void EffectInstaKill(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            if (target == null || !target.IsAlive)
                return;

            if (Caster == target)
                Finish();

            Caster.DealDamage(target, target.Health);
        }

        public void EffectDummy(int effIndex) { throw new NotImplementedException(); }

        public void EffectTeleportUnits(int effIndex) { throw new NotImplementedException(); }

        public void EffectApplyAura(EffectTeleportDirect effect, Unit target, SpellEffectHandleMode mode)
        {
            if (mode != SpellEffectHandleMode.HitTarget)
                return;

            if (SpellAura == null || target == null)
                return;

            Assert.IsTrue(target == SpellAura.Owner);
            SpellAura.ApplyEffectForTargets(effect);
        }

        public void EffectDispel(int effIndex) { throw new NotImplementedException(); }

        public void EffectTriggerSpell(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealMaxHealth(int effIndex) { throw new NotImplementedException(); }

        public void EffectInterruptCast(int effIndex) { throw new NotImplementedException(); }

        public void EffectResurrect(int effIndex) { throw new NotImplementedException(); }

        public void EffectKnockBack(int effIndex) { throw new NotImplementedException(); }

        public void EffectPullTowards(int effIndex) { throw new NotImplementedException(); }

        public void EffectDispelMechanic(int effIndex) { throw new NotImplementedException(); }

        public void EffectHealPercent(int effIndex) { throw new NotImplementedException(); }
    }
}
