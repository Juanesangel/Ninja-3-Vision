using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private Rigidbody rb;

[Header("Dash Settings")]
[SerializeField] private float dashForce = 20f;
[SerializeField] private float dashCooldown = 1.5f;

private PlayerCombat playerCombat;
private float nextDashTime = 0f;

    private Vector2 moveInput;

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    private void FixedUpdate()
    {
        if (playerCombat != null && playerCombat.IsBlocking)
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        return;
    }
        // Movimiento relativo al plano XZ
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        if (moveDir != Vector3.zero)
        {
            transform.forward = moveDir; // Rotar personaje hacia donde camina
        }
    }
    //Mecanica Dash 
    public void OnDash(InputAction.CallbackContext context)
    {
        if (playerCombat != null && playerCombat.IsBlocking) return; // No dash mientras bloquea
        if (context.started && Time.time >= nextDashTime)
        {
            PerformTopDownDash();
            nextDashTime = Time.time + dashCooldown;
        }
    }

    private void PerformTopDownDash()
    {
        Vector3 dashDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        if (dashDirection == Vector3.zero) dashDirection = transform.forward;

        rb.AddForce(dashDirection.normalized * dashForce, ForceMode.Impulse);
    }
    private void Awake()
{
    rb = GetComponent<Rigidbody>();
    playerCombat = GetComponent<PlayerCombat>();
}
}