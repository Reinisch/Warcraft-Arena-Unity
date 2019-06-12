using Bolt;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    internal class WarcraftController : EntityBehaviour<IPlayerState>
    {
        [SerializeField, UsedImplicitly]
        private BalanceReference balance;
        [SerializeField, UsedImplicitly]
        private PhysicsReference physics;

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
        private bool wasFlying;

        private Unit Unit => unit;
        private bool IsRemote => Unit.IsController && !Unit.IsOwner;
        private bool OnEdge => Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;

        internal BoltEntity ClientMoveState { get; private set; }

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

            DetachClientSideMoveState(true);
        }

        public override void SimulateOwner()
        {
            if (!Unit.IsController && balance.NetworkMovementType == NetworkMovementType.ServerSide)
                ProcessMovement();

            if (balance.NetworkMovementType == NetworkMovementType.ClientSide && ClientMoveState != null)
            {
                Unit.Position = ClientMoveState.transform.position;
                Unit.Rotation = ClientMoveState.transform.rotation;
            }
        }

        public override void SimulateController()
        {
            ProcessMovement();

            if (balance.NetworkMovementType == NetworkMovementType.ServerSide && IsRemote)
            {
                ProcessMovementCorrection();

                var moveCommand = PlayerMoveCommand.Create();
                moveCommand.InputVector = inputVelocity;
                moveCommand.Rotation = lastRotation;
                moveCommand.Position = Unit.Position;
                entity.QueueInput(moveCommand);
            }

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

            if (!Unit.IsOwner && Unit.IsController && balance.NetworkMovementType == NetworkMovementType.ClientSide)
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

        public override void ExecuteCommand(Command command, bool resetState)
        {
            if (balance.NetworkMovementType == NetworkMovementType.ClientSide)
                return;

            var moveCommand = (PlayerMoveCommand) command;
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
                        Unit.MovementInfo.Jumping = true;
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
                if (!moveStateEntity.isOwner || !moveStateEntity.isAttached)
                    Destroy(moveStateEntity.gameObject);
                else
                    BoltNetwork.Destroy(moveStateEntity.gameObject);
            }

            unit.MovementInfo.DetachedMoveState();
            ClientMoveState = null;
        }

        private void ApplyControllerInputVelocity()
        {
            Vector3 rawInputVelocity = Vector3.zero;

            if (!Unit.IsAlive)
                inputVelocity = Vector3.zero;
            else if (!Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying))
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
        }

        private void ApplyControllerInputRotation()
        {
            if(!Unit.IsAlive)
                return;

            if (Input.GetMouseButton(1))
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            else
                transform.Rotate(0, Input.GetAxis("Horizontal") * controllerDefinition.RotateSpeed * Time.deltaTime, 0);

            lastRotation = transform.rotation;
        }

        private void ProcessGroundState()
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
            switch (balance.NetworkMovementType)
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
