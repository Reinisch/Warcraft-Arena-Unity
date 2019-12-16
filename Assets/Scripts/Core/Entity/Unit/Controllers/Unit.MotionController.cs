namespace Core
{
    public abstract partial class Unit
    {
        internal class MotionController : IUnitBehaviour
        {
            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

            private Unit unit;
            private int currentMovementIndex;
            private readonly IdleMovement idleMovement = new IdleMovement();
            private readonly MovementGenerator[] movementGenerators = new MovementGenerator[MovementUtils.MovementSlots.Length];
            private readonly bool[] startedMovement = new bool[MovementUtils.MovementSlots.Length];

            private MovementGenerator CurrentMovement => movementGenerators[currentMovementIndex];
            private bool CurrentAlreadyStarted => startedMovement[currentMovementIndex];

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                if (!CurrentMovement.Update(unit, deltaTime))
                    ResetCurrentMovement();
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;

                currentMovementIndex = 0;

                ModifyMovement(idleMovement, MovementSlot.Idle);
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                ResetAllMovement();

                currentMovementIndex = 0;

                unit = null;
            }

            private void ModifyMovement(MovementGenerator movement, MovementSlot newSlot)
            {
                int newIndex = (int)newSlot;

                FinishMovement(newIndex);

                if (currentMovementIndex < newIndex)
                    currentMovementIndex = newIndex;

                movementGenerators[newIndex] = movement;

                if (currentMovementIndex > newIndex)
                    startedMovement[newIndex] = false;
                else
                    BeginMovement(newIndex);
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
                startedMovement[index] = true;

                movementGenerators[index].Begin(unit);
            }

            private void FinishMovement(int index)
            {
                if (movementGenerators[index] == null || index == 0)
                    return;

                if (startedMovement[index])
                    movementGenerators[index].Finish(unit);

                startedMovement[index] = false;
                movementGenerators[index] = null;
            }
        }
    }
}