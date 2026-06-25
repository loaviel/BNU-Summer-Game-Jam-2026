using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class CatController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 25f;
    public float turnSmoothTime = 0.1f;

    private float turnSmoothVelocity;

    [Header("Jump")]
    public float jumpHeight = 2.5f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Gravity")]
    public float gravity = -25f;
    public float groundedGravity = -0.1f;

    [Header("Jump Feel")]
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 2.5f;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private CharacterController controller;
    private Camera cam;

    private Vector3 velocity;
    private Vector3 moveVelocity;

    private CatState currentState;

    [SerializeField] private float climbCheckDistance = 1f;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float climbExitBoost = 5f;

    [Header("Glide")]
    [SerializeField] private float glideGravity = -2f;
    [SerializeField] private float glideForwardBoost = 2f;
    [SerializeField] private float glideFallSpeed = -6f;
    private bool isGliding;

    private UmbrellaSystem umbrellaSystem;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;


        umbrellaSystem = GetComponent<UmbrellaSystem>();
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        HandleCursor();

        UpdateState();

        UpdateCoyoteTime();
        UpdateJumpBuffer();

        HandleClimbInput();

        switch (currentState)
        {
            case CatState.Climbing:
                HandleClimbing();
                break;

            default:
                Move();

                HandleJump();

                HandleGlide();

                ApplyGravity();

                Vector3 finalVelocity =
                    moveVelocity +
                    Vector3.up * velocity.y;

                if (isGliding)
                {
                    finalVelocity +=
                        transform.forward *
                        glideForwardBoost;
                }

                controller.Move(
                    finalVelocity *
                    Time.deltaTime
                );
                break;
        }
    }

    #region Cursor

    private void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UnlockCursor();

        if (Cursor.lockState != CursorLockMode.Locked &&
            Input.GetMouseButtonDown(0))
            LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #endregion

    #region Jump

    private void UpdateCoyoteTime()
    {
        if (controller.isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;
    }

    private void UpdateJumpBuffer()
    {
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleJump()
    {
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    #endregion

    #region Movement

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(inputDirection.x, inputDirection.z) *
                Mathf.Rad2Deg +
                cam.transform.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime
            );

            transform.rotation =
                Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDirection =
                Quaternion.Euler(0f, targetAngle, 0f) *
                Vector3.forward;

            moveVelocity = Vector3.MoveTowards(
                moveVelocity,
                moveDirection * moveSpeed,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            moveVelocity = Vector3.MoveTowards(
                moveVelocity,
                Vector3.zero,
                acceleration * Time.deltaTime
            );
        }

        //controller.Move(moveVelocity * Time.deltaTime);
    }

    #endregion

    #region Gravity

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            Debug.Log(velocity.y);
        }

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = groundedGravity;
        }
        else
        {
            if (isGliding)
            {

                umbrellaSystem.OpenUmbrella();
                umbrellaSystem.DrainForGlide();

                velocity.y += gravity * 0.5f * Time.deltaTime;

                velocity.y = Mathf.Max(
                    velocity.y,
                    glideFallSpeed
                );
            }
            else if (velocity.y < 0)
            {
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
            }
            else if (!Input.GetButton("Jump"))
            {
                velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
        }
        //controller.Move(velocity * Time.deltaTime);
    }

    #endregion



    #region Climb

    private void UpdateState()
    {
        if (currentState == CatState.Climbing)
            return;

        currentState =
            controller.isGrounded
            ? CatState.Grounded
            : CatState.Airborne;
    }

    private bool CanClimb(out RaycastHit hit)
    {
        Vector3 origin =
            transform.position + Vector3.up * 0.5f;

        return Physics.Raycast(
            origin,
            transform.forward,
            out hit,
            climbCheckDistance,
            climbableLayer
        );
    }

    private void HandleClimbInput()
    {
        if (currentState == CatState.Climbing)
            return;

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (CanClimb(out RaycastHit hit))
        {
            EnterClimb(hit);
        }
    }


    private void EnterClimb(RaycastHit hit)
    {
        currentState = CatState.Climbing;

        velocity = Vector3.zero;
        moveVelocity = Vector3.zero;

        transform.forward = -hit.normal;
    }

    private void HandleClimbing()
    {
        if (!CanClimb(out RaycastHit hit))
        {
            ExitClimb();
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 climbMove = Vector3.up * vertical;

        controller.Move(
            climbMove *
            climbSpeed *
            Time.deltaTime
        );

        if (Input.GetButtonDown("Jump"))
        {
            ExitClimb();
        }
    }

    private void ExitClimb()
    {
        currentState = CatState.Airborne;

        moveVelocity = Vector3.zero;

        velocity.y = climbExitBoost;
    }
    #endregion

    #region Glide
    private void HandleGlide()
    {
        isGliding =
    !controller.isGrounded &&
    velocity.y < 0f &&
    Input.GetButton("Jump") &&
    umbrellaSystem.currentCharge > 0f;

        if (isGliding)
            umbrellaSystem.OpenUmbrella();
        else
            umbrellaSystem.CloseUmbrella();
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(
            transform.position,
            transform.forward * 2f
        );

        Gizmos.color = Color.green;

        Vector3 origin =
            transform.position + Vector3.up * 0.5f;

        Gizmos.DrawRay(
            origin,
            transform.forward * climbCheckDistance
        );
    }

}