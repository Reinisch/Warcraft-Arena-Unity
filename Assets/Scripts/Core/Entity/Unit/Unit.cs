using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;
using Core.AuraEffects;
using Zenject;

namespace Core
{
    public abstract partial class Unit : WorldEntity
    {
        [SerializeField, UsedImplicitly, Header(nameof(Unit)), Space(10)]
        private CapsuleCollider unitCollider;
        [SerializeField, UsedImplicitly]
        private WarcraftCharacterController characterController;
        [SerializeField, UsedImplicitly]
        private List<UnitBehaviour> unitBehaviours;

        [Inject] internal EventBus EventBus { get; private set; }

        private SingleReference<Unit> selfReference;
        private UnitControlState controlState;
        private UnitFlags unitFlags;

        private readonly BehaviourController behaviourController = new BehaviourController();
        private CreateToken createToken;

        internal AuraApplicationController Auras { get; } = new AuraApplicationController();
        internal CombatController Combat { get; } = new CombatController();
        internal MotionController Motion { get; } = new MotionController();
        public SpellController Spells { get; } = new SpellController();
        public WarcraftCharacterController CharacterController => characterController;

        internal ShapeShiftForm ShapeShiftForm { get; private set; }
        internal SpellInfo ShapeShiftSpellInfo { get; private set; }
        internal SpellInfo TransformSpellInfo { get; private set; }

        internal abstract UnitAI AI { get; }
        internal abstract UnitAttributeDefinition AttributeDefinition { get; }

        public bool FreeForAll { get => Attributes.FreeForAll; set => Attributes.FreeForAll = value; }
        public int ModelId { get => Attributes.ModelId; set => Attributes.ModelId = value; }
        public int OriginalModelId { get => Attributes.OriginalModelId; set => Attributes.OriginalModelId = value; }
        public FactionDefinition Faction { get => Attributes.Faction; set => Attributes.Faction = value; }
        public DeathState DeathState { get => Attributes.DeathState; set => Attributes.DeathState = value; }
        public IReadOnlyList<AuraApplication> AuraApplications => Auras.AuraApplications;
        public CreateToken CreationToken { get; private set; }
        public AttributeController Attributes { get; } = new AttributeController();
        public AuraVisibleController VisibleAuras { get; } = new AuraVisibleController();
        public override float Size => base.Size * Scale;
        public Vector3 Velocity => CharacterController.Velocity;
        public MovementFlags MovementFlags => Motion.MovementFlags;
        public IReadOnlyReference<Unit> SelfReference => selfReference;
        public Unit Target => Attributes.Target;
        public SpellCast SpellCast => Spells.Cast;
        public SpellHistory SpellHistory => Spells.SpellHistory;
        public CapsuleCollider UnitCollider => unitCollider;

        public int Model => Attributes.ModelId;
        public int Health => Attributes.Health.Value;
        public int MaxHealth => Attributes.MaxHealth.Value;
        public int Power => Attributes.Power(DisplayPowerType);
        public int MaxPower => Attributes.MaxPower(DisplayPowerType);
        public int ComboPoints => Attributes.ComboPoints.Value;
        public int MaxComboPoints => Attributes.ComboPoints.Max;
        public int SpellPower => Attributes.SpellPower.Value;
        public float EmoteFrame => Attributes.EmoteTime;
        public float RotationSpeed => CharacterController.Definition.RotateSpeed;
        public float RunSpeed => Attributes.Speed(UnitMoveType.Run);
        public float ModHaste => Attributes.ModHaste.Value;
        public float CritPercentage => Attributes.CritPercentage.Value;
        public float HealthRatio => MaxHealth > 0 ? (float)Health / MaxHealth : 0.0f;
        public bool IsMovementBlocked => HasState(UnitControlState.Root) || HasState(UnitControlState.Stunned);
        public bool IsAlive => DeathState == DeathState.Alive;
        public bool IsDead => DeathState == DeathState.Dead;
        public bool IsControlledByPlayer => this is Player;
        public float Scale { get => Attributes.Scale; internal set => Attributes.Scale = value; }
        public UnitVisualEffectFlags VisualEffects => Attributes.VisualEffectFlags;
        public SpellPowerType DisplayPowerType { get => Attributes.DisplayPowerType; internal set => Attributes.DisplayPowerType = value; }
        public ClassType ClassType { get => Attributes.ClassType; internal set => Attributes.ClassType = value; }
        public EmoteType EmoteType { get => Attributes.EmoteType; internal set => Attributes.EmoteType = value; }
        public MovementMode MovementMode { get => Attributes.MovementMode; internal set => Attributes.MovementMode = value; }
        public int SlowFallSpeed { get => Attributes.SlowFallSpeed; internal set => Attributes.SlowFallSpeed = value; }

