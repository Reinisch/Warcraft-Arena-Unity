using Bolt;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class WarcraftController : EntityBehaviour<IPlayerState>
    {
        [SerializeField, UsedImplicitly]
        private PlayerControllerDefinition controllerDefinition;
        [SerializeField, UsedImplicitly]
        private Unit unit;
        [SerializeField, UsedImplicitly]
        private Rigidbody unitRigidbody;
        [SerializeField, UsedImplicitly]
        private GroundChecker groundChecker;

        private float groundCheckDistance = 0.2f;

        private Vector3 groundNormal = Vector3.up;
        private Vector3 inputVelocity = Vector3.zero;
        private Vector3 hostPosition = Vector3.zero;
        private Vector3 hostVelocity = Vector3.zero;
        private Quaternion lastRotation;
        private BoltEntity clientMoveState;
        private bool wasFlying;

        private Unit Unit => unit;
        private bool IsRemote => Unit.IsController && !Unit.IsOwner;
        private bool OnEdge => Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;

        [UsedImplicitly]
        private void Awake()
        {
            groundCheckDistance = controllerDefinition.BaseGroundCheckDistance;
        }

        [UsedImplicitly]
        private void Update()
        {
            if (Unit.IsController)
            {
                ApplyControllerInputVelocity();
                ApplyControllerInputRotation();
            }
        }

        public override void Attached()
        {
            UpdateOwnership();
        }

        public override void Detached()
        {
            unitRigidbody.isKinematic = true;
            unitRigidbody.useGravity = false;

            DetachClientSideMovementState(true);
        }

        public override void SimulateOwner()
        {
            if (!Unit.IsController && BalanceManager.NetworkMovementType == NetworkMovementType.ServerSide)
                ProcessMovement();

            if (BalanceManager.NetworkMovementType == NetworkMovementType.ClientSide && clientMoveState != null)
            {
                Unit.Position = clientMoveState.transform.position;
                Unit.Rotation = clientMoveState.transform.rotation;
            }
        }

        public override void SimulateController()
        {
            ProcessMovement();

            if (BalanceManager.NetworkMovementType == NetworkMovementType.ServerSide && IsRemote)
            {
                ProcessMovementCorrection();

                var moveCommand = PlayerControllerMoveCommand.Create();
                moveCommand.InputVector = inputVelocity;
                moveCommand.Rotation = lastRotation;
                moveCommand.Position = Unit.Position;
                entity.QueueInput(moveCommand);
            }

            if (clientMoveState != null)
            {
                clientMoveState.transform.position = Unit.Position;
                clientMoveState.transform.rotation = Unit.Rotation;
            }
        }

        public override void ControlGained()
        {
            base.ControlGained();

            UpdateOwnership();

            if (!Unit.IsOwner && Unit.IsController && BalanceManager.NetworkMovementType == NetworkMovementType.ClientSide)
            {
                BoltEntity localClientMoveState = BoltNetwork.Instantiate(BoltPrefabs.PlayerMoveState);
                localClientMoveState.SetScope(BoltNetwork.server, true);
                localClientMoveState.AssignControl(BoltNetwork.server);

                AttachClientSideMoveState(localClientMoveState);
            }
        }

        public override void ControlLost()
        {
            base.ControlLost();

            UpdateOwnership();

            DetachClientSideMovementState(true);
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            if (BalanceManager.NetworkMovementType == NetworkMovementType.ClientSide)
                return;

            var moveCommand = (PlayerControllerMoveCommand) command;
            if (resetState)
            {
                hostPosition = moveCommand.Result.Position;
                hostVelocity = moveCommand.Result.Velocity;
            }
            if (Unit.IsOwner)
            {
                Unit.Rotation = moveCommand.Input.Rotation;
               
                if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
                {
                    if (moveCommand.Input.InputVector.y > 0)
                    {
                        Unit.MovementInfo.Jump.SpeedXZ = inputVelocity.magnitude;
                        Unit.MovementInfo.Jump.SpeedY = controllerDefinition.JumpSpeed;
                        Unit.MovementInfo.AddMovementFlag(MovementFlags.Flying);
                        Unit.MovementInfo.AddMovementFlag(MovementFlags.Ascending);
                        unitRigidbody.velocity = inputVelocity;
                    }

                    inputVelocity = moveCommand.Input.InputVector;
                }

                moveCommand.Result.Position = Unit.Position;
                moveCommand.Result.Velocity = unitRigidbody.velocity;
            }
        }

        public void AttachClientSideMoveState(BoltEntity moveEntity)
        {
            var localPlayerMoveState = moveEntity.GetState<ILocalPlayerMovementState>();
            localPlayerMoveState.SetTransforms(localPlayerMoveState.LocalTransform, moveEntity.transform);

            clientMoveState = moveEntity;
        }

        public void DetachClientSideMovementState(bool destroyObject)
        {
            BoltEntity moveStateEntity = clientMoveState;
            if (moveStateEntity != null && destroyObject)
            {
                if (!moveStateEntity.isOwner || !moveStateEntity.isAttached)
                    Destroy(moveStateEntity.gameObject);
                else
                    BoltNetwork.Destroy(moveStateEntity.gameObject);
            }

            clientMoveState = null;
        }

        private void ApplyControllerInputVelocity()
        {
            Vector3 rawInputVelocity = Vector3.zero;

            if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
            {
                inputVelocity = new Vector3(Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0, 0, Input.GetAxis("Vertical"));

                if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                    inputVelocity = new Vector3(inputVelocity.x, inputVelocity.y, inputVelocity.z + 1);

                if (inputVelocity.z > 1)
                    inputVelocity.z = 1;

                inputVelocity = new Vector3(inputVelocity.x - Input.GetAxis("Strafing"), inputVelocity.y, inputVelocity.z);

                // if moving forward and to the side at the same time, compensate for distance
                if (Input.GetMouseButton(1) && !Mathf.Approximately(Input.GetAxis("Horizontal"), 0) && !Mathf.Approximately(Input.GetAxis("Vertical"), 0))
                {
                    inputVelocity *= 0.7f;
                }

                // check roots and apply final move speed
                inputVelocity *= Unit.IsMovementBlocked ? 0 : Unit.GetSpeed(UnitMoveType.Run);

                if (Input.GetButton("Jump"))
                {
                    Unit.MovementInfo.Jump.SpeedXZ = inputVelocity.magnitude;
                    Unit.MovementInfo.Jump.SpeedY = controllerDefinition.JumpSpeed;
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
        }

        private void ApplyControllerInputRotation()
        {
            if (Input.GetMouseButton(1))
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            else
                transform.Rotate(0, Input.GetAxis("Horizontal") * controllerDefinition.RotateSpeed * Time.deltaTime, 0);

            lastRotation = transform.rotation;
        }

        private void ProcessGroundState()
        {
            RaycastHit hitInfo;
            wasFlying = Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying);

            if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && IsTouchingGround(out hitInfo))
            {
                var distanceToGround = hitInfo.distance;

                if (distanceToGround > Unit.UnitCollider.bounds.extents.y + groundCheckDistance)
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
                Unit.UnitCollider.material = PhysicsManager.SlidingMaterial;
            else
                Unit.UnitCollider.material = PhysicsManager.GroundedMaterial;
        }

        private void ProcessMovement()
        {
            Unit.UnitCollider.radius = 0.2f;

            if (Unit.MovementInfo.HasMovementFlag(MovementFlags.Ascending) && unitRigidbody.velocity.y <= 0)
            {
                Unit.MovementInfo.Jump.FallTime = TimeHelper.NowInMilliseconds;
                Unit.MovementInfo.RemoveMovementFlag(MovementFlags.Ascending);
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Descending);
            }

            if (Unit.MovementInfo.Jump.SpeedY > 0)
            {
                unitRigidbody.velocity = inputVelocity;
                groundCheckDistance = 0.05f;
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Ascending);
                Unit.MovementInfo.AddMovementFlag(MovementFlags.Flying);
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

            ProcessGroundState();
        }

        private void ProcessMovementCorrection()
        {
            Debug.DrawLine(Unit.Position + Vector3.up, hostPosition + Vector3.up, Color.red, 0.1f);
            if(hostPosition == Vector3.zero)
                return;

            float distanceDifference = Vector3.Distance(Unit.Position, hostPosition);
            if (distanceDifference > controllerDefinition.MovementCorrectionDistance)
                unitRigidbody.AddForce((hostPosition - Unit.Position) * controllerDefinition.CorrectionDampening, ForceMode.VelocityChange);

            if (Input.GetKey(KeyCode.V))
            {
                unitRigidbody.velocity = hostVelocity;
                unitRigidbody.position = hostPosition;
            }
        }

        private bool IsTouchingGround(out RaycastHit hitInfo)
        {
            return Physics.Raycast(Unit.UnitCollider.bounds.center, Vector3.down, out hitInfo, Unit.UnitCollider.bounds.extents.y +
                controllerDefinition.BaseGroundCheckDistance * 2, PhysicsManager.Mask.Ground);
        }

        private void UpdateOwnership()
        {
            switch (BalanceManager.NetworkMovementType)
            {
                case NetworkMovementType.ClientSide:
                    unitRigidbody.isKinematic = !Unit.IsController;
                    unitRigidbody.useGravity = Unit.IsController;
                    break;
                case NetworkMovementType.ServerSide:
                    bool hasLocalMovement = Unit.IsOwner || Unit.IsController;
                    unitRigidbody.isKinematic = !hasLocalMovement;
                    unitRigidbody.useGravity = hasLocalMovement;
                    break;
            }
        }
    }
}
