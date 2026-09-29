using UnityEngine;
using UnityEngine.InputSystem;

public class POVController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Rigidbody rb;


    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 25f;
    [SerializeField] private float dashCooldown = 1.5f;

    private PlayerCombat playerCombat;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float nextDashTime = 0f;

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
        if (playerCombat != null && playerCombat.IsBlocking)
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        return;
    }
        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        
    }

    //Dash Mecanica
        public void OnDash(InputAction.CallbackContext context)
        {
            if (playerCombat != null && playerCombat.IsBlocking) return;
            
            if (context.started && Time.time >= nextDashTime)
            {
                PerformPOVDash();
                nextDashTime = Time.time + dashCooldown;
            }
        }
    private void PerformPOVDash()
    {
        // Si presiona WASD usa esa direccion; si no, impulsa hacia adelante
        Vector3 dashDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (dashDirection == Vector3.zero) dashDirection = transform.forward;

        rb.AddForce(dashDirection.normalized * dashForce, ForceMode.Impulse);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCombat = GetComponent<PlayerCombat>();
    }
}