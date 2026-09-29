using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public enum CameraMode { POV, TopDown, SideScroll }
public class PlayerModeManager : MonoBehaviour
{
    [Header("Componente de Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Cámaras Cinemachine")]
    [SerializeField] private CinemachineCamera povCamera;
    [SerializeField] private CinemachineCamera topDownCamera;
    [SerializeField] private CinemachineCamera sideScrollCamera;

    [Header("Scripts de Control")]
    [SerializeField] private POVController povController;
    [SerializeField] private TopDownController topDownController;
    [SerializeField] private SideScrollController sideScrollController;

    [Header("Física")]
    [SerializeField] private Rigidbody rb;

public CameraMode CurrentMode { get; private set; }

    private void Start()
    {
        SetMode(CameraMode.POV);
    }

    private void Update()
    {
        // Verificar si hay un teclado conectado
        if (Keyboard.current == null) return;
        // Teclas directas para cambiar de perspectiva
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetMode(CameraMode.POV);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetMode(CameraMode.TopDown);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetMode(CameraMode.SideScroll);
    }

    public void SetMode(CameraMode newMode)
    {
        CurrentMode = newMode;
        // 1. Resetear prioridades de cámara (Cinemachine hará la transición suave)
        povCamera.Priority = 0;
        topDownCamera.Priority = 0;
        sideScrollCamera.Priority = 0;

        // 2. Apagar todos los controladores
        povController.enabled = false;
        topDownController.enabled = false;
        sideScrollController.enabled = false;

        // 3. Activar el modo correspondiente
        switch (newMode)
        {
            case CameraMode.POV:
                povCamera.Priority = 10;
                povController.enabled = true;
                playerInput.SwitchCurrentActionMap("POV_Map");
                
                Cursor.lockState = CursorLockMode.Locked;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                break;

            case CameraMode.TopDown:
                topDownCamera.Priority = 10;
                topDownController.enabled = true;
                playerInput.SwitchCurrentActionMap("TopDown_Map");

                Cursor.lockState = CursorLockMode.None;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                break;

            case CameraMode.SideScroll:
                sideScrollCamera.Priority = 10;
                sideScrollController.enabled = true;
                playerInput.SwitchCurrentActionMap("SideScroll_Map");

                Cursor.lockState = CursorLockMode.None;
                // Alinear al centro en Z y bloquear eje Z para mantener plano 2D
                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                break;
        }
    }
}