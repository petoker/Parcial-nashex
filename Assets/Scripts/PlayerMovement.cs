using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;

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

    // Se llama automáticamente por el Input System (WASD / flechas / stick)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput.sqrMagnitude > 0.0001f;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // Si no hay movimiento => AFK
        if (!isMoving)
        {
            animator.SetBool("IsMoving", false);
            animator.SetInteger("LookState", 0); // Afk
            return;
        }

        animator.SetBool("IsMoving", true);

        // Direcciones según el ángulo del input
        // Ángulo en grados: 0 = derecha, 90 = arriba, -90 = abajo, 180/-180 = izquierda
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        int lookState;
        if (angle >= 67.5f && angle < 112.5f) lookState = 1; // Up (W)
        else if (angle <= -67.5f && angle > -112.5f) lookState = 2; // Down (S)
        else if (angle >= 112.5f && angle < 157.5f) lookState = 3; // LeftUp (A+W)
        else if (angle <= -112.5f && angle > -157.5f) lookState = 4; // LeftDown (A+S)
        else if (angle >= 22.5f && angle < 67.5f) lookState = 5; // RightUp (D+W)
        else if (angle <= -22.5f && angle > -67.5f) lookState = 6; // RightDown (D+S)
        else if (angle > 157.5f || angle <= -157.5f) lookState = 4; // A sola = RightDown
        else if (angle >= -22.5f && angle < 22.5f) lookState = 6; // D sola = LeftDown
        else
        {
            // Si quisieras agregar "mirar a izquierda" o "mirar a derecha" puros,
            // podrías mapear aquí 180/-180 y 0 grados. Por ahora, mantenemos AFK si cae fuera.
            lookState = 0;
        }

        animator.SetInteger("LookState", lookState);
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
}