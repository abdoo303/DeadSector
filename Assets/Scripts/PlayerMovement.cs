using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Reference")]
    public Camera mainCamera;

    [Header("Weapon Settings")]
    public string rightHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand";
    public string swordObjectName = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/WeaponHolder";

    public Transform weaponHolder;
    private GameObject swordObject;
    public Weapon currentWeapon;
    [Header("Mouse Rotation Settings")]
    [Range(1f, 1000f)]
    public float mouseRotationSpeed = 360f;
    public float aimRayDistance = 100f;

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private bool hasSword = true;
    private Transform cameraTransform;

    // --- FIX #1: Find the hand in Awake so it's ready immediately ---
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Find the right hand transform immediately
        weaponHolder = transform.Find(rightHandPath);
        if (weaponHolder == null)
        {
            weaponHolder = FindDeepChild(transform, rightHandPath);
            if (weaponHolder == null)
            {
                Debug.LogError("Could not find RightHand! Check rightHandPath.");
            }
        }
    }

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        if (mainCamera != null)
            cameraTransform = mainCamera.transform;
        else
            Debug.LogError("No camera found!");

        // Find the sword object 
        if (weaponHolder != null)
        {
            Transform swordTransform = weaponHolder.Find(swordObjectName);
            if (swordTransform != null)
            {
                swordObject = swordTransform.gameObject;
                swordObject.SetActive(true);
            }
        }

        // Start with sword animation
        animator.SetInteger("WeaponType", 0);
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;
            Transform result = FindDeepChild(child, childName);
            if (result != null) return result;
        }
        return null;
    }

    void Update()
    {
        HandleMovement();
        HandleActions();
    }

    void HandleMovement()
    {
        if (cameraTransform == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Check if player is aiming
        bool isAiming = Input.GetButton("Fire2");

        // ALWAYS rotate the player body to face the camera direction
        Vector3 lookDir = cameraTransform.forward;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                mouseRotationSpeed * Time.deltaTime
            );
        }

        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            // Calculate movement direction relative to camera
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDir = (forward * v + right * h).normalized;

            controller.Move(moveDir * speed * Time.deltaTime);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (controller.isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleActions()
    {
        if (currentWeapon != null)
        {
            if (Input.GetButtonDown("Fire1")) currentWeapon.PrimaryAction(animator);
            if (Input.GetButtonDown("Fire2")) currentWeapon.SecondaryAction(animator);
            if (Input.GetButtonUp("Fire2") && currentWeapon is GunWeapon gun) gun.StopAiming(animator);
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        // 1. Check if we are re-equipping the same weapon. If so, just ensure it's active and leave.
        if (currentWeapon == newWeapon)
        {
            if (currentWeapon.weaponModel != null) currentWeapon.weaponModel.SetActive(true);
            else currentWeapon.gameObject.SetActive(true);

            SetWeaponAnimationState();
            return;
        }

        hasSword = true;

        // 2. Hide old weapon model (ONLY if it is different from the new one)
        if (currentWeapon != null && currentWeapon.weaponModel != null)
        {
            currentWeapon.weaponModel.SetActive(false);
        }

        // 3. Assign new weapon
        currentWeapon = newWeapon;

        // 4. Position the weapon
        if (weaponHolder != null)
        {
            currentWeapon.transform.SetParent(weaponHolder);
            // Remember: We removed the "Vector3.zero" reset lines so your Gun position stays correct.
        }
        else
        {
            currentWeapon.transform.SetParent(transform);
        }

        // 5. Ensure the new weapon is actually visible
        if (currentWeapon.weaponModel != null)
            currentWeapon.weaponModel.SetActive(true);
        else
            currentWeapon.gameObject.SetActive(true);

        SetWeaponAnimationState();
    }

    void SetWeaponAnimationState()
    {
        if (currentWeapon != null)
        {
            if (currentWeapon is GunWeapon) animator.SetInteger("WeaponType", 1);
            else if (currentWeapon is SwordWeapon) animator.SetInteger("WeaponType", 0);
        }
        else if (hasSword)
        {
            animator.SetInteger("WeaponType", 0);
        }
    }

}