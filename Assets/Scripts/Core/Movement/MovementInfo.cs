namespace Core
{
    public class MovementInfo
    {
        private IMoveState localMoveState;
        private IUnitState unitState;
        private Unit unit;

        public MovementFlags Flags { get; private set; }
        public bool Jumping { get; set; }
        public bool IsMoving => Flags.IsMoving();

        public void Attached(Unit unit, IUnitState unitState)
        {
            this.unit = unit;
            this.unitState = unitState;

            if (!unit.IsOwner)
                unit.AddCallback(nameof(IUnitState.MovementFlags), OnUnitStateFlagsChanged);
        }

        public void Detached()
        {
            if (!unit.IsOwner)
                unit.RemoveCallback(nameof(IUnitState.MovementFlags), OnUnitStateFlagsChanged);

            DetachedMoveState();

            unit = null;
            unitState = null;
        }

        public void AttachedMoveState(IMoveState localMoveState)
        {
            this.localMoveState = localMoveState;

            if (unit.IsOwner)
                localMoveState.AddCallback(nameof(IUnitState.MovementFlags), OnLocalMoveStateFlagsChanged);
        }

        public void DetachedMoveState()
        {
            if (localMoveState != null)
            {
                if (unit.IsOwner)
                    localMoveState.RemoveCallback(nameof(IUnitState.MovementFlags), OnLocalMoveStateFlagsChanged);

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
            if(localMoveState != null && !unit.World.HasServerLogic)
                localMoveState.MovementFlags = (int)Flags;

            if (unit.World.HasServerLogic)
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