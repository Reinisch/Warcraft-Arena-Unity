using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Bolt;
using Common;
using Core.AuraEffects;

namespace Core
{
    public abstract partial class Unit : WorldEntity
    {
        [SerializeField, UsedImplicitly, Header(nameof(Unit)), Space(10)]
        private CapsuleCollider unitCollider;
        [SerializeField, UsedImplicitly]
        private WarcraftCharacterController characterController;
        [SerializeField, UsedImplicitly]
        private UnitAttributeDefinition unitAttributeDefinition;  
        [SerializeField, UsedImplicitly]
        private List<UnitBehaviour> unitBehaviours;

        private SingleReference<Unit> selfReference;
        private UnitControlState controlState;
        private IUnitState entityState;
        private UnitFlags unitFlags;

        private readonly BehaviourController behaviourController = new BehaviourController();

        internal AuraVisibleController VisibleAuras { get; } = new AuraVisibleController();
        internal AuraApplicationController Auras { get; } = new AuraApplicationController();
        internal AttributeController Attributes { get; } = new AttributeController();
        internal CombatController Combat { get; } = new CombatController();
        internal MotionController Motion { get; } = new MotionController();
        internal SpellController Spells { get; } = new SpellController();
        internal WarcraftCharacterController CharacterController => characterController;

        internal ShapeShiftForm ShapeShiftForm { get; private set; }
        internal SpellInfo ShapeShiftSpellInfo { get; private set; }
        internal SpellInfo TransformSpellInfo { get; private set; }
        internal CreateToken UnitCreateToken { get; private set; }
        internal Vehicle Vehicle { get; private set; }

        internal abstract UnitAI AI { get; }

        internal bool FreeForAll { get => Attributes.FreeForAll; set => Attributes.FreeForAll = value; }
        internal int ModelId { get => Attributes.ModelId; set => Attributes.ModelId = value; }
        internal int OriginalModelId { get => Attributes.OriginalModelId; set => Attributes.OriginalModelId = value; }
        internal FactionDefinition Faction { get => Attributes.Faction; set => Attributes.Faction = value; }
        internal DeathState DeathState { get => Attributes.DeathState; set => Attributes.DeathState = value; }
        internal IReadOnlyList<AuraApplication> AuraApplications => Auras.AuraApplications;

        public override float Size => base.Size * Scale;

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
        public int EmoteFrame => entityState.EmoteFrame;
        public int VisibleAuraMaxCount => entityState.VisibleAuras.Length;
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
        public int SlowFallSpeed { get => entityState.SlowFallSpeed; internal set => entityState.SlowFallSpeed = value; }

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

        public sealed override void ControlGained()
        {
            base.ControlGained();

            HandleControlGained();
        }

        public sealed override void ControlLost()
        {
            base.ControlLost();

            HandleControlLost();
        }

        protected virtual void HandleAttach()
        {
            selfReference = new SingleReference<Unit>(this);
            UnitCreateToken = (CreateToken)entity.AttachToken;
            entityState = entity.GetState<IUnitState>();

            behaviourController.HandleUnitAttach(this);

            SetMap(World.FindMap(1));
        }

