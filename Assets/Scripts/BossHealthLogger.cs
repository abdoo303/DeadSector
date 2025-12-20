using UnityEngine;
using System.Collections;

public class BossHealthLogger : MonoBehaviour
{
    public Health healthScript;

    void Start()
    {
        // Auto-find health if not assigned
        if (healthScript == null)
            healthScript = GetComponent<Health>();

        if (healthScript == null)
            healthScript = GetComponentInParent<Health>();

        if (healthScript != null)
        {
            StartCoroutine(LogHealthRoutine());
        }
        else
        {
            Debug.LogError("❌ Logger could not find a Health script on " + gameObject.name);
        }
    }

    IEnumerator LogHealthRoutine()
    {
        while (true) // Run forever until object is destroyed
        {
            if (healthScript != null)
            {
                Debug.Log($"🔍 [DEBUG CHECK] Boss Health: {healthScript.currentHealth} / {healthScript.maxHealth}");
            }
            yield return new WaitForSeconds(5f);
        }
    }
}