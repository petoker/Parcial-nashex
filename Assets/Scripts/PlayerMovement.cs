using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

    // Dash 
    [Header("Dash")]
    public float dashSpeed = 12f;        
    public float dashDuration = 0.15f;   
    public float dashCooldown = 0.5f;   

    private bool isDashing = false;      // Animation and logic
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDir = Vector2.zero;
    // =======================

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput; // Dirección de movimiento
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("No se encontró componente Animator en " + gameObject.name);
        }
    }

    // Input de movimiento (WASD / flechas / stick)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0.0001f;
        UpdateAnimation();
    }

    // ===== NUEVO: Input de Dash (Space) =====
    // Creá una Action "Dash" (Button) con binding Space en tu Input Actions.
    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        TryStartDash();
    }

    private void TryStartDash()
    {
        if (isDashing) return;
        if (dashCooldownTimer > 0f) return;

        // Dirección del dash: si hay input, esa; si no, por defecto arriba
        Vector2 dir = isMoving ? moveInput.normalized : Vector2.up;
        if (dir.sqrMagnitude < 0.0001f) return;

        dashDir = dir;
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Fijamos el LookState a la dirección del dash (para animación direccional)
        int dashLook = DirToLookState(dashDir);
        if (animator != null)
        {
            animator.SetBool("IsDashing", true); // <- NUEVO: parámetro bool
            animator.SetInteger("LookState", dashLook);
        }
    }
    // ========================================

    private void Update()
    {
        // Timers de cooldown/duración del dash
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                if (animator != null) animator.SetBool("IsDashing", false); // <- vuelve a false al terminar
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // Movimiento durante el dash (solo en la dir del dash)
            Vector2 newPos = rb.position + dashDir * dashSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            return;
        }

        // Movimiento normal
        Vector2 newPos2 = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos2);
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // Si no hay movimiento y no estamos dashing => AFK
        if (!isMoving && !isDashing)
        {
            animator.SetBool("IsMoving", false);
            animator.SetInteger("LookState", 0); // Afk
            return;
        }

        // Mantener "en movimiento" mientras hay input o dash
        animator.SetBool("IsMoving", isMoving || isDashing);

        // Si estamos dashing, el LookState YA se fijó con dashDir.
        if (isDashing) return;

        // Si no estamos dashing, calculamos LookState por el input actual
        int lookState = DirToLookState(moveInput);
        animator.SetInteger("LookState", lookState);
    }

    // ===== NUEVO: helper para mapear dirección → LookState =====
    // Mapea igual que venías usando:
    // 1=Up, 2=Down, 3=LeftUp, 4=LeftDown, 5=RightUp, 6=RightDown
    // (además A sola -> 4, D sola -> 6 como pediste antes)
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
        if (angle > 157.5f || angle <= -157.5f) return 4;        // A sola
        if (angle >= -22.5f && angle < 22.5f) return 6;          // D sola
        return 0;
    }
    // =========================================
}