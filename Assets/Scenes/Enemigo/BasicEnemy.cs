using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [Header("Parámetros del Enemigo")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float contactDamage = 15f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Referencias")]
    [SerializeField] private LayerMask playerLayer;

    private float currentHealth;
    private float lastAttackTime;
    private Transform playerTransform;
    private PlayerModeManager modeManager;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // Buscar automáticamente las referencias del jugador y del gestor de modos
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            modeManager = playerObj.GetComponent<PlayerModeManager>();
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        Vector3 targetPosition = playerTransform.position;

        // --- ADAPTACIÓN SEGÚN EL MODO DE CÁMARA ---
        if (modeManager != null && modeManager.CurrentMode == CameraMode.SideScroll)
        {
            // En modo lateral (2D), forzamos al enemigo a alinearse a la misma Z del jugador
            // y perseguir únicamente en el eje X
            targetPosition.z = playerTransform.position.z;

            // Suavizamos el ajuste en Z para alinearse al mismo plano instantáneamente
            transform.position = new Vector3(transform.position.x, transform.position.y, playerTransform.position.z);
            
            // Congelamos rotaciones y el eje Z en el Rigidbody
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            // En POV y TopDown el enemigo se mueve en el plano 3D (XZ)
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Calcular dirección de movimiento
        Vector3 direction = (targetPosition - transform.position);
        
        // Si estamos en SideScroll, ignoramos diferencias en Z y Y para el movimiento horizontal puro
        if (modeManager != null && modeManager.CurrentMode == CameraMode.SideScroll)
        {
            direction.z = 0;
            direction.y = 0;
        }
        else
        {
            direction.y = 0; // Prevenir que vuele si el jugador salta
        }

        Vector3 moveDir = direction.normalized;

        // Aplicar velocidad física
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        // Orientar al enemigo hacia donde camina
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    // --- DAÑO POR CONTACTO ---
    private void OnCollisionStay(Collision collision)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        // Verificar si colisionó con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<HealthSystem>(out var playerHealth))
            {
                playerHealth.TakeDamage(contactDamage);
                lastAttackTime = Time.time;
                Debug.Log($"¡Enemigo infligió {contactDamage} de daño al jugador!");
            }
        }
    }

    // --- RECIBIR DAÑO DEL JUGADOR ---
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        Debug.Log($"<color=yellow>[ENEMIGO {gameObject.name}]</color> Recibió {damage} de daño. Vida restante: <b>{currentHealth}/{maxHealth}</b>");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red><b>[ENEMIGO {gameObject.name} DESTRUIDO]</b></color>");
        Destroy(gameObject);
    }
}