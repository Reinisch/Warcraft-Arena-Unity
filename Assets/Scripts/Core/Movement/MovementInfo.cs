namespace Core
{
    public class MovementInfo
    {
        private IUnitState unitState;
        private IMoveState localMoveState;
        private Unit unit;

        public MovementFlags Flags { get; private set; }
        public bool Jumping { get; set; }
        public bool IsMoving => Flags.IsMoving();

        public void Attached(IUnitState unitState, Unit unit)
        {
            this.unitState = unitState;
            this.unit = unit;

            if (!unit.IsOwner)
                unitState.AddCallback(nameof(unitState.MovementFlags), OnUnitStateFlagsChanged);
        }

        public void Detached()
        {
            if (!unit.IsOwner)
                unitState.RemoveCallback(nameof(unitState.MovementFlags), OnUnitStateFlagsChanged);

            DetachedMoveState();

            unitState = null;
            unit = null;
        }

        public void AttachedMoveState(IMoveState localMoveState)
        {
            this.localMoveState = localMoveState;

            if (unit.IsOwner)
                localMoveState.AddCallback(nameof(unitState.MovementFlags), OnLocalMoveStateFlagsChanged);
        }

        public void DetachedMoveState()
        {
            if (localMoveState != null)
            {
                if (unit.IsOwner)
                    localMoveState.RemoveCallback(nameof(unitState.MovementFlags), OnLocalMoveStateFlagsChanged);

                localMoveState = null;
            }
        }

        public void AddMovementFlag(MovementFlags flag)
        {
            Flags |= flag;

            UpdateMovementState();
        }

        public void RemoveMovementFlag(MovementFlags flag)
        {
            Flags &= ~flag;

            UpdateMovementState();
        }

        public bool HasMovementFlag(MovementFlags flag)
        {
            return (Flags & flag) != 0;
        }

        private void SetFlags(MovementFlags flags)
        {
            Flags = flags;

            UpdateMovementState();
        }

        private void UpdateMovementState()
        {
            if(localMoveState != null && !unit.WorldManager.HasServerLogic)
                localMoveState.MovementFlags = (int)Flags;

            if (unit.WorldManager.HasServerLogic)
                unitState.MovementFlags = (int)Flags;
        }

        private void OnLocalMoveStateFlagsChanged()
        {
            SetFlags((MovementFlags)localMoveState.MovementFlags);
        }

        private void OnUnitStateFlagsChanged()
        {
            SetFlags((MovementFlags)unitState.MovementFlags);
        }
    }
}