        public override void Attached(Entity.CreateToken createToken)
        {
            CreationToken = (CreateToken)createToken;

            base.Attached(createToken);

            HandleAttach();

            World.UnitManager.Attach(this);
        }

        public sealed override void Detached()
        {
            World.UnitManager.Detach(this);

            HandleDetach();

            base.Detached();
        }

        protected virtual void HandleAttach()
        {
            selfReference = new SingleReference<Unit>(this);

            behaviourController.HandleUnitAttach(this);

            Assert.IsNotNull(CreationToken.Map, $"{name}) created without a map!");
            SetMap(CreationToken.Map ?? World.MapController.PrimaryMap);
        }

        protected virtual void HandleDetach()
        {
            ResetShapeShiftForm();
            ResetTransformSpell();

            behaviourController.HandleUnitDetach();

            ResetMap();

            selfReference.Invalidate();
            selfReference = null;

            TransformSpellInfo = null;
            controlState = 0;
            unitFlags = 0;
        }

        protected virtual void AddBehaviours(BehaviourController unitBehaviourController)
        {
            unitBehaviourController.TryAddBehaviour(Attributes);
            unitBehaviourController.TryAddBehaviour(CharacterController);
            unitBehaviourController.TryAddBehaviour(Combat);
            unitBehaviourController.TryAddBehaviour(Motion);
            unitBehaviourController.TryAddBehaviour(Spells);
            unitBehaviourController.TryAddBehaviour(Auras);
            unitBehaviourController.TryAddBehaviour(VisibleAuras);
        }

        internal override void DoUpdate(int deltaTime)
        {
            behaviourController.DoUpdate(deltaTime);
        }

        /// <summary>Sets the unit's facing through the character motor. The KCC owns rotation (it reapplies its
        /// own <c>TransientRotation</c> each step), so a plain <see cref="WorldEntity.Rotation"/> set is
        /// overwritten — kinematic movement (e.g. pounce) must route the facing through the motor to take effect.</summary>
        public void SetFacing(Quaternion rotation)
        {
            characterController.Motor.SetRotation(rotation);
        }

        public override void Teleport(Vector3 position, bool notify = true)
        {
            characterController.Motor.SetPosition(position);

            base.Teleport(position, notify);

            // Server-authoritative: notify so the net layer relays the teleport to the owning client, whose
            // movement is client-authoritative and otherwise wouldn't follow a server-side position change.
            if (notify && World.HasServerLogic)
                EventBus.ExecuteEvent(Common.GameEvents.ServerUnitTeleported, this, position);
        }

        public bool IsOnSameMap(Unit unit) => Map == unit.Map;

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

        public bool HasMovementFlag(MovementFlags flag) => Motion.HasMovementFlag(flag);

        public T FindBehaviour<T>() where T : UnitBehaviour => behaviourController.FindBehaviour<T>();

        public AuraApplication VisibleAura(int index) => VisibleAuras.ApplicationSlots[index];

        internal bool HasAuraType(AuraEffectType auraEffectType) => Auras.HasAuraType(auraEffectType);

        internal bool HasAuraState(AuraStateType auraStateType, Unit caster = null, Spell spell = null) => Auras.HasAuraState(auraStateType, caster, spell);

        internal IReadOnlyList<AuraEffect> GetAuraEffects(AuraEffectType auraEffectType) => Auras.GetAuraEffects(auraEffectType);

        internal float TotalAuraModifier(AuraEffectType auraType) => Auras.TotalAuraModifier(auraType);

        internal float TotalAuraMultiplier(AuraEffectType auraType) => Auras.TotalAuraMultiplier(auraType);

        internal float MaxPositiveAuraModifier(AuraEffectType auraType) => Auras.MaxPositiveAuraModifier(auraType);

