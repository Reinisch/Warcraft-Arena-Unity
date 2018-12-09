using System;
using Bolt;
using Core;
using UnityEngine;

namespace Client
{
    public class WarcraftController : EntityBehaviour<IPlayerState>
    {
        [SerializeField]
        private PlayerControllerDefinition controllerDefinition;
        [SerializeField]
        private Unit unit;
        [SerializeField]
        private CapsuleCollider unitCollider;

        private float groundCheckDistance = 0.2f;

        private Vector3 groundNormal = Vector3.up;
        private Vector3 inputVelocity = Vector3.zero;
        private Vector3 hostPosition = Vector3.zero;

        private Rigidbody unitRigidbody;
        private GroundChecker groundChecker;
        private Quaternion lastRotation;
        private bool wasFlying;

        private Unit Unit => unit;
        private bool IsRemote => Unit.IsController && !Unit.IsOwner;
        private bool OnEdge => Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;

        protected void Awake()
        {
            unitRigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponentInChildren<GroundChecker>();

            groundCheckDistance = controllerDefinition.BaseGroundCheckDistance;
        }

        protected void Update()
        {
            if (Unit.IsController)
            {
                // Only allow movement and jumps while grounded
                ApplyInputVelocity();

                // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
                ApplyInputRotation();
            }
        }

        protected void FixedUpdate()
        {
            if (IsRemote)
            {
                ProcessMovement();

                if (hostPosition != Vector3.zero && Vector3.Distance(hostPosition, Unit.Position) > controllerDefinition.MovementCorrectionDistance)
                    unitRigidbody.AddForce((hostPosition - Unit.Position) * controllerDefinition.CorrectionDampening, ForceMode.VelocityChange);
            }
        }

        public override void Attached()
        {
        }

        public override void Detached()
        {
        }

        public override void SimulateOwner()
        {
           ProcessMovement();
        }

        public override void SimulateController()
        {
            if (IsRemote)
            {
                var moveCommand = PlayerControllerMoveCommand.Create();
                moveCommand.InputVector = inputVelocity;
                moveCommand.Rotation = lastRotation;
                entity.QueueInput(moveCommand);
            }
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            var moveCommand = (PlayerControllerMoveCommand) command;
            if (resetState)
            {
                hostPosition = moveCommand.Result.Position;
            }
            else if (Unit.IsOwner)
            {
                {
                    Unit.Rotation = moveCommand.Input.Rotation;
                    inputVelocity = moveCommand.Input.InputVector;

                    moveCommand.Result.Position = Unit.Position;
                }
            }
        }

        private void ApplyInputVelocity()
        {
            Vector3 rawInputVelocity = Vector3.zero;

            if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
            {
                //movedirection
                inputVelocity = new Vector3(Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0, 0, Input.GetAxis("Vertical"));

                //L+R MouseButton Movement
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                    inputVelocity = new Vector3(inputVelocity.x, inputVelocity.y, inputVelocity.z + 1);

                if (inputVelocity.z > 1)
                    inputVelocity = new Vector3(inputVelocity.x, inputVelocity.y, 1);

                //Strafing move (like Q/E movement    
                inputVelocity = new Vector3(inputVelocity.x - Input.GetAxis("Strafing"), inputVelocity.y, inputVelocity.z);

                // if moving forward and to the side at the same time, compensate for distance
                if (Input.GetMouseButton(1) && !Mathf.Approximately(Input.GetAxis("Horizontal"), 0) && !Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                {
                    inputVelocity *= 0.7f;
                }

                // Check roots and apply final move speed
                inputVelocity *= Unit.IsMovementBlocked ? 0 : Unit.GetSpeed(UnitMoveType.Run);

                // Jump!
                if (Input.GetButton("Jump"))
                {
                    Unit.MovementInfo.Jump.SpeedXZ = inputVelocity.magnitude;
                    Unit.MovementInfo.Jump.SpeedY = controllerDefinition.JumpSpeed;
                    inputVelocity = new Vector3(inputVelocity.x, controllerDefinition.JumpSpeed, inputVelocity.z);
                }

                //transform direction
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
        }

        private void ApplyInputRotation()
        {
            if (Input.GetMouseButton(1))
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            else
                transform.Rotate(0, Input.GetAxis("Horizontal") * controllerDefinition.RotateSpeed * Time.deltaTime, 0);

            lastRotation = transform.rotation;
        }

        private void CheckGroundStatus()
        {
            wasFlying = Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying);
            RaycastHit hitInfo;

            if (Physics.Raycast(unitCollider.bounds.center, Vector3.down, out hitInfo, unitCollider.bounds.extents.y +
                controllerDefinition.BaseGroundCheckDistance * 2, PhysicsManager.Mask.Ground))
            {
                var distanceToGround = hitInfo.distance;

                if (distanceToGround > unitCollider.bounds.extents.y + groundCheckDistance)
                {
                    if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && inputVelocity.y <= 0)
                    {
                        unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                        Unit.MovementInfo.Pitch = Mathf.Asin(hitInfo.normal.y);
                        groundNormal = hitInfo.normal;
                    }
                    else
                    {
                        groundNormal = Vector3.up;
                        Unit.MovementInfo.Pitch = Mathf.Asin(Vector3.up.y);

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
                    Unit.MovementInfo.Pitch = Mathf.Asin(hitInfo.normal.y);

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
                Unit.MovementInfo.Pitch = Mathf.Asin(Vector3.up.y);
            }

            if (TooSteep || OnEdge)
                unitCollider.material = PhysicsManager.SlidingMaterial;
            else
                unitCollider.material = PhysicsManager.GroundedMaterial;
        }

        private void ProcessMovement()
        {
            unitCollider.radius = 0.2f;

            if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && unitRigidbody.velocity.y <= 0)
            {
                Unit.MovementInfo.SetFallTime(DateTime.Now.Ticks);
                Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Ascending);
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Descending);
            }

            if (Unit.MovementInfo.Jump.SpeedY > 0)
            {
                unitRigidbody.velocity = inputVelocity;
                groundCheckDistance = 0.05f;
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Ascending);
                Unit.MovementInfo.Jump.Reset();
            }
            else if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
            {
                unitRigidbody.velocity = new Vector3(inputVelocity.x, unitRigidbody.velocity.y, inputVelocity.z);

                if (!wasFlying)
                    groundCheckDistance = controllerDefinition.BaseGroundCheckDistance;
            }
            else if (groundCheckDistance < controllerDefinition.BaseGroundCheckDistance)
                groundCheckDistance = unitRigidbody.velocity.y < 0 ? controllerDefinition.BaseGroundCheckDistance : groundCheckDistance + 0.01f;

            CheckGroundStatus();
        }
    }
}
