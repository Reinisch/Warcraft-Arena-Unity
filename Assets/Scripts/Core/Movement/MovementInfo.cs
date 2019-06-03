namespace Core
{
    public class MovementInfo
    {
        private const string MovementFlagsUnitStatePropertyName = "MovementFlags";

        private IUnitState unitState;
        private IMoveState localMoveState;
        private Unit unit;

        private MovementFlags Flags { get; set; }

        public bool Jumping { get; set; }

        public void Attached(IUnitState unitState, Unit unit)
        {
            this.unitState = unitState;
            this.unit = unit;

            if (!unit.WorldManager.HasServerLogic)
                unitState.AddCallback(MovementFlagsUnitStatePropertyName, OnUnitStateFlagsChanged);
        }

        public void Detached()
        {
            if (!unit.WorldManager.HasServerLogic)
                unitState.RemoveCallback(MovementFlagsUnitStatePropertyName, OnUnitStateFlagsChanged);

            DetachedMoveState();

            unitState = null;
            unit = null;
        }

        public void AttachedMoveState(IMoveState localMoveState)
        {
            this.localMoveState = localMoveState;

            if (unit.WorldManager.HasServerLogic)
                localMoveState.AddCallback(MovementFlagsUnitStatePropertyName, OnLocalMoveStateFlagsChanged);
        }

        public void DetachedMoveState()
        {
            if (localMoveState != null)
            {
                if (unit.WorldManager.HasServerLogic)
                    localMoveState.RemoveCallback(MovementFlagsUnitStatePropertyName, OnLocalMoveStateFlagsChanged);

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