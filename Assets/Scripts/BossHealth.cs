using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health Setup")]
    public float maxHealth = 500f;
    public float currentHealth;

    [Header("UI Connection")]
    // Drag your "HealthBarCanvas" object here
    public GameObject healthBarCanvasObject;
    private Slider healthSlider; // We will find this automatically

    private BossController controller;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<BossController>();

        // 1. Find the Slider on the canvas you provided
        if (healthBarCanvasObject != null)
        {
            healthSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        // 2. Update the Slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        if (controller != null) controller.OnDeath();

        // Hide the health bar
        if (healthBarCanvasObject != null) Destroy(healthBarCanvasObject, 2f);
    }
}