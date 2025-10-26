using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public GameObject gunWeaponPrefab; // Assign your gun weapon prefab here
    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.P;

    [Header("UI Feedback (Optional)")]
    public GameObject pickupPromptUI; // Optional: UI element to show "Press P to pickup"

    private Transform player;
    private PlayerMovement playerMovement;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();

        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);
    }

    void Update()
    {
        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= pickupRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (pickupPromptUI != null)
                    pickupPromptUI.SetActive(true);
            }

            // Check for pickup input
            if (Input.GetKeyDown(pickupKey))
            {
                PickupGun();
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (pickupPromptUI != null)
                    pickupPromptUI.SetActive(false);
            }
        }
    }

    void PickupGun()
    {
        if (playerMovement != null)
        {
            // Instantiate the gun weapon and equip it
            GameObject gunInstance = Instantiate(gunWeaponPrefab);
            GunWeapon gunWeapon = gunInstance.GetComponent<GunWeapon>();
            
            if (gunWeapon != null)
            {
                playerMovement.EquipWeapon(gunWeapon);
                Destroy(gameObject); // Remove the pickup from the scene
            }
        }
    }

    // Optional: Draw pickup range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}