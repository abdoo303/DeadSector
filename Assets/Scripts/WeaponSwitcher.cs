using UnityEngine;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject sword;
    public GameObject gun;
    public PlayerMovement playerMovement;

    [Header("UI References (DRAG THESE IN!)")]
    public TMP_Text uiAmmoText;       // Drag "AmmoText" here
    public TMP_Text uiNoAmmoText;     // Drag "noAmmoText" here
    public GameObject uiReticle;      // Drag "AimReticle" here

    private bool hasGun = false;
    private int currentWeaponIndex = 0; // 0 = sword, 1 = gun

    void Start()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        // Start with sword
        sword.SetActive(true);
        if (gun != null) gun.SetActive(false);

        Weapon swordWeapon = sword.GetComponent<Weapon>();
        if (swordWeapon != null)
            playerMovement.EquipWeapon(swordWeapon);
    }

    public void EnableGun(GameObject gunObject)
    {
        gun = gunObject;
        hasGun = true;

        // --- THE FIX: Pass the UI from Player to the new Gun ---
        GunWeapon gunWeapon = gun.GetComponent<GunWeapon>();
        if (gunWeapon != null)
        {
            gunWeapon.ammoText = uiAmmoText;
            gunWeapon.noAmmoText = uiNoAmmoText;
            gunWeapon.reticleUI = uiReticle;
        }
        // -------------------------------------------------------

        gun.SetActive(false); // Keep inactive until equipped
    }

    void Update()
    {
        if (!hasGun) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            currentWeaponIndex = 1 - currentWeaponIndex;

            if (currentWeaponIndex == 0)
            {
                // Equip Sword
                sword.SetActive(true);
                if (gun != null) gun.SetActive(false);

                Weapon swordWeapon = sword.GetComponent<Weapon>();
                if (swordWeapon != null)
                    playerMovement.EquipWeapon(swordWeapon);
            }
            else
            {
                // Equip Gun
                if (gun == null) return;

                // Toggle logic to refresh UI
                gun.SetActive(false);
                gun.SetActive(true);
                sword.SetActive(false);

                Weapon gunWeapon = gun.GetComponent<Weapon>();
                if (gunWeapon != null)
                    playerMovement.EquipWeapon(gunWeapon);
            }
        }
    }
}