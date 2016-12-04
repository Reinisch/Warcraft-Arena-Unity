using UnityEngine;

public enum MovementStatus
{
    Stand, Run, StrafeLeft, StrafeRight, Jumping
}

public class WarcraftController : MonoBehaviour
{
    [SerializeField]
    float jumpSpeed = 4.0f;
    [SerializeField]
    float rotateSpeed = 250.0f;
    [SerializeField]
    float baseGroundCheckDistance = 0.2f;
    float groundCheckDistance = 0.2f;

    Vector3 groundNormal = Vector3.up;
    Vector3 inputVelocity = Vector3.zero;

    Unit controlledUnit;
    Rigidbody unitRigidbody;
    CapsuleCollider unitCollider;
    GroundChecker groundChecker;

    MovementStatus movementStatus;
    bool jumping = false;
    bool grounded = false;
    bool wasGrounded = false;


    public bool OnEdge
    {
        get
        {
            return !grounded && TouchingGround;
        }
    }

    public bool TooSteep
    {
        get
        {
            return (groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad));
        }
    }

    public bool OnSlope
    {
        get
        {
            return (groundNormal.y >= Mathf.Cos(5 * Mathf.Deg2Rad));
        }
    }

    public bool TouchingGround
    {
        get
        {
            return groundChecker.GroundCollisions > 0;
        }
    }


    void Awake()
    {
        unitRigidbody = GetComponent<Rigidbody>();
        controlledUnit = GetComponent<Unit>();
        unitCollider = GetComponent<CapsuleCollider>();
        groundChecker = GetComponentInChildren<GroundChecker>();

        groundCheckDistance = baseGroundCheckDistance;
    }

    void Update()
    {
        // Only allow movement and jumps while grounded
        ApplyInputVelocity();

        // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
        ApplyInputRotation(); 
    }

    void FixedUpdate()
    {
        unitCollider.radius = 0.2f;

        if (jumping)
        {
            unitRigidbody.velocity = inputVelocity;
            groundCheckDistance = 0.05f;
            jumping = false;
        }
        else if (grounded)
        {
            unitRigidbody.velocity = new Vector3(inputVelocity.x, unitRigidbody.velocity.y, inputVelocity.z);

            if(wasGrounded)
                groundCheckDistance = baseGroundCheckDistance;
        }
        else if (groundCheckDistance < baseGroundCheckDistance)
            groundCheckDistance = unitRigidbody.velocity.y < 0 ? baseGroundCheckDistance : groundCheckDistance + 0.01f;

        CheckGroundStatus();
    }


    void ApplyInputVelocity()
    {
        if (grounded)
        {
            //movedirection
            inputVelocity = new Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));

            //L+R MouseButton Movement
            if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && (Input.GetAxis("Vertical") == 0))
                inputVelocity.z += 1;

            if (inputVelocity.z > 1)
                inputVelocity.z = 1;

            //Strafing move (like Q/E movement    
            inputVelocity.x -= Input.GetAxis("Strafing");

            // if moving forward and to the side at the same time, compensate for distance
            if (Input.GetMouseButton(1) && (Input.GetAxis("Horizontal") != 0) && (Input.GetAxis("Vertical") != 0))
            {
                inputVelocity *= 0.7f;
            }

            // Check roots and apply final move speed
            inputVelocity *= controlledUnit.IsMovementBlocked ? 0 : controlledUnit.Character.parameters[ParameterType.Speed].FinalValue;

            // Jump!
            if (Input.GetButton("Jump") || jumping)
            {
                jumping = true;
                inputVelocity.y = jumpSpeed;
            }

            ApplyGroundedAnimations();

            //transform direction
            inputVelocity = transform.TransformDirection(inputVelocity);
        }
        else
        {
            inputVelocity = Vector3.zero;
            ApplyFlyingAnimations();
        }

    }

    void ApplyInputRotation()
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

    void ApplyGroundedAnimations()
    {
        var lastStatus = movementStatus;

        if (inputVelocity.x > 0)
            movementStatus = MovementStatus.StrafeRight;
        else if (inputVelocity.x < 0)
            movementStatus = MovementStatus.StrafeLeft;
        else if (inputVelocity.magnitude > 0)
            movementStatus = MovementStatus.Run;
        else
            movementStatus = MovementStatus.Stand;

        controlledUnit.Animator.SetBool("Grounded", true);

        float strafeTarget = (Vector3.Normalize(inputVelocity).x + 1) / 2;
        float currentStrafe = controlledUnit.Animator.GetFloat("Strafe");
        float strafeDelta = Time.deltaTime * 2 * Mathf.Sign(strafeTarget - currentStrafe);
        float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

        if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
            controlledUnit.Animator.SetFloat("Strafe", resultStrafe);


        if(lastStatus == MovementStatus.StrafeLeft || lastStatus == MovementStatus.StrafeRight)
            controlledUnit.Animator.SetFloat("Speed", 1);
        else
            controlledUnit.Animator.SetFloat("Speed", movementStatus == MovementStatus.Stand ? 0 : 1);
    }

    void ApplyFlyingAnimations()
    {
        movementStatus = MovementStatus.Jumping;

        controlledUnit.Animator.SetBool("Grounded", false);
    }

    void CheckGroundStatus()
    {
        wasGrounded = grounded;
        RaycastHit hitInfo;

        if (Physics.Raycast(unitCollider.bounds.center, Vector3.down, out hitInfo, unitCollider.bounds.extents.y +
            baseGroundCheckDistance * 2, 1 << LayerMask.NameToLayer("Ground")))
        {
            var distanceToGround = hitInfo.distance;

            if(distanceToGround > unitCollider.bounds.extents.y + groundCheckDistance)
            {
                if (grounded && inputVelocity.y <= 0)
                {
                    unitRigidbody.AddForce(Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                    groundNormal = hitInfo.normal;
                    grounded = true;
                }
                else
                {
                    grounded = false;
                    groundNormal = Vector3.up;
                }
            }
            else
            {
                groundNormal = hitInfo.normal;
                grounded = true;
            }
        }
        else
        {
            grounded = false;
            groundNormal = Vector3.up;
        }

        controlledUnit.IsGrounded = grounded;

        if (TooSteep || OnEdge)
            unitCollider.material = GameManager.SlidingMaterial;
        else
            unitCollider.material = GameManager.GroundedMaterial;
    }
}