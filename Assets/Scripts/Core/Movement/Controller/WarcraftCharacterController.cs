using Bolt;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    internal class WarcraftCharacterController : EntityBehaviour<IUnitState>, IUnitBehaviour
    {
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

        private bool OnEdge => unit.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;
        private bool IsMovementController => unit.Motion.HasMovementControl ? unit.IsController : unit.IsOwner;

        internal IControllerInputProvider InputProvider { set => currentInputProvider = value; }

        internal PlayerControllerDefinition Definition => controllerDefinition;

        public bool HasClientLogic => true;
        public bool HasServerLogic => true;

        void IUnitBehaviour.DoUpdate(int deltaTime)
        {
            if (IsMovementController)
            {
                IControllerInputProvider inputProvider = unit.Motion.HasMovementControl
                    ? currentInputProvider ?? defaultInputProvider
                    : defaultInputProvider;

                inputProvider.PollInput(unit, out inputVelocity, out var inputRotation, out var shouldJump);

                inputVelocity.Normalize();

                // slow down when moving backward
                if (inputVelocity.z < 0)
                    inputVelocity *= 0.3f;

                if (shouldJump && unit.IsMovementBlocked)
                    shouldJump = false;

                Vector3 rawInputVelocity = Vector3.zero;

                if (!unit.IsAlive)
                    inputVelocity = Vector3.zero;
                else if (!unit.HasMovementFlag(MovementFlags.Flying))
                {
                    // check roots and apply final move speed
                    inputVelocity *= unit.IsMovementBlocked ? 0 : unit.RunSpeed;

                    if (shouldJump)
                    {
                        unit.Motion.Jumping = true;
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
                    unit.SetMovementFlag(MovementFlags.StrafeLeft, false);
                    unit.SetMovementFlag(MovementFlags.StrafeRight, true);
                }
                else if (movingLeft)
                {
                    unit.SetMovementFlag(MovementFlags.StrafeRight, false);
                    unit.SetMovementFlag(MovementFlags.StrafeLeft, true);
                }
                else
                    unit.SetMovementFlag(MovementFlags.StrafeRight | MovementFlags.StrafeLeft, false);

                unit.SetMovementFlag(MovementFlags.Backward, rawInputVelocity.z < 0);
                unit.SetMovementFlag(MovementFlags.Forward, rawInputVelocity.z > 0);

                if (unit.IsAlive && unit.Motion.HasMovementControl)
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
            if (!unit.IsController && !unit.Motion.HasMovementControl || unit.IsController)
                ProcessMovement();

            unit.Motion.SimulateOwner();
        }

        public override void SimulateController()
        {
            unit.Motion.SimulateController();

            if (!unit.IsOwner && unit.Motion.HasMovementControl)
                ProcessMovement();
        }

        internal void UpdateMovementControl(bool hasMovementControl)
        {
            unit.Motion.UpdateMovementControl(hasMovementControl);
            unit.UpdateSyncTransform(unit.IsOwner || !hasMovementControl);

            UpdateRigidbody();

            if (unit.IsOwner && unit is Player player)
                EventHandler.ExecuteEvent(GameEvents.ServerPlayerMovementControlChanged, player, hasMovementControl);
        }

        internal void StopMoving()
        {
            unitRigidbody.velocity = Vector3.zero;
        }

        internal void UpdateRigidbody()
        {
            bool isMovingLocally = IsMovementController;
            bool isKinematic = !isMovingLocally || unit.AI.NavMeshAgentEnabled || unit.Motion.UsesKinematicMovement;
            unitRigidbody.isKinematic = isKinematic;
            unitRigidbody.useGravity = !isKinematic;
            unitRigidbody.interpolation = unit.IsOwner && unit is Player ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        private void ProcessMovement()
        {
            unit.UnitCollider.radius = 0.2f;
            hasGroundHit = IsTouchingGround(out lastGroundHitInfo);

            if (unit.HasMovementFlag(MovementFlags.Ascending) && unitRigidbody.velocity.y <= 0)
            {
                unit.SetMovementFlag(MovementFlags.Ascending, false);
                unit.SetMovementFlag(MovementFlags.Descending, true);
            }

            if (unit.Motion.Jumping)
            {
                unitRigidbody.velocity = inputVelocity;
                groundCheckDistance = 0.05f;
                unit.SetMovementFlag(MovementFlags.Ascending, true);
                unit.SetMovementFlag(MovementFlags.Flying, true);
                unit.Motion.Jumping = false;
            }
            else if (!unit.HasMovementFlag(MovementFlags.Flying))
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

            if (unit.SlowFallSpeed != 0 && unit.HasMovementFlag(MovementFlags.Flying) && unitRigidbody.velocity.y < -unit.SlowFallSpeed)
                unitRigidbody.velocity = new Vector3(unitRigidbody.velocity.x, -unit.SlowFallSpeed, unitRigidbody.velocity.z);

            void ProcessGroundState()
            {
                wasFlying = unit.HasMovementFlag(MovementFlags.Flying);

                if (!unit.HasMovementFlag(MovementFlags.Ascending) && hasGroundHit)
                {
                    var distanceToGround = lastGroundHitInfo.distance;

                    if (distanceToGround > unit.UnitCollider.bounds.extents.y + groundCheckDistance)
                    {
                        if (!unit.HasMovementFlag(MovementFlags.Flying) && inputVelocity.y <= 0)
                        {
                            unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                            groundNormal = lastGroundHitInfo.normal;
                        }
                        else
                        {
                            groundNormal = Vector3.up;
                            if (unit.HasMovementFlag(MovementFlags.Flying))
                            {
                                unit.SetMovementFlag(MovementFlags.Flying, false);
                                unit.SetMovementFlag(MovementFlags.Descending, false);
                            }
                        }
                    }
                    else
                    {
                        groundNormal = lastGroundHitInfo.normal;
                        if (unit.HasMovementFlag(MovementFlags.Flying))
                        {
                            unit.SetMovementFlag(MovementFlags.Flying, false);
                            unit.SetMovementFlag(MovementFlags.Descending, false);
                        }
                    }
                }
                else
                {
                    unit.SetMovementFlag(MovementFlags.Flying, true);
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
                    bool inNonGroundedState = !TouchingGround || unit.HasMovementFlag(MovementFlags.MaskAir);
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
