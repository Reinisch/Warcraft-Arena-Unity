using System.Collections.Generic;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

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

        internal AuraVisibleController VisibleAuras { get; } = new AuraVisibleController();
        internal AuraApplicationController Auras { get; } = new AuraApplicationController();
        internal AttributeController Attributes { get; } = new AttributeController();
        internal ThreatController Threat { get; } = new ThreatController();
        internal SpellController Spells { get; } = new SpellController();
        internal WarcraftCharacterController CharacterController => characterController;

        internal FactionDefinition Faction { get => Attributes.Faction; set => Attributes.Faction = value; }
        internal DeathState DeathState { get => Attributes.DeathState; set => Attributes.DeathState = value; }
        internal bool FreeForAll { get => Attributes.FreeForAll; set => Attributes.FreeForAll = value; }

        internal IReadOnlyDictionary<UnitMoveType, float> SpeedRates => Attributes.SpeedRates;
        internal IReadOnlyList<AuraApplication> AuraApplications => Auras.AuraApplications;
        internal EntityAttributeInt HealthAttribute => Attributes.Health;
        internal EntityAttributeInt MaxHealthAttribute => Attributes.MaxHealth;
        internal EntityAttributeInt ManaAttribute => Attributes.Mana;
        internal EntityAttributeInt MaxManaAttribute => Attributes.MaxMana;
        internal EntityAttributeInt LevelAttribute => Attributes.Level;
        internal EntityAttributeInt SpellPowerAttribute => Attributes.SpellPower;
        internal EntityAttributeFloat ModHasteAttribute => Attributes.ModHaste;
        internal EntityAttributeFloat ModRegenHasteAttribute => Attributes.ModRegenHaste;
        internal EntityAttributeFloat CritPercentageAttribute => Attributes.CritPercentage;

        public IUnitState EntityState { get; private set; }

        public Unit Target => Attributes.Target;
        public SpellCast SpellCast => Spells.SpellCast;
        public SpellHistory SpellHistory => Spells.SpellHistory;
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

        protected virtual void HandleAttach()
        {
            Attributes.InitializeAttributes(this);

            createToken = (CreateToken) entity.AttachToken;
            EntityState = entity.GetState<IUnitState>();

            MovementInfo.Attached(this);

            SetMap(World.FindMap(1));
        }

        protected virtual void HandleDetach()
        {
            EntityState.RemoveAllCallbacks();

            ResetMap();

            MovementInfo.Detached();

            createToken = null;
        }

        internal override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            behaviourController.DoUpdate(deltaTime);
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

        internal bool HasAuraType(AuraEffectType auraEffectType) => Auras.HasAuraType(auraEffectType);

        internal float TotalAuraModifier(AuraEffectType auraType) => Auras.TotalAuraModifier(auraType);

        internal float TotalAuraMultiplier(AuraEffectType auraType) => Auras.TotalAuraMultiplier(auraType);

        internal float MaxPositiveAuraModifier(AuraEffectType auraType) => Auras.MaxPositiveAuraModifier(auraType);

        internal float MaxNegativeAuraModifier(AuraEffectType auraType) => Auras.MaxNegativeAuraModifier(auraType);

        internal bool IsImmunedToDamage(SpellInfo spellInfo) => Spells.IsImmunedToDamage(spellInfo);

        internal bool IsImmuneToSpell(SpellInfo spellInfo, Unit caster) => Spells.IsImmuneToSpell(spellInfo, caster);

        internal bool IsImmuneToAura(AuraInfo auraInfo, Unit caster) => Spells.IsImmuneToAura(auraInfo, caster);

        internal bool IsImmuneToAuraEffect(AuraEffectInfo auraEffectInfo, Unit caster) => Spells.IsImmuneToAuraEffect(auraEffectInfo, caster);

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
    }
}
