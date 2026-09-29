using UnityEngine;
using UnityEngine.InputSystem;

public class SideScrollController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Rigidbody rb;

    private float moveInputX;

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
        // Movimiento restringido solo en el eje X
        rb.linearVelocity = new Vector3(moveInputX * speed, rb.linearVelocity.y, 0f);

        if (moveInputX != 0)
        {
            transform.forward = new Vector3(moveInputX, 0, 0); // Mirar a izquierda o derecha
        }
    }
}