using UnityEngine;

namespace Core
{
    public abstract partial class Unit
    {
        internal class MotionController : IUnitBehaviour
        {
            private const int IgnoredFramesAfterControlGained = 10;

            private Unit unit;
            private BoltEntity moveEntity;
            private IMoveState moveState;

            private int currentMovementIndex;
            private int remoteControlGainFrame;

            private readonly IdleMovement idleMovement = new IdleMovement();
            private readonly MovementGenerator[] movementGenerators = new MovementGenerator[MovementUtils.MovementSlots.Length];
            private readonly bool[] startedMovement = new bool[MovementUtils.MovementSlots.Length];

            private MovementGenerator CurrentMovement => movementGenerators[currentMovementIndex];
            private bool CurrentAlreadyStarted => startedMovement[currentMovementIndex];

            internal bool UsesKinematicMovement { get; set; }
            internal bool HasMovementControl { get; private set; }

            public MovementFlags MovementFlags { get; private set; }
            public bool Jumping { get; set; }
            public bool IsMoving => MovementFlags.IsMoving();

            public bool HasClientLogic => true;
            public bool HasServerLogic => true;

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                if (!CurrentMovement.Update(unit, deltaTime))
                    ResetCurrentMovement();
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;

                currentMovementIndex = 0;

                StartMovement(idleMovement, MovementSlot.Idle);

                if (!unit.IsOwner)
                    unit.AddCallback(nameof(IUnitState.MovementFlags), OnUnitStateFlagsChanged);
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                if (!unit.IsOwner)
                    unit.RemoveCallback(nameof(IUnitState.MovementFlags), OnUnitStateFlagsChanged);

                ResetAllMovement();

                DetachMoveState(true);

                currentMovementIndex = 0;

                unit = null;
            }

            public bool HasMovementFlag(MovementFlags flag)
            {
                return (MovementFlags & flag) != 0;
            }
            
            public void ModifyConfusedMovement(bool isConfused)
            {
                if (isConfused)
                    StartMovement(new ConfusedMovement(), MovementSlot.Controlled);
                else
                    CancelMovement(MovementType.Confused, MovementSlot.Controlled);
            }

            public void StartChargingMovement(Vector3 chargePoint, float chargeSpeed)
            {
                StartMovement(new ChargeMovement(chargePoint, chargeSpeed), MovementSlot.Controlled);
            }

            public void StartPounceMovement(Vector3 pouncePoint, float pounceSpeed)
            {
                StartMovement(new PounceMovement(pouncePoint, pounceSpeed), MovementSlot.Controlled);
            }

            internal void OverrideMovementFlags(MovementFlags flags)
            {
                MovementFlags = flags;

                UpdateMovementState();
            }

            internal void SetMovementFlag(MovementFlags flag, bool add)
            {
                if (add)
                    MovementFlags |= flag;
                else
                    MovementFlags &= ~flag;

                UpdateMovementState();
            }

            internal void UpdateMovementControl(bool hasControl)
            {
                if (unit.IsOwner && hasControl && !HasMovementControl)
                    remoteControlGainFrame = BoltNetwork.ServerFrame;

                HasMovementControl = hasControl;
            }

            internal void AttachMoveState(BoltEntity moveEntity)
            {
                this.moveEntity = moveEntity;

                moveState = moveEntity.GetState<IMoveState>();
                moveState.SetTransforms(moveState.LocalTransform, moveEntity.transform);

                if (unit.IsOwner)
                    moveState.AddCallback(nameof(IUnitState.MovementFlags), OnMoveStateFlagsChanged);
            }

            internal void DetachMoveState(bool destroyEntity)
            {
                if (moveEntity == null)
                    return;

                if (unit.IsOwner)
                    moveState.RemoveCallback(nameof(IUnitState.MovementFlags), OnMoveStateFlagsChanged);

                if (destroyEntity)
                {
                    if (!moveEntity.IsOwner || !moveEntity.IsAttached)
                        Destroy(moveEntity.gameObject);
                    else
                        BoltNetwork.Destroy(moveEntity.gameObject);
                }

                moveEntity = null;
                moveState = null;
            }

            internal void SimulateOwner()
            {
                if (unit.Motion.HasMovementControl && moveEntity != null)
                {
                    bool shouldIgnore = BoltNetwork.ServerFrame < remoteControlGainFrame + IgnoredFramesAfterControlGained;
                    if (!shouldIgnore)
                    {
                        unit.Position = moveEntity.transform.position;
                        unit.Rotation = moveEntity.transform.rotation;
                    }
                }

                if (unit.Motion.IsMoving)
                    unit.IsVisibilityChanged = true;
            }

            internal void SimulateController()
            {
                if (!unit.IsOwner && moveEntity != null)
                {
                    moveEntity.transform.position = unit.Position;
                    moveEntity.transform.rotation = unit.Rotation;
                }
            }

            private void StartMovement(MovementGenerator movement, MovementSlot newMovementSlot)
            {
                int newMovementIndex = (int)newMovementSlot;

                FinishMovement(newMovementIndex);

                if (currentMovementIndex < newMovementIndex)
                    currentMovementIndex = newMovementIndex;

                movementGenerators[newMovementIndex] = movement;

                if (currentMovementIndex > newMovementIndex)
                    startedMovement[newMovementIndex] = false;
                else
                    BeginMovement(newMovementIndex);
            }

            private void CancelMovement(MovementType movementType, MovementSlot cancelledMovementSlot)
            {
                int cancelledIndex = (int)cancelledMovementSlot;
                if (movementGenerators[cancelledIndex] == null)
                    return;

                if (movementGenerators[cancelledIndex].Type != movementType)
                    return;

                if (currentMovementIndex == cancelledIndex)
                    ResetCurrentMovement();
                else
                    FinishMovement(cancelledIndex);
            }

            private void ResetCurrentMovement()
            {
                while (currentMovementIndex > 0)
                {
                    FinishMovement(currentMovementIndex);

                    currentMovementIndex--;

                    if (CurrentMovement != null)
                    {
                        if (!CurrentAlreadyStarted)
                            BeginMovement(currentMovementIndex);

                        break;
                    }
                }
            }

            private void ResetAllMovement()
            {
                while (currentMovementIndex > 0)
                {
                    FinishMovement(currentMovementIndex);

                    currentMovementIndex--;
                }
            }

            private void BeginMovement(int index)
            {
                SwitchGenerator(index, true);
            }

            private void FinishMovement(int index)
            {
                if (movementGenerators[index] == null || index == 0)
                    return;

                if (startedMovement[index])
                    SwitchGenerator(index, false);

                movementGenerators[index] = null;
            }

            private void SwitchGenerator(int index, bool active)
            {
                if (active)
                    movementGenerators[index].Begin(unit);
                else
                    movementGenerators[index].Finish(unit);

                startedMovement[index] = active;
                unit.CharacterController.UpdateRigidbody();
            }

            private void SetFlags(MovementFlags flags)
            {
                MovementFlags = flags;

                UpdateMovementState();
            }

            private void UpdateMovementState()
            {
                if (unit.World.HasServerLogic)
                    unit.entityState.MovementFlags = (int)MovementFlags;
                else if (moveEntity != null && HasMovementControl)
                    moveState.MovementFlags = (int)MovementFlags;
            }

            private void OnUnitStateFlagsChanged()
            {
                if (unit.IsController && HasMovementControl)
                    return;

                SetFlags((MovementFlags)unit.entityState.MovementFlags);
            }

            private void OnMoveStateFlagsChanged()
            {
                if (HasMovementControl)
                    SetFlags((MovementFlags)moveState.MovementFlags);
            }
        }
    }
}