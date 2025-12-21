using TMPro;
using UnityEngine;

using UnityEngine;
using System.Collections;
using TMPro;

public class GunWeapon : Weapon
{
    [Header("Gun Stats")]
    public float range = 100f;

    [Header("Ammo Settings")]
    public int magSize = 10;
    public int ammoInMag = 10;
    public int reserveAmmo = 20;
    public float reloadTime = 1.5f;

    [Header("Visual References")]
    public Camera aimCamera;
    public LineRenderer bulletLine;
    public GameObject reticleUI;
    public TMP_Text ammoText;
    public TMP_Text noAmmoText;

    [Header("Aiming Settings")]
    public float defaultFOV = 60f;
    public float zoomedFOV = 40f;
    public float zoomSpeed = 8f;

    [Header("Aiming Alignment (Iron Sights)")]
    public Vector3 aimPosition;
    public Vector3 aimRotation;
    public float aimSmoothSpeed = 10f;

    private bool isReloading = false;
    private bool isAiming = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Animator cachedAnimator;

    void Start()
    {
        weaponName = "Gun";

        // Auto-find Camera (Cameras are usually active, so Find works here)
        if (aimCamera == null) aimCamera = Camera.main;
        if (aimCamera != null) defaultFOV = aimCamera.fieldOfView;

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void OnEnable()
    {
        // Turn UI ON
        if (ammoText != null) ammoText.gameObject.SetActive(true);
        if (noAmmoText != null) noAmmoText.gameObject.SetActive(false); // Default to off until empty
        // Note: We don't turn on Reticle here, because you only want it when aiming right-click

        UpdateAmmoUI();
        isReloading = false;
    }

    void OnDisable()
    {
        // Turn UI OFF
        if (ammoText != null) ammoText.gameObject.SetActive(false);
        if (noAmmoText != null) noAmmoText.gameObject.SetActive(false);
        if (reticleUI != null) reticleUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) TryReload();
        if (Input.GetKeyDown(KeyCode.L)) SimulateZombieKill();

        HandleZoom();
        HandleReticleFollow();
        HandleGunPosition();
        UpdateNoAmmoWarning();
    }


    public override void PrimaryAction(Animator animator)
    {
        cachedAnimator = animator;
        if (isReloading) return;
        if (ammoInMag <= 0) { UpdateNoAmmoWarning(); return; }

        ammoInMag--;
        UpdateAmmoUI();
        animator.SetTrigger("Fire");
        Shoot();
    }

    public override void SecondaryAction(Animator animator)
    {
        isAiming = true;
        animator.SetBool("Aiming", true);
        if (reticleUI != null) reticleUI.SetActive(true);

        // Hide player body when aiming
        HidePlayerBody(true);
    }

    public void StopAiming(Animator animator)
    {
        isAiming = false;
        animator.SetBool("Aiming", false);
        if (reticleUI != null) reticleUI.SetActive(false);

        // Show player body when not aiming
        HidePlayerBody(false);
    }

    void HidePlayerBody(bool hide)
    {
        // Find player's skinned mesh renderers (body parts)
        Transform player = transform.root; // Get the root player object
        SkinnedMeshRenderer[] renderers = player.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            // Hide body but keep weapon visible
            if (!renderer.gameObject.name.Contains("Weapon") &&
                !renderer.gameObject.name.Contains("Gun") &&
                !renderer.gameObject.name.Contains("Sword"))
            {
                renderer.shadowCastingMode = hide ? UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly : UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }

    void Shoot()
    {
        if (aimCamera == null) return;

        // Shoot from center of screen when aiming, otherwise from mouse position
        Vector3 shootPoint = isAiming ? new Vector3(Screen.width / 2f, Screen.height / 2f, 0f) : Input.mousePosition;

        Ray ray = aimCamera.ScreenPointToRay(shootPoint);
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit, range) ? hit.point : ray.GetPoint(range);

        // Check if we hit an enemy
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(20f); // Gun damage
                Debug.Log($"Gun hit {hit.collider.name}!");
            }
        }

        StartCoroutine(RenderTrace(targetPoint));
    }

    IEnumerator RenderTrace(Vector3 hitPoint)
    {
        if (bulletLine != null)
        {
            bulletLine.enabled = true;
            bulletLine.SetPosition(0, transform.position);
            bulletLine.SetPosition(1, hitPoint);
            yield return new WaitForSeconds(0.1f);
            bulletLine.enabled = false;
        }
    }

    void TryReload()
    {
        if (isReloading || ammoInMag >= magSize || reserveAmmo <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (cachedAnimator != null) cachedAnimator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        int needed = magSize - ammoInMag;
        int taken = Mathf.Min(needed, reserveAmmo);
        ammoInMag += taken;
        reserveAmmo -= taken;
        isReloading = false;
        UpdateAmmoUI();
    }

    void HandleZoom()
    {
        if (aimCamera == null) return;
        float targetFOV = isAiming ? zoomedFOV : defaultFOV;
        aimCamera.fieldOfView = Mathf.Lerp(aimCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    void HandleGunPosition()
    {
        Vector3 targetPos = isAiming ? aimPosition : originalPosition;
        Quaternion targetRot = isAiming ? Quaternion.Euler(aimRotation) : originalRotation;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * aimSmoothSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * aimSmoothSpeed);
    }

    void HandleReticleFollow()
    {
        if (!isAiming || reticleUI == null) return;
        // Center reticle when aiming
        reticleUI.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = ammoInMag + "/" + reserveAmmo;
    }

    void UpdateNoAmmoWarning()
    {
        if (noAmmoText == null) return;
        bool show = (ammoInMag == 0 && reserveAmmo == 0);
        if (noAmmoText.gameObject.activeSelf != show) noAmmoText.gameObject.SetActive(show);
    }

    void SimulateZombieKill()
    {
        AddAmmo(10);
        Debug.Log("Zombie killed. Ammo added.");
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
        UpdateAmmoUI();
    }

    public override string GetIdleAnimationState()
    {
        return "CarryingGunIdle";
    }
}