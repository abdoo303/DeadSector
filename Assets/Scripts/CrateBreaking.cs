using UnityEngine;
using UnityEngine.UI;

public class CrateInteract : MonoBehaviour
{
    [Header("Crate Settings")]
    public int healthBoost = 1;          // Amount of health to add
    public float interactDistance = 3f;  // Distance to interact with crate

    [Header("UI (optional)")]
    public Text interactText;            // UI Text to show "Press E to interact"

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogWarning("Player not found! Make sure it has the tag 'Player'.");

        if (interactText != null)
            interactText.enabled = false; // Hide UI prompt initially
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Show or hide the interact prompt
        if (interactText != null)
            interactText.enabled = distance <= interactDistance;

        // Press E to interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (distance <= interactDistance)
            {
                InteractWithCrate();
            }
            else
            {
                Debug.Log("No crate nearby to interact with.");
            }
        }
    }

    void InteractWithCrate()
    {
        // Give player full health using unified Health system
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.HealToMax();
            Debug.Log("Player fully healed from crate!");
        }
        else
        {
            Debug.LogWarning("Health component not found on Player!");
        }

        // Make crate disappear
        gameObject.SetActive(false);
        Debug.Log("Crate disappeared!");
    }
}
