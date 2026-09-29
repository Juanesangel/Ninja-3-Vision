using UnityEngine;
using UnityEngine.InputSystem;

public class POVController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();

    private void Update()
    {
        // Rotación de cámara vertical/horizontal en 1a persona
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * (lookInput.x * mouseSensitivity));
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
    }
}