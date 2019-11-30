using Bolt;
using Common;
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

        private const float IdleGroundDistance = 0.05f;

        private float groundCheckDistance = 0.2f;
        private Vector3 groundNormal = Vector3.up;
        private Vector3 inputVelocity = Vector3.zero;
        private RaycastHit lastGroundHitInfo;

        private IControllerInputProvider currentInputProvider;
        private IControllerInputProvider defaultInputProvider;
        private bool wasFlying;
        private bool hasGroundHit;
        private Unit unit;

        private bool OnEdge => unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;
        private bool IsMovementController => unit.MovementInfo.HasMovementControl ? unit.IsController : unit.IsOwner;

        internal IControllerInputProvider InputProvider { set => currentInputProvider = value; }

        internal PlayerControllerDefinition Definition => controllerDefinition;

        public bool HasClientLogic => true;

        public bool HasServerLogic => true;

        void IUnitBehaviour.DoUpdate(int deltaTime)
        {
            if (IsMovementController)
            {
                IControllerInputProvider inputProvider = unit.MovementInfo.HasMovementControl
                    ? currentInputProvider ?? defaultInputProvider
                    : defaultInputProvider;

                inputProvider.PollInput(unit, out inputVelocity, out var inputRotation, out var shouldJump);

                if (shouldJump && unit.IsMovementBlocked)
                    shouldJump = false;

                Vector3 rawInputVelocity = Vector3.zero;

                if (!unit.IsAlive)
                    inputVelocity = Vector3.zero;
                else if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                {
                    // check roots and apply final move speed
                    inputVelocity *= unit.IsMovementBlocked ? 0 : unit.RunSpeed;

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

                if (rawInputVelocity.z < 0)
                    unit.MovementInfo.AddMovementFlag(MovementFlags.Backward);
                else
                    unit.MovementInfo.RemoveMovementFlag(MovementFlags.Backward);

                if (rawInputVelocity.z > 0)
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
            defaultInputProvider = new IdleControllerInputProvider();

            UpdateRigidbody();
        }

        void IUnitBehaviour.HandleUnitDetach()
        {
            unitRigidbody.isKinematic = true;
            unitRigidbody.useGravity = false;

            unit = null;
        }

        public override void SimulateOwner()
        {
            if (!unit.IsController && !unit.MovementInfo.HasMovementControl || unit.IsController)
                ProcessMovement();

            if (unit.MovementInfo.HasMovementControl && unit.MovementInfo.MoveEntity != null)
            {
                unit.Position = unit.MovementInfo.MoveEntity.transform.position;
                unit.Rotation = unit.MovementInfo.MoveEntity.transform.rotation;
            }

            if (unit.MovementInfo.IsMoving)
                unit.VisibilityChanged = true;
        }

        public override void SimulateController()
        {
            if (!unit.IsOwner && unit.MovementInfo.MoveEntity != null)
            {
                unit.MovementInfo.MoveEntity.transform.position = unit.Position;
                unit.MovementInfo.MoveEntity.transform.rotation = unit.Rotation;
            }

            if (!unit.IsOwner && unit.MovementInfo.HasMovementControl)
                ProcessMovement();
        }

        internal void UpdateMovementControl(bool hasMovementControl)
        {
            unit.MovementInfo.HasMovementControl = hasMovementControl;
            unit.UpdateSyncTransform(unit.IsOwner || !hasMovementControl);

            UpdateRigidbody();

            if (unit.IsOwner && unit is Player player)
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerMovementControlChanged, player, hasMovementControl);
        }

        internal void StopMoving()
        {
            unitRigidbody.velocity = Vector3.zero;
        }

        internal void UpdateRigidbody()
        {
            bool isMovingLocally = IsMovementController;
            unitRigidbody.isKinematic = !isMovingLocally;
            unitRigidbody.useGravity = isMovingLocally;
            unitRigidbody.interpolation = unit.IsOwner && unit is Player ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        private void ProcessMovement()
        {
            unit.UnitCollider.radius = 0.2f;
            hasGroundHit = IsTouchingGround(out lastGroundHitInfo);

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

            if (unitRigidbody.velocity.y > controllerDefinition.JumpSpeed)
                unitRigidbody.velocity = new Vector3(unitRigidbody.velocity.x, controllerDefinition.JumpSpeed, unitRigidbody.velocity.z);

            ProcessGroundState();

            void ProcessGroundState()
            {
                wasFlying = unit.MovementInfo.HasMovementFlag(MovementFlags.Flying);

                if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && hasGroundHit)
                {
                    var distanceToGround = lastGroundHitInfo.distance;

                    if (distanceToGround > unit.UnitCollider.bounds.extents.y + groundCheckDistance)
                    {
                        if (!unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && inputVelocity.y <= 0)
                        {
                            unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                            groundNormal = lastGroundHitInfo.normal;
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
                        groundNormal = lastGroundHitInfo.normal;
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
                {
                    unit.UnitCollider.material = physics.SlidingMaterial;
                    unitRigidbody.useGravity = true;
                }
                else
                {
                    unit.UnitCollider.material = physics.GroundedMaterial;

                    bool farFromGround = !hasGroundHit || Mathf.Abs(unit.UnitCollider.bounds.center.y - lastGroundHitInfo.point.y - unit.UnitCollider.bounds.extents.y) > IdleGroundDistance;
                    bool inNonGroundedState = !TouchingGround || unit.MovementInfo.HasMovementFlag(MovementFlags.MaskAir);
                    unitRigidbody.useGravity = farFromGround || inNonGroundedState;
                }
            }
        }

        private bool IsTouchingGround(out RaycastHit groundHitInfo)
        {
            return Physics.Raycast(unit.UnitCollider.bounds.center, Vector3.down, out groundHitInfo, unit.UnitCollider.bounds.extents.y +
                controllerDefinition.BaseGroundCheckDistance * 2, PhysicsReference.Mask.Ground);
        }
    }
}