        internal float MaxNegativeAuraModifier(AuraEffectType auraType) => Auras.MaxNegativeAuraModifier(auraType);

        internal bool IsImmunedToDamage(SpellInfo spellInfo, SpellSchoolMask? schoolMaskOverride = null, Unit caster = null)
        {
            return Spells.IsImmunedToDamage(spellInfo, schoolMaskOverride, caster);
        }

        internal bool IsImmuneToSpell(SpellInfo spellInfo, Unit caster) => Spells.IsImmuneToSpell(spellInfo, caster);

        internal bool IsImmuneToAura(AuraInfo auraInfo, Unit caster) => Spells.IsImmuneToAura(auraInfo, caster);

        internal bool IsImmuneToAuraEffect(AuraEffectInfo auraEffectInfo, Unit caster) => Spells.IsImmuneToAuraEffect(auraEffectInfo, caster);

        internal void AddState(UnitControlState state) => controlState |= state;

        internal bool HasState(UnitControlState state) => (controlState & state) == state;

        internal bool HasAnyState(UnitControlState state) => (controlState & state) != 0;

        internal void RemoveState(UnitControlState state) => controlState &= ~state;

        internal void SetMovementFlag(MovementFlags flag, bool add) => Motion.SetMovementFlag(flag, add);

        internal void SetFlag(UnitFlags flag) => unitFlags |= flag;

        internal void RemoveFlag(UnitFlags flag) => unitFlags &= ~flag;

        internal bool HasFlag(UnitFlags flag) => (unitFlags & flag) == flag;

        internal void UpdateControlState(UnitControlState state, bool applied)
        {
            if (applied && HasState(state))
                return;

            if (!applied && !HasState(state))
                return;

            bool hadControl = Motion.HasMovementControl;
            bool hadFreeMovement = HasFreeMovement;

            if (applied)
            {
                switch (state)
                {
                    case UnitControlState.Stunned:
                        UpdateStunState(true);
                        break;
                    case UnitControlState.Root:
                        if(!HasState(UnitControlState.Stunned))
                            UpdateRootState(true);
                        break;
                    case UnitControlState.Confused:
                        if (!HasState(UnitControlState.Stunned))
                        {
                            SpellCast.Cancel();
                            UpdateConfusionState(true);
                        }
                        break;
                }

                AddState(state);
            }
            else
            {
                switch (state)
                {
                    case UnitControlState.Stunned:
                        if (!HasAuraType(AuraEffectType.StunState))
                            UpdateStunState(false);
                        if (!HasAuraType(AuraEffectType.RootState))
                            UpdateRootState(false);
                        break;
                    case UnitControlState.Root:
                        if (!HasAuraType(AuraEffectType.RootState) && !HasState(UnitControlState.Stunned))
                            UpdateRootState(false);
                        break;
                    case UnitControlState.Confused:
                        if (!HasAuraType(AuraEffectType.ConfusionState))
                            UpdateConfusionState(false);
                        break;
                    default:
                        RemoveState(state);
                        break;
                }
            }

            if (HasAuraType(AuraEffectType.StunState))
            {
                if (!HasState(UnitControlState.Stunned))
                    UpdateStunState(true);
            }
            else
            {
                if (!HasState(UnitControlState.Root) && HasAuraType(AuraEffectType.RootState))
                    UpdateRootState(true);

                if (!HasState(UnitControlState.Confused) && HasAuraType(AuraEffectType.ConfusionState))
                    UpdateConfusionState(true);
            }

            bool hasControl = !HasAnyState(UnitControlState.LostControl);
            if (hasControl != hadControl)
                Motion.UpdateMovementControl(hasControl);

            // Net authority: when the player can no longer move itself (root/stun/polymorph/fear), the server
            // takes movement authority (re-owns its shadow); restored when it can move again. Server-only.
            if (HasFreeMovement != hadFreeMovement && World.HasServerLogic)
                EventBus.ExecuteEvent(Common.GameEvents.ServerPlayerMovementControlChanged, this, HasFreeMovement);
        }

        /// <summary>True when this unit can freely control its own movement (no root/stun/loss-of-control) —
        /// i.e. its movement stays client-authoritative; false means the server drives it.</summary>
        public bool HasFreeMovement => !IsMovementBlocked && !HasAnyState(UnitControlState.LostControl);

