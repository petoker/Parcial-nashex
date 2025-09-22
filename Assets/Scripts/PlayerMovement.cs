using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    // Dash 
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    private bool isDashing = false;      // Animation and logic
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDir = Vector2.zero;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput; // Movement vector
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name);
        }
    }

    // Movement input (WASD / arrows / stick)
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0.0001f;
        UpdateAnimation();
    }

    // Create an Action "Dash" (Button) with Space binding in your Input Actions.
    private void OnDash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        TryStartDash();
    }

    private void TryStartDash()
    {
        if (isDashing) return;
        if (dashCooldownTimer > 0f) return;

        // Dash direction: if there is input, use that; otherwise, default is up
        Vector2 dir = isMoving ? moveInput.normalized : Vector2.up;
        if (dir.sqrMagnitude < 0.0001f) return;

        dashDir = dir;
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Set LookState to dash direction (for directional animation)
        int dashLook = DirToLookState(dashDir);
        if (animator != null)
        {
            animator.SetBool("IsDashing", true); 
            animator.SetInteger("LookState", dashLook);
        }
    }

    private void Update()
    {
        // Dash cooldown/duration timers
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                if (animator != null) animator.SetBool("IsDashing", false); // <- back to false when finished
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // Movement during dash (only in dash direction)
            Vector2 newPos = rb.position + dashDir * dashSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            return;
        }

        // Normal movement
        Vector2 newPos2 = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos2);
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // If no movement and not dashing => AFK
        if (!isMoving && !isDashing)
        {
            animator.SetBool("IsMoving", false);
            animator.SetInteger("LookState", 0); // Afk
            return;
        }

        // Keep "in movement" while there is input or dash
        animator.SetBool("IsMoving", isMoving || isDashing);

        // If we are dashing, LookState is already set with dashDir
        if (isDashing) return;

        // If we are not dashing, calculate LookState from current input
        int lookState = DirToLookState(moveInput);
        animator.SetInteger("LookState", lookState);
    }

    private int DirToLookState(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return 0;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle >= 67.5f && angle < 112.5f) return 1;          // Up
        if (angle <= -67.5f && angle > -112.5f) return 2;        // Down
        if (angle >= 112.5f && angle < 157.5f) return 3;         // LeftUp
        if (angle <= -112.5f && angle > -157.5f) return 4;       // LeftDown
        if (angle >= 22.5f && angle < 67.5f) return 5;           // RightUp
        if (angle <= -22.5f && angle > -67.5f) return 6;         // RightDown
        if (angle > 157.5f || angle <= -157.5f) return 4;        // A alone
        if (angle >= -22.5f && angle < 22.5f) return 6;          // D alone
        return 0;
    }
    // =========================================
}