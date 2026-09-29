using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Escudo")]
    [SerializeField] private float maxShield = 50f;
    private float currentShield;
    private PlayerCombat playerCombat;

    // Eventos opcionales para conectar con la UI mas adelante
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnShieldChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        playerCombat = GetComponent<PlayerCombat>();
    }

    public void TakeDamage(float damage)
    {
        if (playerCombat != null && playerCombat.IsBlocking)
        {
            Debug.Log("<color=blue>[JUGADOR]</color> ¡Ataque BLOQUEADO! Daño negado.");
            return;
        }
        if (damage <= 0) return;

        // El daño impacta primero al escudo
        if (currentShield > 0)
        {
            if (currentShield >= damage)
            {
                currentShield -= damage;
                damage = 0;
            }
            else
            {
                damage -= currentShield;
                currentShield = 0;
            }
            OnShieldChanged?.Invoke(currentShield, maxShield);
        }

        // El daño restante pasa a la vida
        if (damage > 0)
        {
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RechargeShield(float amount)
    {
        currentShield = Mathf.Clamp(currentShield + amount, 0, maxShield);
        OnShieldChanged?.Invoke(currentShield, maxShield);
    }

    private void Die()
    {
        Debug.Log("<color=red><b>[JUGADOR HA MUERTO]</b> Desapareciendo del juego...</color>");
        
        // Desactiva o destruye el GameObject del jugador
        gameObject.SetActive(false); 
        // Nota: Se usa SetActive(false) o Destroy(gameObject). 
        // SetActive(false) previene errores de referencia nula en cámaras/scripts inmediatamente.
    }
    private void PrintStatus()
    {
        Debug.Log($"<color=green>[JUGADOR]</color> Vida: <b>{currentHealth}/{maxHealth}</b> | Escudo: <b>{currentShield}/{maxShield}</b>");
    }
}