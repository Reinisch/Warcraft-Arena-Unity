using Bolt;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    internal class WarcraftController : EntityBehaviour<IUnitState>
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private PhysicsReference physics;
        [SerializeField, UsedImplicitly] private PlayerControllerDefinition controllerDefinition;
        [SerializeField, UsedImplicitly] private Rigidbody unitRigidbody;
        [SerializeField, UsedImplicitly] private GroundChecker groundChecker;
        [SerializeField, UsedImplicitly] private Unit unit;

        private float groundCheckDistance = 0.2f;
        private Vector3 groundNormal = Vector3.up;
        private Vector3 inputVelocity = Vector3.zero;
        private IControllerInputProvider currentInputProvider;
        private IControllerInputProvider defaultInputProvider;
        private bool wasFlying;

        private Unit Unit => unit;
        private bool OnEdge => Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;

        internal BoltEntity ClientMoveState { get; private set; }

        internal IControllerInputProvider InputProvider
        {
            private get => currentInputProvider ?? defaultInputProvider;
            set => currentInputProvider = value;
        }

        internal PlayerControllerDefinition ControllerDefinition => controllerDefinition;

        [UsedImplicitly]
        private void Awake()
        {
            defaultInputProvider = new IdleControllerInputProvider(unit);
        }

        internal void DoUpdate()
        {
            if (Unit.IsController)
            {
                InputProvider.PollInput(out inputVelocity, out var inputRotation, out var shouldJump);

                Vector3 rawInputVelocity = Vector3.zero;

                if (!Unit.IsAlive)
                    inputVelocity = Vector3.zero;
                else if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                {
                    // check roots and apply final move speed
                    inputVelocity *= Unit.IsMovementBlocked ? 0 : Unit.GetSpeed(UnitMoveType.Run);

                    if (shouldJump)
                    {
                        Unit.MovementInfo.Jumping = true;
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
                    Unit.MovementInfo.RemoveMovementFlag(MovementFlags.StrafeLeft);
                    Unit.MovementInfo.AddMovementFlag(MovementFlags.StrafeRight);
                }
                else if (movingLeft)
                {
                    Unit.MovementInfo.RemoveMovementFlag(MovementFlags.StrafeRight);
                    Unit.MovementInfo.AddMovementFlag(MovementFlags.StrafeLeft);
                }
                else
                    Unit.MovementInfo.RemoveMovementFlag(MovementFlags.StrafeRight | MovementFlags.StrafeLeft);

                if (moving)
                    Unit.MovementInfo.AddMovementFlag(MovementFlags.Forward);
                else
                    Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Forward);

                if (Unit.IsAlive)
                    transform.rotation = inputRotation;
            }
        }

        public override void Attached()
        {
            groundCheckDistance = controllerDefinition.BaseGroundCheckDistance;

            UpdateOwnership();
        }

        public override void Detached()
        {
            unitRigidbody.isKinematic = true;
            unitRigidbody.useGravity = false;

            DetachClientSideMoveState(true);
        }

        public override void SimulateOwner()
        {
            if (ClientMoveState != null)
            {
                Unit.Position = ClientMoveState.transform.position;
                Unit.Rotation = ClientMoveState.transform.rotation;
            }
        }

        public override void SimulateController()
        {
            ProcessMovement();

            if (ClientMoveState != null)
            {
                ClientMoveState.transform.position = Unit.Position;
                ClientMoveState.transform.rotation = Unit.Rotation;
            }
        }

        public override void ControlGained()
        {
            base.ControlGained();

            UpdateOwnership();

            if (!Unit.IsOwner && Unit.IsController)
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

        private void ProcessMovement()
        {
            Unit.UnitCollider.radius = 0.2f;

            if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && unitRigidbody.velocity.y <= 0)
            {
                Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Ascending);
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Descending);
            }

            if (Unit.MovementInfo.Jumping)
            {
                unitRigidbody.velocity = inputVelocity;
                groundCheckDistance = 0.05f;
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Ascending);
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Flying);
                Unit.MovementInfo.Jumping = false;
            }
            else if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
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
                wasFlying = Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying);

                if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && IsTouchingGround(out RaycastHit hitInfo))
                {
                    var distanceToGround = hitInfo.distance;

                    if (distanceToGround > Unit.UnitCollider.bounds.extents.y + groundCheckDistance)
                    {
                        if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && inputVelocity.y <= 0)
                        {
                            unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                            Mathf.Asin(hitInfo.normal.y);
                            groundNormal = hitInfo.normal;
                        }
                        else
                        {
                            groundNormal = Vector3.up;
                            Mathf.Asin(Vector3.up.y);

                            if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                            {
                                Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Flying);
                                Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Descending);
                            }
                        }
                    }
                    else
                    {
                        groundNormal = hitInfo.normal;
                        Mathf.Asin(hitInfo.normal.y);

                        if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                        {
                            Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Flying);
                            Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Descending);
                        }
                    }
                }
                else
                {
                    Unit.MovementInfo.AddMovementFlag(MovementFlags.Flying);
                    groundNormal = Vector3.up;
                    Mathf.Asin(Vector3.up.y);
                }

                if (TooSteep || OnEdge)
                    Unit.UnitCollider.material = physics.SlidingMaterial;
                else
                    Unit.UnitCollider.material = physics.GroundedMaterial;
            }
        }

        private bool IsTouchingGround(out RaycastHit groundHitInfo)
        {
            return Physics.Raycast(Unit.UnitCollider.bounds.center, Vector3.down, out groundHitInfo, Unit.UnitCollider.bounds.extents.y +
                controllerDefinition.BaseGroundCheckDistance * 2, PhysicsReference.Mask.Ground);
        }

        private void UpdateOwnership()
        {
            unitRigidbody.isKinematic = !Unit.IsController;
            unitRigidbody.useGravity = Unit.IsController;
        }
    }
}
