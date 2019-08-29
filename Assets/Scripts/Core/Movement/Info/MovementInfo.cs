namespace Core
{
    public class MovementInfo
    {
        protected IUnitState UnitState { get; private set; }
        protected Unit Unit { get; private set; }

        internal bool HasMovementControl { get; set; }
        internal virtual BoltEntity MoveEntity => null;

        public MovementFlags Flags { get; private set; }
        public bool Jumping { get; set; }
        public bool IsMoving => Flags.IsMoving();

        public MovementInfo(Unit unit, IUnitState unitState)
        {
            Unit = unit;
            UnitState = unitState;

            if (!unit.IsOwner)
                unit.AddCallback(nameof(IUnitState.MovementFlags), OnUnitStateFlagsChanged);
        }

        public virtual void Dispose()
        {
            if (!Unit.IsOwner)
                Unit.RemoveCallback(nameof(IUnitState.MovementFlags), OnUnitStateFlagsChanged);

            Unit = null;
            UnitState = null;
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

        protected virtual void UpdateMovementState()
        {
            if (Unit.World.HasServerLogic)
                UnitState.MovementFlags = (int)Flags;
        }

        protected void SetFlags(MovementFlags flags)
        {
            Flags = flags;

            UpdateMovementState();
        }

        private void OnUnitStateFlagsChanged()
        {
            if (Unit.IsController && HasMovementControl)
                return;

            SetFlags((MovementFlags)UnitState.MovementFlags);
        }
    }
}