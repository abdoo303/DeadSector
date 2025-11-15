using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth=100;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log("Player hit! Health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }
    public void AddHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Health increased by " + amount + ". Current health: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("PLAYER DIED!");
        // TODO later: add respawn, game over screen, animation, etc.
    }
}
