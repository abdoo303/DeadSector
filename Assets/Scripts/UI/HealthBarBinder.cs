using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthBarBinder : MonoBehaviour
{
    public HealthBarController barController; // optionally assign in inspector
    
    void Awake()
    {
        Debug.Log($"[HealthBarBinder] Awake called on {gameObject.name}");
        
        if (barController == null)
        {
            Debug.Log($"[HealthBarBinder] barController is null, searching in children...");
            barController = GetComponentInChildren<HealthBarController>();
        }
        
        if (barController != null)
        {
            Debug.Log($"[HealthBarBinder] Found barController: {barController.gameObject.name}");
            Health health = GetComponent<Health>();
            Debug.Log($"[HealthBarBinder] Found Health component: {health != null}");
            barController.Bind(health);
        }
        else
        {
            Debug.LogError($"[HealthBarBinder] Could NOT find HealthBarController on {gameObject.name}!");
        }
    }
}