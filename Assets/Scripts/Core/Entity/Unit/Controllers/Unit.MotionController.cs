using UnityEngine;

namespace Core
{
    public abstract partial class Unit
    {
        internal class MotionController : IUnitBehaviour
        {
            private Unit unit;
            private int currentMovementIndex;
            private readonly IdleMovement idleMovement = new IdleMovement();
            private readonly MovementGenerator[] movementGenerators = new MovementGenerator[MovementUtils.MovementSlots.Length];
            private readonly bool[] startedMovement = new bool[MovementUtils.MovementSlots.Length];

            private MovementGenerator CurrentMovement => movementGenerators[currentMovementIndex];
            private bool CurrentAlreadyStarted => startedMovement[currentMovementIndex];

            internal bool UsesKinematicMovement { get; set; }

            public bool HasClientLogic => false;
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
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                ResetAllMovement();

                currentMovementIndex = 0;

                unit = null;
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
        }
    }
}