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
    public float groundedGravity = -2f;

    [Header("Jump Feel")]
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 2.5f;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private CharacterController controller;
    private Camera cam;

    private Vector3 velocity;
    private Vector3 moveVelocity;

    private Vector3 smoothedMoveDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        HandleCursor();

        UpdateCoyoteTime();
        UpdateJumpBuffer();

        Move();
        HandleJump();
        ApplyGravity();
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

        controller.Move(moveVelocity * Time.deltaTime);
    }

    #endregion

    #region Gravity

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = groundedGravity;
        }
        else
        {
            if (velocity.y < 0)
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

        controller.Move(velocity * Time.deltaTime);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(
            transform.position,
            transform.forward * 2f
        );
    }
}