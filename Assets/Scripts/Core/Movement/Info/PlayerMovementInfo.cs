using UnityEngine;

namespace Core
{
    internal class PlayerMovementInfo : MovementInfo
    {
        private IMoveState clientMoveState;
        private BoltEntity clientMoveEntity;

        internal override BoltEntity MoveEntity => clientMoveEntity;

        public PlayerMovementInfo(Unit unit, IUnitState unitState) : base(unit, unitState)
        {

        }

        public override void Dispose()
        {
            DetachMoveState(true);

            base.Dispose();
        }

        public void AttachMoveState(BoltEntity moveEntity)
        {
            clientMoveEntity = moveEntity;
            clientMoveState = moveEntity.GetState<IMoveState>();
            clientMoveState.SetTransforms(clientMoveState.LocalTransform, moveEntity.transform);

            if (Unit.IsOwner)
                clientMoveState.AddCallback(nameof(IUnitState.MovementFlags), OnLocalMoveStateFlagsChanged);
        }

        public void DetachMoveState(bool destroyObject)
        {
            if (clientMoveEntity == null)
                return;

            if (Unit.IsOwner)
                clientMoveState.RemoveCallback(nameof(IUnitState.MovementFlags), OnLocalMoveStateFlagsChanged);

            if (destroyObject)
            {
                if (!clientMoveEntity.IsOwner || !clientMoveEntity.IsAttached)
                    Object.Destroy(clientMoveEntity.gameObject);
                else
                    BoltNetwork.Destroy(clientMoveEntity.gameObject);
            }
            
            clientMoveEntity = null;
            clientMoveState = null;
        }

        protected override void UpdateMovementState()
        {
            base.UpdateMovementState();

            if (clientMoveState != null && !Unit.World.HasServerLogic && HasMovementControl)
                clientMoveState.MovementFlags = (int)Flags;
        }

        private void OnLocalMoveStateFlagsChanged()
        {
            if (HasMovementControl)
                SetFlags((MovementFlags)clientMoveState.MovementFlags);
        }
    }
}