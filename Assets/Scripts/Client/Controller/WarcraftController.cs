using System;
using Core;
using UnityEngine;

namespace Client
{
    public class WarcraftController : MonoBehaviour
    {
        [SerializeField]
        private bool isPlayerControlled;
        [SerializeField]
        private float jumpSpeed = 4.0f;
        [SerializeField]
        private float rotateSpeed = 250.0f;
        [SerializeField]
        private float baseGroundCheckDistance = 0.2f;
        [SerializeField]
        private CapsuleCollider unitCollider;

        private float groundCheckDistance = 0.2f;

        private Vector3 groundNormal = Vector3.up;
        private Vector3 inputVelocity = Vector3.zero;

        private Rigidbody unitRigidbody;
        private GroundChecker groundChecker;

        private bool wasFlying;

        private Unit Unit { get; set; }
        private bool OnEdge => Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying) && TouchingGround;
        private bool TooSteep => groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad);
        private bool TouchingGround => groundChecker.GroundCollisions > 0;

        protected void Awake()
        {
            unitRigidbody = GetComponent<Rigidbody>();
            groundChecker = GetComponentInChildren<GroundChecker>();

            groundCheckDistance = baseGroundCheckDistance;
        }

        protected void Update()
        {
            if (Unit == null)
                return;

            if (isPlayerControlled)
            {
                // Only allow movement and jumps while grounded
                ApplyInputVelocity();

                // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
                ApplyInputRotation();
            }
        }

        protected void FixedUpdate()
        {
            if (Unit == null)
                return;

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
                    groundCheckDistance = baseGroundCheckDistance;
            }
            else if (groundCheckDistance < baseGroundCheckDistance)
                groundCheckDistance = unitRigidbody.velocity.y < 0 ? baseGroundCheckDistance : groundCheckDistance + 0.01f;

            CheckGroundStatus();
        }

        public void Initialize(Unit unit)
        {
            Unit = unit;
        }

        public void Deinitialize()
        {
            Unit = null;
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
                    inputVelocity.z += 1;

                if (inputVelocity.z > 1)
                    inputVelocity.z = 1;

                //Strafing move (like Q/E movement    
                inputVelocity.x -= Input.GetAxis("Strafing");

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
                    Unit.MovementInfo.Jump.SpeedY = jumpSpeed;
                    inputVelocity.y = jumpSpeed;
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
            {
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                //transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            }
            else
            {
                transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            }
        }

        private void CheckGroundStatus()
        {
            wasFlying = Unit.MovementInfo.HasMovementFlag(MovementFlags.Flying);
            RaycastHit hitInfo;

            if (Physics.Raycast(unitCollider.bounds.center, Vector3.down, out hitInfo, unitCollider.bounds.extents.y +
                baseGroundCheckDistance * 2, PhysicsManager.Mask.Ground))
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
    }
}