        protected virtual void HandleDetach()
        {
            DestroyVehicle();

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

        protected virtual void HandleControlGained()
        {
            UpdateSyncTransform(IsOwner);
            CharacterController.UpdateRigidbody();
        }

        protected virtual void HandleControlLost()
        {
            UpdateSyncTransform(true);
            CharacterController.UpdateRigidbody();
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

        internal override void PrepareForScoping()
        {
            base.PrepareForScoping();

            if (IsOwner)
            {
                UnitCreateToken.VisualEffectFlags = Attributes.VisualEffectFlags;
                UnitCreateToken.FactionId = Faction.FactionId;
                UnitCreateToken.DeathState = Attributes.DeathState;
                UnitCreateToken.ClassType = Attributes.ClassType;
                UnitCreateToken.EmoteType = Attributes.EmoteType;
                UnitCreateToken.DisplayPowerType = Attributes.DisplayPowerType;
                UnitCreateToken.Scale = Attributes.Scale;
                UnitCreateToken.ModelId = Attributes.ModelId;
                UnitCreateToken.FreeForAll = Attributes.FreeForAll;
                UnitCreateToken.DisplayPower = Power;
                UnitCreateToken.DisplayPowerMax = MaxPower;
            }
        }

        internal override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            behaviourController.DoUpdate(deltaTime);
        }

        internal void CreateVehicle(VehicleInfo vehicleInfo, CreatureInfo vehicleCreature)
        {
            Vehicle = new Vehicle(this, vehicleInfo, vehicleCreature);
        }

        internal void DestroyVehicle()
        {
            Vehicle?.Dispose();
            Vehicle = null;
        }

        internal void HandleVehicleApplicationEnter(Vehicle vehicle, int seatIndex, Aura vehicleAura)
        {
            Vehicle?.Unit.Auras.RemoveAurasWithEffect(AuraEffectType.ControlVehicle, exceptAura: vehicleAura, onlyWithCaster: this);

            if (this is Player passengerPlayer && vehicle.Unit is Player && passengerPlayer.Combat.InCombat)
                HanderEnteringCancellation();

            if (vehicleAura.IsRemoved)
                return;

            if (vehicle.AddPassenger(this, seatIndex))
                Vehicle = vehicle;
            else
                HanderEnteringCancellation();

            void HanderEnteringCancellation()
            {
                vehicle.Unit.Auras.RemoveOwnedAura(vehicleAura, AuraRemoveMode.Cancel);
            }
        }

        internal void HandleVehicleApplicationExit()
        {
            Vehicle.RemovePassenger(this);
            Vehicle = null;

            // calculate jump target position
            float topCheck = Mathf.Abs(UnitCollider.bounds.max.y) / 2;
            float safeExtentsY = Mathf.Abs(UnitCollider.bounds.extents.y);
            float safeExtentsX = Mathf.Abs(UnitCollider.bounds.extents.x);
            float distance = UnitCollider.height;

            Vector3 targetTop = UnitCollider.bounds.center + Vector3.up * topCheck;
            Vector3 targetPosition;

            Drawing.DrawLine(UnitCollider.bounds.center, UnitCollider.bounds.center + Vector3.up * topCheck, Color.red, 3f);

            if (Physics.Raycast(UnitCollider.bounds.center, Vector3.up, out RaycastHit hitInfo, topCheck, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point - Vector3.up * safeExtentsY;
            else
                targetPosition = targetTop;

            Drawing.DrawLine(targetPosition, targetPosition + transform.forward * distance, Color.red, 3f);

            if (Physics.Raycast(targetPosition, -transform.forward, out hitInfo, distance, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point + transform.forward * safeExtentsX;
            else
                targetPosition = targetPosition - transform.forward * distance;

            Drawing.DrawLine(targetPosition, targetPosition - Vector3.up * topCheck * 1.5f, Color.red, 3f);

            if (Physics.Raycast(targetPosition, -Vector3.up, out hitInfo, topCheck * 2f, PhysicsReference.Mask.Ground))
                targetPosition = hitInfo.point;
            else
                targetPosition = targetPosition - Vector3.up * topCheck * 2f;

            Motion.StartPounceMovement(targetPosition, Attributes.Speed(UnitMoveType.Run));
        }

        internal void EnterVehicle(Vehicle vehicle, int seatIndex)
        {
            Spells.TriggerSpell(Balance.Spells.ControlVehicle, vehicle.Unit, SpellCastFlags.IgnoreCasterMountedOrOnVehicle);
        }

        internal void ExitVehicle()
        {
            Vehicle?.Unit.Auras.RemoveAurasWithEffect(AuraEffectType.ControlVehicle, onlyWithCaster: this);
        }

        internal bool IsOnVehicle(Unit unit)
        {
            return Vehicle != null && Vehicle == unit.Vehicle;
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

        public bool HasMovementFlag(MovementFlags flag) => Motion.HasMovementFlag(flag);

        public T FindBehaviour<T>() where T : UnitBehaviour => behaviourController.FindBehaviour<T>();

        public VisibleAuraState VisibleAura(int index) => entityState.VisibleAuras[index];

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
                CharacterController.UpdateMovementControl(hasControl);
        }

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

        internal void ModifyEmoteState(EmoteType emoteType)
        {
            if (!IsDead && !HasFlag(UnitFlags.Stunned))
                EmoteType = emoteType;
        }

        internal void ModifyDeathState(DeathState newState)
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

            CharacterController.StopMoving();
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

            if (IsOwner && this is Player rootedPlayer)
                EventHandler.ExecuteEvent(GameEvents.ServerPlayerRootChanged, rootedPlayer, applied);
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
