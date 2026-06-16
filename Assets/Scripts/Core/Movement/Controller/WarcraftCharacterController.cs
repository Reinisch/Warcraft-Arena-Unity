using Game.Core.CharacterController;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core
{
    public class WarcraftCharacterController : MonoBehaviour, IUnitBehaviour, ICharacterController
    {
        [Inject]
        private PhysicsReference physics;

        [Inject]
        private ControllerInputContainer controllerInputs;

        [SerializeField, UsedImplicitly] 
        private PlayerControllerDefinition controllerDefinition;

        [SerializeField, UsedImplicitly]
        private Rigidbody unitRigidbody;

        [SerializeField, UsedImplicitly]
        private KinematicCharacterMotor motor;

        [Header("Movement")]
        public float StableMovementSharpness = 15f;
        public float Gravity = -30f;
        public float MaxGravity = -300f;
        public List<Collider> IgnoredColliders = new();

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding;
        public float AirMovementSharpness = 5f;
        public float JumpPreGroundingGraceTime;
        public float JumpPostGroundingGraceTime;

        private bool jumpConsumed;
        private bool jumpedThisFrame;
        private bool jumpRequested;
        private float timeSinceJumpRequested = Mathf.Infinity;
        private float timeSinceLastAbleToJump;
        private Vector3 internalVelocityAdd = Vector3.zero;
        private Vector3 inputVelocity = Vector3.zero;
        private Vector3 rawInputVelocity = Vector3.zero;
        private Quaternion inputRotation = Quaternion.identity;
        private ControllerInputSettings currentInputProvider;
        private ControllerInputSettings activeInputProvider;
        private Unit unit;

        public KinematicCharacterMotor Motor => motor;
        public ControllerInputSettings InputProvider { set => currentInputProvider = value; }
        public PlayerControllerDefinition Definition => controllerDefinition;
        public Vector3 Velocity => motor.Velocity;

        private void Awake()
        {
            Motor.CharacterController = this;
        }

        void IUnitBehaviour.DoUpdate(int deltaTime)
        {
            UpdateInputMode();
        }

        void IUnitBehaviour.HandleUnitAttach(Unit unit)
        {
            this.unit = unit;

            Motor.SetPositionAndRotation(transform.position, transform.rotation);

            UpdateInputMode();
        }

        void IUnitBehaviour.HandleUnitDetach()
        {
            unitRigidbody.isKinematic = true;
            unitRigidbody.useGravity = false;

            unit = null;
        }

        /// <summary>
        /// Toggle local physics simulation. Disabled on remote/puppet units (driven purely by replicated
        /// transform): the motor unregisters from the simulation system in OnDisable, so it stops moving and
        /// no longer overwrites the transform we set from the network.
        /// </summary>
        public void SetSimulated(bool simulated)
        {
            if (motor == null)
                return;

            motor.enabled = simulated;

            // Re-enabling: the motor's internal position is stale (puppets set transform.position directly),
            // so sync it to the current transform to avoid snapping back to the pre-puppet position.
            if (simulated)
                motor.SetPositionAndRotation(transform.position, transform.rotation);
        }

        void ICharacterController.BeforeCharacterUpdate(float deltaTime)
        {
            rawInputVelocity = Vector3.zero;
            bool hasControl = unit.Motion.HasMovementControl;

            activeInputProvider.PollInput(unit, out inputVelocity, out inputRotation, out jumpRequested);
            inputVelocity.Normalize();

            if (hasControl)
            {
                if (inputVelocity.z < 0)
                    inputVelocity *= 0.3f;

                if (jumpRequested && unit.IsMovementBlocked)
                    jumpRequested = false;

                if (!unit.IsAlive)
                    inputVelocity = Vector3.zero;
                else
                {
                    if (unit.IsMovementBlocked)
                        inputVelocity = Vector3.zero;

                    if (jumpRequested && !unit.HasMovementFlag(MovementFlags.Flying))
                    {
                        timeSinceJumpRequested = 0f;

                        unit.Motion.Jumping = true;
                    }

                    rawInputVelocity = inputVelocity;
                    inputVelocity = transform.TransformDirection(inputVelocity);
                }
            }
            else
            {
                inputVelocity = Vector3.zero;
            }
        }

        void ICharacterController.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (!unit.IsAlive)
                return;

            if (unit.Motion.HasMovementControl)
            {
                currentRotation = inputRotation;
            }
            else if (unit.AI.NavMeshAgentEnabled)
            {
                // AI-driven movement (confusion/polymorph, charge): the KCC owns transform rotation, so face
                // the steering direction here. NavMeshAgent.updateRotation can't do it — the motor reapplies
                // its own rotation every step and overwrites the agent's.
                Vector3 direction = unit.AI.DesiredVelocity;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.0001f)
                    currentRotation = Quaternion.RotateTowards(currentRotation,
                        Quaternion.LookRotation(direction, Motor.CharacterUp), unit.AI.AngularSpeed * deltaTime);
            }
        }

        void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (Motor.GroundingStatus.IsStableOnGround)
                HandleGroundMovement(deltaTime, ref currentVelocity);
            else
                HandleAirMovement(deltaTime, ref currentVelocity);

            HandleJumping(deltaTime, ref currentVelocity);
            HandleMovementEffects(ref currentVelocity);
        }

        void ICharacterController.AfterCharacterUpdate(float deltaTime)
        {
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

            if (unit.Motion.Jumping && timeSinceJumpRequested > JumpPreGroundingGraceTime)
                unit.Motion.Jumping = false;

            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
            {
                if (!jumpedThisFrame)
                    jumpConsumed = false;

                timeSinceLastAbleToJump = 0f;
            }
            else
                timeSinceLastAbleToJump += deltaTime;

            unit.Motion.SimulateOwner();
        }

        void ICharacterController.PostGroundingUpdate(float deltaTime)
        {
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
                unit.UnitCollider.material = physics.GroundedMaterial;
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
                unit.UnitCollider.material = physics.SlidingMaterial;
        }

        bool ICharacterController.IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
                return true;

            if (IgnoredColliders.Contains(coll))
                return false;

            return true;
        }

        private void UpdateInputMode()
        {
            activeInputProvider = unit.Motion.HasMovementControl
                ? currentInputProvider ?? controllerInputs.Idle
                : controllerInputs.Idle;
        }

        private void HandleGroundMovement(float deltaTime, ref Vector3 currentVelocity)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;

            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

            // Reorient velocity on slope
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(inputVelocity, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * inputVelocity.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * unit.RunSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));

            unit.SetMovementFlag(MovementFlags.Ascending, false);
            unit.SetMovementFlag(MovementFlags.Descending, false);
            unit.SetMovementFlag(MovementFlags.Flying, false);
        }

        private void HandleAirMovement(float deltaTime, ref Vector3 currentVelocity)
        {
            if (inputVelocity.sqrMagnitude > 0f)
            {
                Vector3 targetMovementVelocity = inputVelocity * unit.RunSpeed;
                targetMovementVelocity.y = currentVelocity.y;
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-AirMovementSharpness * deltaTime));
            }

            currentVelocity.y += Gravity * deltaTime;

            if (currentVelocity.y < MaxGravity)
                currentVelocity.y = MaxGravity;

            unit.SetMovementFlag(MovementFlags.Ascending, currentVelocity.y > 0);
            unit.SetMovementFlag(MovementFlags.Descending, currentVelocity.y < 0);
        }

        private void HandleJumping(float deltaTime, ref Vector3 currentVelocity)
        {
            jumpedThisFrame = false;
            timeSinceJumpRequested += deltaTime;
            if (unit.Motion.Jumping)
            {
                // See if we actually are allowed to jump
                if (!jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = Motor.CharacterUp;
                    if (Motor.GroundingStatus is { FoundAnyGround: true, IsStableOnGround: false })
                        jumpDirection = Motor.GroundingStatus.GroundNormal;

                    unit.Motion.Jumping = false;

                    // Makes the character skip ground probing/snapping on its next update. 
                    // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                    Motor.ForceUnground();

                    unit.SetMovementFlag(MovementFlags.Ascending, true);
                    unit.SetMovementFlag(MovementFlags.Descending, false);
                    unit.SetMovementFlag(MovementFlags.Flying, true);

                    // Add to the return velocity and reset jump state
                    currentVelocity += (jumpDirection * controllerDefinition.JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                    jumpConsumed = true;
                    jumpedThisFrame = true;
                }
            }
        }

        private void HandleMovementEffects(ref Vector3 currentVelocity)
        {
            if (unit.SlowFallSpeed != 0 && unit.HasMovementFlag(MovementFlags.Flying) && currentVelocity.y < -unit.SlowFallSpeed)
                currentVelocity = new Vector3(currentVelocity.x, -unit.SlowFallSpeed, currentVelocity.z);

            if (internalVelocityAdd.sqrMagnitude > 0f)
            {
                currentVelocity += internalVelocityAdd;
                internalVelocityAdd = Vector3.zero;
            }
        }
    }
}
