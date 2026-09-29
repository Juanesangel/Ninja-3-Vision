using UnityEngine;
using UnityEngine.InputSystem;

public class SideScrollController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Rigidbody rb;
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 18f;
    [SerializeField] private float dashCooldown = 1.5f;
    private float nextDashTime = 0f;
    private float moveInputX;
    private float lastFacingDirectionX = 1f;
    private PlayerCombat playerCombat;

    public void OnMove(InputAction.CallbackContext context) => moveInputX = context.ReadValue<Vector2>().x;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (playerCombat != null && playerCombat.IsBlocking)
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        return;
    }
        // Movimiento restringido solo en el eje X
        rb.linearVelocity = new Vector3(moveInputX * speed, rb.linearVelocity.y, 0f);

        if (moveInputX != 0)
        {
            transform.forward = new Vector3(moveInputX, 0, 0); // Mirar a izquierda o derecha
        }
    }
    //Mecanica Dash 
    public void OnDash(InputAction.CallbackContext context)
    {
        if (playerCombat != null && playerCombat.IsBlocking) return;
        if (context.started && Time.time >= nextDashTime)
        {
            PerformSideScrollDash();
            nextDashTime = Time.time + dashCooldown;
        }
    }

        private void PerformSideScrollDash()
    {
        // Si estás presionando A/D usa ese input; si estás quieto, usa la última dirección a la que mirabas
        float directionX = moveInputX != 0 ? Mathf.Sign(moveInputX) : lastFacingDirectionX;
        
        Vector3 dashDirection = new Vector3(directionX, 0f, 0f);

        // Mantiene impulso vertical para no interrumpir el salto pero aplica el dash horizontal
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y * 0.5f, 0f);
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCombat = GetComponent<PlayerCombat>();
    }
}