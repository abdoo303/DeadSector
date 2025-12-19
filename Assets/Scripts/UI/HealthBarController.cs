using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;      // the fill image (uses fillAmount)
    public Canvas canvas;        // world-space canvas (optional)
    
    [Header("Colors")]
    public Color friendlyColor = Color.green;
    public Color enemyColor = Color.red;
    public Color neutralColor = Color.yellow;
    
    private Health trackedHealth;
    private TeamIdentifier teamIdentifier;
    
    // call this to bind the UI to a character's health
    public void Bind(Health health)
    {
        Debug.Log($"[HealthBarController] Bind called on {gameObject.name}");
        
        if (health == null)
        {
            Debug.LogError($"[HealthBarController] Health is NULL on {gameObject.name}!");
            return;
        }
        
        if (fillImage == null)
        {
            Debug.LogError($"[HealthBarController] FillImage is NULL on {gameObject.name}!");
            return;
        }
        
        Debug.Log($"[HealthBarController] Successfully starting bind for {health.gameObject.name}");
        
        trackedHealth = health;
        teamIdentifier = health.GetComponent<TeamIdentifier>();
        
        // set color based on team
        if (teamIdentifier != null)
        {
            Debug.Log($"[HealthBarController] Team is: {teamIdentifier.team}");
            switch (teamIdentifier.team)
            {
                case Team.Friendly:
                    fillImage.color = friendlyColor;
                    Debug.Log($"[HealthBarController] Set color to FRIENDLY (green)");
                    break;
                case Team.Enemy:
                    fillImage.color = enemyColor;
                    Debug.Log($"[HealthBarController] Set color to ENEMY (red)");
                    break;
                default:
                    fillImage.color = neutralColor;
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"[HealthBarController] No TeamIdentifier found!");
        }
        
        // initialize UI:
        UpdateBar(health.currentHealth, health.maxHealth);
        
        // subscribe:
        health.OnHealthChanged += UpdateBar;
        health.OnDied += OnTrackedDied;
        
        Debug.Log($"[HealthBarController] Bind complete! Subscribed to events.");
    }
    
    void OnDestroy()
    {
        if (trackedHealth != null)
        {
            trackedHealth.OnHealthChanged -= UpdateBar;
            trackedHealth.OnDied -= OnTrackedDied;
        }
    }
    
    void UpdateBar(float current, float max)
    {
        Debug.Log($"[HealthBarController] UpdateBar called: {current}/{max} = {current/max}");
        
        if (fillImage != null)
        {
            float fillAmount = Mathf.Clamp01(current / max);
            fillImage.fillAmount = fillAmount;
            Debug.Log($"[HealthBarController] Set fillAmount to: {fillAmount}");
        }
        else
        {
            Debug.LogError("[HealthBarController] FillImage is null in UpdateBar!");
        }
    }
    
    void OnTrackedDied()
    {
        Debug.Log($"[HealthBarController] Character died!");
        // optional: fade out, disable, play animation
        gameObject.SetActive(false);
    }
}