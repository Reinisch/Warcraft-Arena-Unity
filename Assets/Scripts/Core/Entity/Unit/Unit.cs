using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UdpKit;
using Bolt;

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
        private UnitControlState controlState;
        private IUnitState entityState;
        private UnitFlags unitFlags;

        private readonly BehaviourController behaviourController = new BehaviourController();

        internal AuraVisibleController VisibleAuras { get; } = new AuraVisibleController();
        internal AuraApplicationController Auras { get; } = new AuraApplicationController();
        internal AttributeController Attributes { get; } = new AttributeController();
        internal ThreatController Threat { get; } = new ThreatController();
        internal SpellController Spells { get; } = new SpellController();
        internal WarcraftCharacterController CharacterController => characterController;

        internal bool FreeForAll { get => Attributes.FreeForAll; set => Attributes.FreeForAll = value; }
        internal FactionDefinition Faction { get => Attributes.Faction; set => Attributes.Faction = value; }
        internal DeathState DeathState { get => Attributes.DeathState; set => Attributes.DeathState = value; }
        internal IReadOnlyDictionary<UnitMoveType, float> SpeedRates => Attributes.SpeedRates;
        internal IReadOnlyList<AuraApplication> AuraApplications => Auras.AuraApplications;

        public Unit Target => Attributes.Target;
        public SpellCast SpellCast => Spells.SpellCast;
        public SpellHistory SpellHistory => Spells.SpellHistory;
        public CapsuleCollider UnitCollider => unitCollider;
        public PlayerControllerDefinition ControllerDefinition => characterController.ControllerDefinition;

        public int Level => Attributes.Level.Value;
        public int Health => Attributes.Health.Value;
        public int MaxHealth => Attributes.MaxHealth.Value;
        public int BaseMana => Attributes.Mana.Base;
        public int Mana => Attributes.Mana.Value;
        public int MaxMana => Attributes.MaxMana.Value;
        public int SpellPower => Attributes.SpellPower.Value;
        public int VisibleAuraMaxCount => entityState.VisibleAuras.Length;
        public float ModHaste => Attributes.ModHaste.Value;
        public float ModRegenHaste => Attributes.ModRegenHaste.Value;
        public float CritPercentage => Attributes.CritPercentage.Value;
        public float HealthRatio => MaxHealth > 0 ? (float)Health / MaxHealth : 0.0f;
        public bool IsMovementBlocked => HasState(UnitControlState.Root) || HasState(UnitControlState.Stunned);
        public bool IsAlive => DeathState == DeathState.Alive;
        public bool IsDead => DeathState == DeathState.Dead;
        public bool IsControlledByPlayer => this is Player;
        public bool IsStopped => !HasState(UnitControlState.Moving);

        public bool HealthBelowPercent(int percent) => Health < MaxHealth.CalculatePercentage(percent);
        public bool HealthAbovePercent(int percent) => Health > MaxHealth.CalculatePercentage(percent);
        public bool HealthAbovePercentHealed(int percent, int healAmount) => Health + healAmount > MaxHealth.CalculatePercentage(percent);
        public bool HealthBelowPercentDamaged(int percent, int damageAmount) => Health - damageAmount < MaxHealth.CalculatePercentage(percent);
        public float GetSpeed(UnitMoveType type) => SpeedRates[type] * unitMovementDefinition.BaseSpeedByType(type);
        public float GetPowerPercent(SpellResourceType type) => GetMaxPower(type) > 0 ? 100.0f * GetPower(type) / GetMaxPower(type) : 0.0f;
        public int GetPower(SpellResourceType type) => Mana;
        public int GetMaxPower(SpellResourceType type) => MaxMana;
        public VisibleAuraState GetVisibleAura(int index) => entityState.VisibleAuras[index];

        public sealed override void Attached()
        {
            base.Attached();

            HandleAttach();
            
            World.UnitManager.Attach(this);
        }

        public sealed override void Detached()
        {
            // called twice on client (from Detached Photon callback and manual in UnitManager.Dispose)
            // if he needs to instantly destroy current world and avoid any events
            if (IsValid)
            {
                World.UnitManager.Detach(this);

                HandleDetach();

                base.Detached();
            }
        }

        protected virtual void HandleAttach()
        {
            createToken = (CreateToken)entity.AttachToken;
            entityState = entity.GetState<IUnitState>();

            behaviourController.HandleUnitAttach(this);
            MovementInfo.Attached(this, entityState);

            SetMap(World.FindMap(1));
        }

        protected virtual void HandleDetach()
        {
            ResetMap();

            MovementInfo.Detached();
            behaviourController.HandleUnitDetach();
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

        public void AddCallback(string path, PropertyCallback propertyCallback) => entityState.AddCallback(path, propertyCallback);

        public void AddCallback(string path, PropertyCallbackSimple propertyCallback) => entityState.AddCallback(path, propertyCallback);

        public void RemoveCallback(string path, PropertyCallback propertyCallback) => entityState.RemoveCallback(path, propertyCallback);

        public void RemoveCallback(string path, PropertyCallbackSimple propertyCallback) => entityState.RemoveCallback(path, propertyCallback);

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

        internal void AddState(UnitControlState state) { controlState |= state; }

        internal bool HasState(UnitControlState state) { return (controlState & state) != 0; }

        internal void RemoveState(UnitControlState state) { controlState &= ~state; }

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
            return Attributes.SetHealth(Health + delta);
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

            victim.Attributes.SetHealth(0);
            victim.ModifyDeathState(DeathState.Dead);
        }
    }
}