        internal void UpdateShapeShiftForm(AuraEffectShapeShift shapeShiftEffect)
        {
            ShapeShiftForm = shapeShiftEffect.EffectInfo.ShapeShiftForm;
            ShapeShiftSpellInfo = shapeShiftEffect.Aura.SpellInfo;
        }

        internal void ResetShapeShiftForm()
        {
            ShapeShiftForm = ShapeShiftForm.None;
            ShapeShiftSpellInfo = null;
        }

        internal void UpdateTransformSpell(AuraEffectChangeDisplayModel changeDisplayEffect)
        {
            TransformSpellInfo = changeDisplayEffect.Aura.SpellInfo;
            ModelId = changeDisplayEffect.EffectInfo.ModelId;
        }

        internal void ResetTransformSpell()
        {
            TransformSpellInfo = null;
            ModelId = OriginalModelId;
        }

        public void ModifyEmoteState(EmoteType emoteType)
        {
            if (!IsDead && !HasFlag(UnitFlags.Stunned))
                EmoteType = emoteType;
        }

        public void ModifyDeathState(DeathState newState)
        {
            DeathState = newState;

            if (IsDead && SpellCast.IsCasting)
                SpellCast.Cancel();

            if (newState == DeathState.Dead)
            {
                Auras.RemoveNonDeathPersistentAuras();

                ModifyEmoteState(EmoteType.None);
            }
        }

        internal void ModifyHealth(int delta)
        {
            Attributes.SetHealth(Health + delta);
        }

        public void ApplyAttributeDefinition(UnitAttributeDefinition definition)
        {
            if (definition != null)
                Attributes.ApplyAttributeDefinition(definition);
        }

        internal void ModifyComboPoints(int delta)
        {
            Attributes.SetComboPoints(ComboPoints + delta);
        }

        internal void DealDamage(Unit target, int damageAmount, SpellDamageType spellDamageType)
        {
            if (damageAmount < 1)
                return;

            if (spellDamageType != SpellDamageType.Processed)
            {
                target.Auras.RemoveAurasWithInterrupt(AuraInterruptFlags.AnyDamageTaken);

                if (spellDamageType == SpellDamageType.Direct)
                    target.Auras.RemoveAurasWithInterrupt(AuraInterruptFlags.DirectDamageTaken);

                target.Auras.RemoveAurasWithCombinedDamageInterrupt(damageAmount);
            }

            int healthValue = target.Health;
            if (healthValue <= damageAmount)
            {
                Kill(target);
                return;
            }

            target.ModifyHealth(-damageAmount);
        }

        internal void DealHeal(Unit target, int healAmount)
        {
            if (healAmount < 1)
                return;

            target.ModifyHealth(healAmount);
        }

        internal void Kill(Unit victim)
        {
            if (victim.Health <= 0)
                return;

            victim.Attributes.SetHealth(0);
            victim.ModifyDeathState(DeathState.Dead);
        }

        internal void StopMoving()
        {
            SetMovementFlag(MovementFlags.MaskMoving, false);

            CharacterController.Motor.ResetVelocity();
        }

        private void UpdateStunState(bool applied)
        {
            if (applied)
            {
                SpellCast.Cancel();
                StopMoving();

                SetFlag(UnitFlags.Stunned);
                AddState(UnitControlState.Stunned);

                UpdateRootState(true);
            }
            else
            {
                RemoveState(UnitControlState.Stunned);
                RemoveFlag(UnitFlags.Stunned);

                if (!HasState(UnitControlState.Root))
                    UpdateRootState(false);
            }
        }

        private void UpdateRootState(bool applied)
        {
            if (applied)
            {
                StopMoving();

                AddState(UnitControlState.Root);
                SetMovementFlag(MovementFlags.Root, true);
            }
            else
            {
                RemoveState(UnitControlState.Root);
                SetMovementFlag(MovementFlags.Root, false);
            }
        }

        private void UpdateConfusionState(bool applied)
        {
            if (applied)
            {
                SetFlag(UnitFlags.Confused);
                AddState(UnitControlState.Confused);
                Motion.ModifyConfusedMovement(true);
            }
            else
            {
                RemoveFlag(UnitFlags.Confused);
                RemoveState(UnitControlState.Confused);
                Motion.ModifyConfusedMovement(false);
            }
        }
    }
}
