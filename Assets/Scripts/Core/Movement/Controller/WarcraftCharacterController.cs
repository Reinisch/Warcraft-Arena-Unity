using Bolt;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    internal class WarcraftCharacterController : EntityBehaviour<IUnitState>, IUnitBehaviour
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private PhysicsReference physics;
        [SerializeField, UsedImplicitly] private PlayerControllerDefinition controllerDefinition;
        [SerializeField, UsedImplicitly] private Rigidbody unitRigidbody;
        [SerializeField, UsedImplicitly] private GroundChecker groundChecker;

        private float groundCheckDistance = 0.2f;
        private Vector3 groundNormal = Vector3.up;
        private Vector3 inputVelocity = Vector3.zero;
        private IControllerInputProvider currentInputProvider;
        private IControllerInputProvider defaultInputProvider;
        private bool wasFlying;
        private Unit unit;

        private bool OnEdge => unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;

        internal BoltEntity ClientMoveState { get; private set; }

        internal IControllerInputProvider InputProvider
        {
            private get => currentInputProvider ?? defaultInputProvider;
            set => currentInputProvider = value;
        }

        internal PlayerControllerDefinition ControllerDefinition => controllerDefinition;

        public bool HasClientLogic => true;

        public bool HasServerLogic => true;

        void IUnitBehaviour.DoUpdate(int deltaTime)
        {
            if (unit.IsController)
            {
                InputProvider.PollInput(out inputVelocity, out var inputRotation, out var shouldJump);

                if (shouldJump && unit.IsMovementBlocked)
                    shouldJump = false;

                Vector3 rawInputVelocity = Vector3.zero;

                if (!unit.IsAlive)
                    inputVelocity = Vector3.zero;
                else if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                {
                    // check roots and apply final move speed
                    inputVelocity *= unit.IsMovementBlocked ? 0 : unit.GetSpeed(UnitMoveType.Run);

                    if (shouldJump)
                    {
                        unit.MovementInfo.Jumping = true;
                        inputVelocity = new Vector3(inputVelocity.x, controllerDefinition.JumpSpeed, inputVelocity.z);
                    }

                    rawInputVelocity = inputVelocity;
                    inputVelocity = transform.TransformDirection(inputVelocity);
                }
                else
                    inputVelocity = Vector3.zero;

                bool movingRight = rawInputVelocity.x > 0;
                bool movingLeft = rawInputVelocity.x < 0;
                bool moving = rawInputVelocity.magnitude > 0;

                if (movingRight)
                {
                    unit.MovementInfo.RemoveMovementFlag(MovementFlags.StrafeLeft);
                    unit.MovementInfo.AddMovementFlag(MovementFlags.StrafeRight);
                }
                else if (movingLeft)
                {
                    unit.MovementInfo.RemoveMovementFlag(MovementFlags.StrafeRight);
                    unit.MovementInfo.AddMovementFlag(MovementFlags.StrafeLeft);
                }
                else
                    unit.MovementInfo.RemoveMovementFlag(MovementFlags.StrafeRight | MovementFlags.StrafeLeft);

                if (moving)
                    unit.MovementInfo.AddMovementFlag(MovementFlags.Forward);
                else
                    unit.MovementInfo.RemoveMovementFlag(MovementFlags.Forward);

                if (unit.IsAlive)
                    transform.rotation = inputRotation;
            }
        }

        void IUnitBehaviour.HandleUnitAttach(Unit unit)
        {
            this.unit = unit;

            groundCheckDistance = controllerDefinition.BaseGroundCheckDistance;
            defaultInputProvider = new IdleControllerInputProvider(unit);

            UpdateOwnership();
        }

        void IUnitBehaviour.HandleUnitDetach()
        {
            unitRigidbody.isKinematic = true;
            unitRigidbody.useGravity = false;

            DetachClientSideMoveState(true);

            unit = null;
        }

        public override void SimulateOwner()
        {
            if (ClientMoveState != null)
            {
                unit.Position = ClientMoveState.transform.position;
                unit.Rotation = ClientMoveState.transform.rotation;
            }
        }

        public override void SimulateController()
        {
            ProcessMovement();

            if (ClientMoveState != null)
            {
                ClientMoveState.transform.position = unit.Position;
                ClientMoveState.transform.rotation = unit.Rotation;
            }
        }

        public override void ControlGained()
        {
            base.ControlGained();

            UpdateOwnership();

            if (!unit.IsOwner && unit.IsController)
            {
                BoltEntity localClientMoveState = BoltNetwork.Instantiate(BoltPrefabs.MoveState);
                localClientMoveState.SetScopeAll(false);
                localClientMoveState.SetScope(BoltNetwork.Server, true);
                localClientMoveState.AssignControl(BoltNetwork.Server);

                AttachClientSideMoveState(localClientMoveState);
            }
        }

        public override void ControlLost()
        {
            base.ControlLost();

            UpdateOwnership();

            DetachClientSideMoveState(true);
        }

        internal void AttachClientSideMoveState(BoltEntity moveEntity)
        {
            var localPlayerMoveState = moveEntity.GetState<IMoveState>();
            unit.MovementInfo.AttachedMoveState(localPlayerMoveState);
            localPlayerMoveState.SetTransforms(localPlayerMoveState.LocalTransform, moveEntity.transform);

            ClientMoveState = moveEntity;
        }

        internal void DetachClientSideMoveState(bool destroyObject)
        {
            BoltEntity moveStateEntity = ClientMoveState;
            if (moveStateEntity != null && destroyObject)
            {
                if (!moveStateEntity.IsOwner || !moveStateEntity.IsAttached)
                    Destroy(moveStateEntity.gameObject);
                else
                    BoltNetwork.Destroy(moveStateEntity.gameObject);
            }

            unit.MovementInfo.DetachedMoveState();
            ClientMoveState = null;
        }

        internal void StopMoving()
        {
            unitRigidbody.velocity = Vector3.zero;
        }

        private void ProcessMovement()
        {
            unit.UnitCollider.radius = 0.2f;

            if (unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && unitRigidbody.velocity.y <= 0)
            {
                unit.MovementInfo.RemoveMovementFlag(MovementFlags.Ascending);
                unit.MovementInfo.AddMovementFlag(MovementFlags.Descending);
            }

            if (unit.MovementInfo.Jumping)
            {
                unitRigidbody.velocity = inputVelocity;
                groundCheckDistance = 0.05f;
                unit.MovementInfo.AddMovementFlag(MovementFlags.Ascending);
                unit.MovementInfo.AddMovementFlag(MovementFlags.Flying);
                unit.MovementInfo.Jumping = false;
            }
            else if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
            {
                unitRigidbody.velocity = new Vector3(inputVelocity.x, unitRigidbody.velocity.y, inputVelocity.z);

                if (!wasFlying)
                    groundCheckDistance = controllerDefinition.BaseGroundCheckDistance;
            }
            else if (groundCheckDistance < controllerDefinition.BaseGroundCheckDistance)
                groundCheckDistance = unitRigidbody.velocity.y < 0 ? controllerDefinition.BaseGroundCheckDistance : groundCheckDistance + 0.01f;

            ProcessGroundState();

            void ProcessGroundState()
            {
                wasFlying = unit.MovementInfo.HasMovementFlag(MovementFlags.Flying);

                if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && IsTouchingGround(out RaycastHit hitInfo))
                {
                    var distanceToGround = hitInfo.distance;

                    if (distanceToGround > unit.UnitCollider.bounds.extents.y + groundCheckDistance)
                    {
                        if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && inputVelocity.y <= 0)
                        {
                            unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                            groundNormal = hitInfo.normal;
                        }
                        else
                        {
                            groundNormal = Vector3.up;
                            if (unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                            {
                                unit.MovementInfo.RemoveMovementFlag(MovementFlags.Flying);
                                unit.MovementInfo.RemoveMovementFlag(MovementFlags.Descending);
                            }
                        }
                    }
                    else
                    {
                        groundNormal = hitInfo.normal;
                        if (unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                        {
                            unit.MovementInfo.RemoveMovementFlag(MovementFlags.Flying);
                            unit.MovementInfo.RemoveMovementFlag(MovementFlags.Descending);
                        }
                    }
                }
                else
                {
                    unit.MovementInfo.AddMovementFlag(MovementFlags.Flying);
                    groundNormal = Vector3.up;
                    Mathf.Asin(Vector3.up.y);
                }

                if (TooSteep || OnEdge)
                    unit.UnitCollider.material = physics.SlidingMaterial;
                else
                    unit.UnitCollider.material = physics.GroundedMaterial;
            }
        }

        private bool IsTouchingGround(out RaycastHit groundHitInfo)
        {
            return Physics.Raycast(unit.UnitCollider.bounds.center, Vector3.down, out groundHitInfo, unit.UnitCollider.bounds.extents.y +
                controllerDefinition.BaseGroundCheckDistance * 2, PhysicsReference.Mask.Ground);
        }

        private void UpdateOwnership()
        {
            unitRigidbody.isKinematic = !unit.IsController;
            unitRigidbody.useGravity = unit.IsController;
        }
    }
}
