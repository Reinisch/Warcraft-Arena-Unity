using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public abstract class UnitAI
    {
        public static AISpellInfoType AISpellInfo;
        protected Unit Me { get; set; }

        protected UnitAI(Unit unit)
        {
            Me = unit;
        }


        public virtual bool CanAIAttack(Unit target)
        {
            return true;
        }

        public virtual void AttackStart(Unit target)
        {
        }

        public abstract void UpdateAI(uint diff);


        public virtual void InitializeAI()
        {
            if (!Me.IsDead)
                Reset();
        }

        public virtual void DeinitializeAI()
        {
        }

        public virtual void Reset()
        {
        }


        // Called when unit is charmed
        public abstract void OnCharmed(bool apply);


        // Pass parameters between AI
        public virtual void DoAction(int param)
        {
        }

        public virtual uint GetData(uint id = 0)
        {
            return 0;
        }

        public virtual void SetData(uint id, uint value)
        {
        }

        public virtual void SetGUID(Guid guid, int id = 0)
        {
        }

        public virtual Guid GetGUID(int id = 0)
        {
            return Guid.Empty;
        }


        public Unit SelectTarget(SelectAggroTarget targetType, uint position = 0, float dist = 0.0f, bool playerOnly = false,
            int aura = 0)
        {
            return null;
        }

        public Unit SelectTarget(SelectAggroTarget targetType, uint position, Predicate<Unit> predicate)
        {
            var threatlist = Me.ThreatManager.ThreatList;
            if (position >= threatlist.Count)
                return null;

            var targetList =
                threatlist.FindAll(hostileRef => predicate(hostileRef.Target)).Select(hostile => hostile.Target).ToList();

            if (position >= targetList.Count)
                return null;

            if (targetType == SelectAggroTarget.Nearest || targetType == SelectAggroTarget.Farthest)
                targetList.Sort((x, y) => Me.GetDistanceOrder(x, y) ? 1 : -1);

            switch (targetType)
            {
                case SelectAggroTarget.Nearest:
                case SelectAggroTarget.TopAggro:
                    return targetList[(int) position];
                case SelectAggroTarget.Farthest:
                case SelectAggroTarget.BottomAggro:
                    return targetList[targetList.Count - (int) position];
                case SelectAggroTarget.Random:
                    return targetList[RandomHelper.Next((int) position, targetList.Count)];
            }

            return null;
        }

        public void SelectTargetList(List<Unit> targetList, uint num, SelectAggroTarget targetType, float dist = 0.0f,
            bool playerOnly = false, int aura = 0)
        {
        }

        public void SelectTargetList(List<Unit> targetList, Predicate<Unit> predicate, uint maxTargets,
            SelectAggroTarget targetType)
        {
            var threatlist = Me.ThreatManager.ThreatList;
            if (threatlist.Count == 0)
                return;

            targetList.AddRange(
                threatlist.FindAll(hostileRef => predicate(hostileRef.Target)).Select(hostile => hostile.Target));

            if (targetList.Count < maxTargets)
                return;

            if (targetType == SelectAggroTarget.Nearest || targetType == SelectAggroTarget.Farthest)
                targetList.Sort((x, y) => Me.GetDistanceOrder(x, y) ? 1 : -1);
            else if (targetType == SelectAggroTarget.Farthest || targetType == SelectAggroTarget.BottomAggro)
                targetList.Sort((x, y) => Me.GetDistanceOrder(x, y) ? -1 : 1);
            else if (targetType == SelectAggroTarget.Random)
                targetList.Sort((x, y) => Math.Sign(RandomHelper.NextDouble() - 0.5d));

            targetList.RemoveRange(0, targetList.Count - (int) maxTargets);
        }


        // Called at any Damage to any victim (before damage apply)
        public virtual void DamageDealt(Unit victim, ref uint damage, DamageEffectType damageType)
        {
        }

        // Called at any Damage from any attacker (before damage apply)
        // Note: it for recalculation damage or special reaction at damage
        // for attack reaction use AttackedBy called for not DOT damage in Unit::DealDamage also
        public virtual void DamageTaken(Unit attacker, ref uint damage)
        {
        }

        // Called when the creature receives heal
        public virtual void HealReceived(Unit doneBy, ref uint addhealth)
        {
        }

        // Called when the unit heals
        public virtual void HealDone(Unit doneTo, ref uint addhealth)
        {
        }

        // Called when a spell is interrupted by Spell::EffectInterruptCast
        // Use to reschedule next planned cast of spell.
        public virtual void SpellInterrupted(uint spellId, uint unTimeMs)
        {
        }


        public void AttackStartCaster(Unit victim, float dist)
        {
        }


        public void DoCast(uint spellId)
        {
        }

        public void DoCast(Unit victim, uint spellId, bool triggered = false)
        {
        }

        public void DoCastVictim(uint spellId, bool triggered = false)
        {
        }

        public void DoCastAOE(uint spellId, bool triggered = false)
        {
        }


        public void DoMeleeAttackIfReady()
        {
        }

        public bool DoSpellAttackIfReady(uint spellId)
        {
            return false;
        }

        public static void FillAISpellInfo()
        {
        }
    }
}