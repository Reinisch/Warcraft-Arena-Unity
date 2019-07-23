using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Core
{
    public abstract partial class Unit : WorldEntity
    {
        public new class CreateToken : WorldEntity.CreateToken
        {
            public DeathState DeathState { private get; set; }
            public bool FreeForAll { private get; set; }
            public int FactionId { private get; set; }

            public override void Read(UdpPacket packet)
            {
                base.Read(packet);

                DeathState = (DeathState)packet.ReadInt();
                FactionId = packet.ReadInt();
                FreeForAll = packet.ReadBool();
            }

            public override void Write(UdpPacket packet)
            {
                base.Write(packet);

                packet.WriteInt((int)DeathState);
                packet.WriteInt(FactionId);
                packet.WriteBool(FreeForAll);
            }

            protected void Attached(Unit unit)
            {
                unit.DeathState = DeathState;
                unit.Faction = unit.Balance.FactionsById[FactionId];
                unit.FreeForAll = FreeForAll;
            }
        }

        [SerializeField, UsedImplicitly, Header(nameof(Unit)), Space(10)]
        private CapsuleCollider unitCollider;
        [SerializeField, UsedImplicitly]
        private WarcraftCharacterController characterController;
        [SerializeField, UsedImplicitly]
        private UnitAttributeDefinition unitAttributeDefinition;
        [SerializeField, UsedImplicitly]
        private UnitMovementDefinition unitMovementDefinition;
        [SerializeField, UsedImplicitly]
        private List<UnitBehaviour> unitBehaviours;

        private CreateToken createToken;
        private UnitFlags unitFlags;
        private UnitState unitState;

        private readonly BehaviourController behaviourController = new BehaviourController();

        internal AuraApplicationController ApplicationAuraController { get; } = new AuraApplicationController();
        internal AuraVisibleController VisibleAuraController { get; } = new AuraVisibleController();
        internal AttributeController AttributeUnitController { get; } = new AttributeController();
        internal ThreatController ThreatUnitController { get; } = new ThreatController();
        internal WarcraftCharacterController CharacterController => characterController;

        internal bool NeedUpdateVisibleAuras { set => VisibleAuraController.NeedUpdateVisibleAuras = value; }
        internal FactionDefinition Faction { get => AttributeUnitController.Faction; set => AttributeUnitController.Faction = value; }
        internal DeathState DeathState { get => AttributeUnitController.DeathState; set => AttributeUnitController.DeathState = value; }
        internal bool FreeForAll { get => AttributeUnitController.FreeForAll; set => AttributeUnitController.FreeForAll = value; }

        internal IReadOnlyDictionary<UnitMoveType, float> SpeedRates => AttributeUnitController.SpeedRates;
        internal IReadOnlyList<AuraApplication> AuraApplications => ApplicationAuraController.AuraApplications;
        internal EntityAttributeInt HealthAttribute => AttributeUnitController.Health;
        internal EntityAttributeInt MaxHealthAttribute => AttributeUnitController.MaxHealth;
        internal EntityAttributeInt ManaAttribute => AttributeUnitController.Mana;
        internal EntityAttributeInt MaxManaAttribute => AttributeUnitController.MaxMana;
        internal EntityAttributeInt LevelAttribute => AttributeUnitController.Level;
        internal EntityAttributeInt SpellPowerAttribute => AttributeUnitController.SpellPower;
        internal EntityAttributeFloat ModHasteAttribute => AttributeUnitController.ModHaste;
        internal EntityAttributeFloat ModRegenHasteAttribute => AttributeUnitController.ModRegenHaste;
        internal EntityAttributeFloat CritPercentageAttribute => AttributeUnitController.CritPercentage;

        public SpellCast SpellCast { get; private set; }
        public IUnitState EntityState { get; private set; }
        public SpellHistory SpellHistory { get; private set; }

        public Unit Target => AttributeUnitController.Target;
        public CapsuleCollider UnitCollider => unitCollider;
        public PlayerControllerDefinition ControllerDefinition => characterController.ControllerDefinition;

        public int Level => LevelAttribute.Value;
        public int Health => HealthAttribute.Value;
        public int MaxHealth => MaxHealthAttribute.Value;
        public int BaseMana => ManaAttribute.Base;
        public int Mana => ManaAttribute.Value;
        public int MaxMana => MaxManaAttribute.Value;
        public int SpellPower => SpellPowerAttribute.Value;
        public float ModHaste => ModHasteAttribute.Value;
        public float ModRegenHaste => ModRegenHasteAttribute.Value;
        public float CritPercentage => CritPercentageAttribute.Value;
        public float HealthRatio => MaxHealth > 0 ? (float)Health / MaxHealth : 0.0f;
        public bool IsMovementBlocked => HasState(UnitState.Root) || HasState(UnitState.Stunned);
        public bool IsAlive => DeathState == DeathState.Alive;
        public bool IsDead => DeathState == DeathState.Dead;
        public bool IsControlledByPlayer => this is Player;
        public bool IsStopped => !HasState(UnitState.Moving);

        public bool HealthBelowPercent(int percent) => Health < MaxHealth.CalculatePercentage(percent);
        public bool HealthAbovePercent(int percent) => Health > MaxHealth.CalculatePercentage(percent);
        public bool HealthAbovePercentHealed(int percent, int healAmount) => Health + healAmount > MaxHealth.CalculatePercentage(percent);
        public bool HealthBelowPercentDamaged(int percent, int damageAmount) => Health - damageAmount < MaxHealth.CalculatePercentage(percent);
        public float GetSpeed(UnitMoveType type) => SpeedRates[type] * unitMovementDefinition.BaseSpeedByType(type);
        public float GetPowerPercent(SpellResourceType type) => GetMaxPower(type) > 0 ? 100.0f * GetPower(type) / GetMaxPower(type) : 0.0f;
        public int GetPower(SpellResourceType type) => Mana;
        public int GetMaxPower(SpellResourceType type) => MaxMana;

        public sealed override void Attached()
        {
            base.Attached();

            HandleAttach();
            
            behaviourController.HandleUnitAttach(this);

            World.UnitManager.Attach(this);
        }

        public sealed override void Detached()
        {
            // called twice on client (from Detached Photon callback and manual in UnitManager.Dispose)
            // if he needs to instantly destroy current world and avoid any events
            if (IsValid)
            {
                World.UnitManager.Detach(this);

                behaviourController.HandleUnitDetach();

                HandleDetach();

                base.Detached();
            }
        }

        public bool IsHostileTo(Unit unit)
        {
            if (unit == this)
                return false;

            if (unit.FreeForAll && FreeForAll)
                return true;

            return Faction.HostileFactions.Contains(unit.Faction);
        }

        public bool IsFriendlyTo(Unit unit)
        {
            if (unit == this)
                return true;

            if (unit.FreeForAll && FreeForAll)
                return false;

            return Faction.FriendlyFactions.Contains(unit.Faction);
        }

        public T FindBehaviour<T>() where T : UnitBehaviour => behaviourController.FindBehaviour<T>();

        protected virtual void HandleAttach()
        {
            AttributeUnitController.InitializeAttributes(this);

            createToken = (CreateToken) entity.AttachToken;
            EntityState = entity.GetState<IUnitState>();

            MovementInfo.Attached(EntityState, this);

            SpellHistory = new SpellHistory(this);
            SpellCast = new SpellCast(this);

            SetMap(World.FindMap(1));
        }

        protected virtual void HandleDetach()
        {
            SpellHistory.Detached();
            SpellCast.Detached();

            EntityState.RemoveAllCallbacks();

            ResetMap();

            MovementInfo.Detached();

            createToken = null;
        }

        internal override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            SpellHistory.DoUpdate(deltaTime);

            behaviourController.DoUpdate(deltaTime);
        }

        internal bool HasAuraType(AuraEffectType auraEffectType) => ApplicationAuraController.HasAuraType(auraEffectType);

        internal float TotalAuraModifier(AuraEffectType auraType) => ApplicationAuraController.TotalAuraModifier(auraType);

        internal float TotalAuraMultiplier(AuraEffectType auraType) => ApplicationAuraController.TotalAuraMultiplier(auraType);

        internal float MaxPositiveAuraModifier(AuraEffectType auraType) => ApplicationAuraController.MaxPositiveAuraModifier(auraType);

        internal float MaxNegativeAuraModifier(AuraEffectType auraType) => ApplicationAuraController.MaxNegativeAuraModifier(auraType);

        internal void AddState(UnitState state) { unitState |= state; }

        internal bool HasState(UnitState state) { return (unitState & state) != 0; }

        internal void RemoveState(UnitState state) { unitState &= ~state; }

        internal void SetFlag(UnitFlags flag) => unitFlags |= flag;

        internal void RemoveFlag(UnitFlags flag) => unitFlags &= ~flag;

        internal bool HasFlag(UnitFlags flag) => (unitFlags & flag) == flag;

        internal void ModifyDeathState(DeathState newState)
        {
            DeathState = newState;

            if (IsDead && SpellCast.IsCasting)
                SpellCast.Cancel();
        }

        internal int ModifyHealth(int delta)
        {
            return SetHealth(Health + delta);
        }

        internal int SetHealth(int value)
        {
            int delta = HealthAttribute.Set(Mathf.Clamp(value, 0, MaxHealth));
            EntityState.Health = Health;
            return delta;
        }

        internal int DealDamage(Unit target, int damageAmount)
        {
            if (damageAmount < 1)
                return 0;

            int healthValue = target.Health;
            if (healthValue <= damageAmount)
            {
                Kill(target);
                return healthValue;
            }

            return target.ModifyHealth(-damageAmount);
        }

        internal int DealHeal(Unit target, int healAmount)
        {
            if (healAmount < 1)
                return 0;

            return target.ModifyHealth(healAmount);
        }

        internal void Kill(Unit victim)
        {
            if (victim.Health <= 0)
                return;

            victim.SetHealth(0);
            victim.ModifyDeathState(DeathState.Dead);
        }

        #region Spell Handling

        internal SpellCastResult CastSpell(SpellInfo spellInfo, SpellCastingOptions castOptions)
        {
            Spell spell = new Spell(this, spellInfo, castOptions);

            SpellCastResult castResult = spell.Prepare();
            if (castResult != SpellCastResult.Success)
            {
                World.SpellManager.Remove(spell);
                return castResult;
            }

            switch (spell.ExecutionState)
            {
                case SpellExecutionState.Casting:
                    SpellCast.HandleSpellCast(spell, SpellCast.HandleMode.Started);
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

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, this, victim, damageInfoInfo.Damage, damageInfoInfo.HitInfo == HitType.CriticalHit);

            return DealDamage(victim, damageInfoInfo.Damage);
        }

        internal int HealBySpell(Unit target, SpellInfo spellInfo, int healAmount, bool critical = false)
        {
            return DealHeal(target, healAmount);
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
            if (victim.IsImmuneToSpell(spellInfo, this))
                return SpellMissType.Immune;

            if (this == victim)
                return SpellMissType.None;

            // all positive spells can`t miss
            if (spellInfo.IsPositive() && !IsHostileTo(victim))
                return SpellMissType.None;

            return SpellMissType.None;
        }

        internal float GetSpellMinRangeForTarget(Unit target, SpellInfo spellInfo)
        {
            if (Mathf.Approximately(spellInfo.MinRangeFriend, spellInfo.MinRangeHostile))
                return spellInfo.GetMinRange(false);
            if (target == null)
                return spellInfo.GetMinRange(true);
            return spellInfo.GetMinRange(!IsHostileTo(target));
        }

        internal float GetSpellMaxRangeForTarget(Unit target, SpellInfo spellInfo)
        {
            if (Mathf.Approximately(spellInfo.MaxRangeFriend, spellInfo.MaxRangeHostile))
                return spellInfo.GetMaxRange(false);
            if (target == null)
                return spellInfo.GetMaxRange(true);
            return spellInfo.GetMaxRange(!IsHostileTo(target));
        }

        internal Unit GetMagicHitRedirectTarget(Unit victim, SpellInfo spellProto) { return null; }

        internal Unit GetMeleeHitRedirectTarget(Unit victim, SpellInfo spellProto = null) { return null; }

        internal int SpellDamageBonusDone(Unit victim, SpellInfo spellProto, int damage, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        internal void ApplySpellMod(SpellInfo spellInfo, SpellModifierType modifierType, ref int value) { }

        internal void ApplySpellMod(SpellInfo spellInfo, SpellModifierType modifierType, ref float value) { }

        internal int SpellDamageBonusTaken(Unit caster, SpellInfo spellProto, int damage, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1)
        {
            return damage;
        }

        internal uint SpellHealingBonusDone(Unit victim, SpellInfo spellProto, uint healamount, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        internal uint SpellHealingBonusTaken(Unit caster, SpellInfo spellProto, uint healamount, SpellDamageType damagetype, SpellEffectInfo effect, uint stack = 1) { return 0; }

        internal float SpellHealingPercentDone(Unit victim, SpellInfo spellProto) { return 0.0f; }

        internal bool IsSpellCrit(Unit victim, SpellInfo spellProto, SpellSchoolMask schoolMask, WeaponAttackType attackType = WeaponAttackType.BaseAttack) { return false; }

        internal int SpellCriticalHealingBonus(SpellInfo spellProto, int damage, Unit victim) { return 0; }

        internal bool IsImmunedToDamage(SpellSchoolMask meleeSchoolMask) { return false; }

        internal bool IsImmunedToDamage(SpellInfo spellProto) { return false; }

        internal bool IsImmuneToSpell(SpellInfo spellInfo, Unit caster) { return false; }

        internal bool IsImmuneToAura(AuraInfo auraInfo, Unit caster) { return false; }

        internal bool IsImmuneToAuraEffect(AuraEffectInfo auraEffect, Unit caster) { return false; }

        internal void CalcAbsorbResist(Unit victim, SpellSchoolMask schoolMask, SpellDamageType damagetype, int damage, ref int absorb, ref int resist, SpellInfo spellProto = null) { }

        internal void CalcHealAbsorb(Unit victim, SpellInfo spellProto, ref int healAmount, ref int absorb) { }

        internal float ApplyEffectModifiers(SpellInfo spellProto, int effectIndex, float value)
        {
            //var modOwner = this;
            /*modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_ALL_EFFECTS, value);
            switch (effect_index)
            {
                case 0:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT1, value);
                    break;
                case 1:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT2, value);
                    break;
                case 2:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT3, value);
                    break;
                case 3:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT4, value);
                    break;
                case 4:
                    modOwner->ApplySpellMod(spellProto->Id, SPELLMOD_EFFECT5, value);
                    break;
            }*/
            return value;
        }

        internal int ModifyAuraDuration(AuraInfo auraInfo, Unit target, int duration) { return duration; }

        internal void ModSpellCastTime(SpellInfo spellProto, ref int castTime, Spell spell = null) { }

        internal void ModSpellDurationTime(SpellInfo spellProto, ref int castTime, Spell spell = null) { }

        #endregion
    }
}
