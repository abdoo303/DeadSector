using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
    public GameObject pickupText;
    public float damageMultiplier = 2f;
    public float duration = 10f;

    private bool playerInRange = false;

    void Start()
    {
        if (pickupText != null) pickupText.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("🍄 Mushroom picked up!");

            SwordDamage sword = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<SwordDamage>();
            
            if (sword != null)
            {
                sword.ApplyDamageMultiplierSafe(damageMultiplier, duration);
                Debug.Log($"⚡ Damage boost: {damageMultiplier}x for {duration}s!");
            }

            if (pickupText != null) pickupText.SetActive(false);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pickupText != null) pickupText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pickupText != null) pickupText.SetActive(false);
        }
    }
}