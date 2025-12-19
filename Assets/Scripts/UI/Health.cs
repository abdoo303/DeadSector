using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(TeamIdentifier))]
public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Game Over Settings (For Player)")]
    public bool isPlayer = false;
    public string gameOverSceneName = "GameOver"; // Name of your game over scene

    // events
    public event Action<float, float> OnHealthChanged; // (current, max)
    public event Action OnDied;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        Debug.Log("amount is "+ amount);
        if (amount <= 0) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log("Current Health "+currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void HealToMax()
    {
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} fully healed to max health: {maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        OnDied?.Invoke();
        
        // If this is the player, trigger game over
        if (isPlayer)
        {
            Time.timeScale = 1f; // Ensure time is running
            Debug.Log("Player died! Loading Game Over scene...");
            SceneManager.LoadScene(gameOverSceneName);
            // Alternative: Use scene index: SceneManager.LoadScene(0);
        }
        // For non-players (like zombies), just invoke the event
        // The ZombieAI script will handle the death animation and destruction
    }
}