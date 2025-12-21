using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private WeaponSwitcher weaponSwitcher;
    public GameObject gunPrefab;

    [Header("Pickup Settings")]
    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.P;

    [Header("Held Adjustments")]
    public Vector3 heldPosition = new Vector3(3.2f, -0.2f, 0.5f);
    public Vector3 heldRotation = Vector3.zero;

    [Header("UI Feedback (Optional)")]
    public GameObject pickupPromptUI;

    private Transform player;
    private PlayerMovement playerMovement;
    private bool playerInRange = false;
    private bool pickedUp = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        playerMovement = player.GetComponent<PlayerMovement>();
        weaponSwitcher = player.GetComponent<WeaponSwitcher>();

        if (pickupPromptUI != null) pickupPromptUI.SetActive(false);
    }

    void Update()
    {
        if (pickedUp) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (pickupPromptUI != null) pickupPromptUI.SetActive(true);
            }

            if (Input.GetKeyDown(pickupKey))
                PickupGun();
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (pickupPromptUI != null) pickupPromptUI.SetActive(false);
            }
        }
    }

    void PickupGun()
    {
        pickedUp = true;

        GameObject gunInstance = Instantiate(gunPrefab);

        // Remove the pickup script & Collider from the gun in hand
        if (gunInstance.GetComponent<GunPickup>()) Destroy(gunInstance.GetComponent<GunPickup>());
        if (gunInstance.GetComponent<BoxCollider>()) Destroy(gunInstance.GetComponent<BoxCollider>());

        gunInstance.transform.SetParent(playerMovement.weaponHolder);
        gunInstance.transform.localPosition = heldPosition;
        gunInstance.transform.localRotation = Quaternion.Euler(heldRotation);
        gunInstance.transform.localScale = Vector3.one;

        gunInstance.SetActive(false); // Keep inactive

        // EnableGun in WeaponSwitcher will handle the UI assignment now
        weaponSwitcher.EnableGun(gunInstance);

        // Notify LevelManager that gun has been picked up
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnGunPickedUp();
        }

        if (pickupPromptUI != null) pickupPromptUI.SetActive(false);
        Destroy(gameObject);
    }
}