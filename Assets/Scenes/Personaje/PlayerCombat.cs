using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque Básico / Combo")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float attackOffset = 1.2f; 
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private float comboResetTime = 1.2f;
    [SerializeField] private LayerMask enemyLayer;

    private int comboStep = 0; // 0: Sin combo, 1: Golpe 1, 2: Golpe 2, 3: Golpe 3 (Máximo)
    private float lastAttackTime;
    private float lastHitTime;

    [Header("Defensa / Bloqueo")]
    public bool IsBlocking { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Transform attackPoint; // Punto frente al jugador para verificar el radio de golpe

    private void Awake()
    {
        if (attackPoint == null) attackPoint = transform;
    }

    private void Update()
    {
        // Reiniciar el combo si pasa demasiado tiempo sin acertar/atacar
        if (comboStep > 0 && Time.time - lastHitTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    // --- ACCIÓN DE ATAQUE (Click Izquierdo) ---
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (IsBlocking) return; // No puede atacar si se está defendiendo
        if (Time.time - lastAttackTime < attackCooldown) return;

        ExecuteAttack();
    }

    private void ExecuteAttack()
    {
        lastAttackTime = Time.time;
        Vector3 sphereCenter = transform.position + (transform.forward * attackOffset);

        // Detectar enemigos en un radio cercano alrededor de attackPoint
        Collider[] hitEnemies = Physics.OverlapSphere(sphereCenter, attackRadius, enemyLayer);
        
        if (hitEnemies.Length > 0)
        {
            // Avanzar en el combo si acertó
            comboStep = Mathf.Min(comboStep + 1, 3);
            lastHitTime = Time.time;

            // Multiplicador de daño según la racha acertada (1x, 1.5x, 2x por ejemplo, o daño fijo x1, x2, x3)
            float currentDamage = baseDamage * comboStep;

            foreach (Collider enemy in hitEnemies)
            {
                // Reemplaza 'EnemyAI' por la clase/script de daño de tu enemigo
                if (enemy.TryGetComponent<BasicEnemy>(out var enemyScript)) 
                {
                    // enemyScript.TakeDamage(currentDamage);
                    Debug.Log($"¡Golpe {comboStep}/3 acertado! Daño infligido: {currentDamage}");
                }
            }
        }
        else
        {
            // Si falla el ataque en el aire, se reinicia la racha del combo
            ResetCombo();
            Debug.Log("Ataque fallado. Combo reiniciado.");
        }
        foreach (Collider enemy in hitEnemies)
            {
                if (enemy.TryGetComponent<BasicEnemy>(out var enemyScript))
                {
                    float currentDamage = baseDamage * comboStep;
                    enemyScript.TakeDamage(currentDamage);
                    Debug.Log($"¡Golpe {comboStep}/3 acertado! Daño infligido: {currentDamage}");
                }
            }
    }

    private void ResetCombo()
    {
        comboStep = 0;
    }

    // --- ACCIÓN DE DEFENSA (Click Derecho Mantener) ---
    public void OnDefend(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsBlocking = true;
            Debug.Log("Defensa activada.");
        }
        else if (context.canceled)
        {
            IsBlocking = false;
            Debug.Log("Defensa desactivada.");
        }
    }

    // Dibujar el radio de ataque en la vista Scene para ajustarlo fácilmente
    private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Vector3 sphereCenter = transform.position + (transform.forward * attackOffset);
    Gizmos.DrawWireSphere(sphereCenter, attackRadius);
}
}