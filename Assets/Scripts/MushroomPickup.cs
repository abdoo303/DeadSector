using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
    public GameObject pickupText; // UI text to show
    public float damageFactor = 2f; // Multiply damage by this factor (2 = doubles damage)

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Mushroom picked up!");

            // Permanently increase sword damage by the factor
            SwordDamage sword = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<SwordDamage>();
            if (sword != null)
            {
                sword.IncreaseDamagePermanently(damageFactor);
                Debug.Log($"Sword damage permanently multiplied by {damageFactor}x!");
            }

            // Hide UI and destroy mushroom
            if (pickupText != null) pickupText.SetActive(false);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter with: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered mushroom trigger");
            playerInRange = true;
            if (pickupText != null) pickupText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit with: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left mushroom trigger");
            playerInRange = false;
            if (pickupText != null) pickupText.SetActive(false);
        }
    }
}
