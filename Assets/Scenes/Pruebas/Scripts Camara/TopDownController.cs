using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private Rigidbody rb;

    private Vector2 moveInput;

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    private void FixedUpdate()
    {
        // Movimiento relativo al plano XZ
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        if (moveDir != Vector3.zero)
        {
            transform.forward = moveDir; // Rotar personaje hacia donde camina
        }
    }
}