using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    
    [Header("Camera Reference")]
    [Tooltip("Assign the Main Camera here")]
    public Camera mainCamera;

    [Header("Weapon Settings")]
    [Tooltip("Path to right hand in hierarchy, e.g., 'Armature/Hips/Spine/RightShoulder/RightArm/RightHand'")]
    public string rightHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"; 
    
    [Tooltip("Name of your sword GameObject under RightHand")]
    public string swordObjectName = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/WeaponHolder";
    
    private Transform weaponHolder;
    private GameObject swordObject;
    public Weapon currentWeapon;
    
    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private bool hasSword = true;
    private Transform cameraTransform; // We'll get this from mainCamera

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Get camera transform from the camera reference
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("No camera found! Please assign a camera to the PlayerMovement script.");
        }

        // Find the right hand transform at runtime
        weaponHolder = transform.Find(rightHandPath);
        if (weaponHolder == null)
        {
            weaponHolder = FindDeepChild(transform, rightHandPath);
            if (weaponHolder == null)
            {
                Debug.LogError("Could not find RightHand! Check rightHandPath: " + rightHandPath);
            }
        }

        // Find the sword object at runtime
        if (weaponHolder != null)
        {
            Transform swordTransform = weaponHolder.Find(swordObjectName);
            if (swordTransform != null)
            {
                swordObject = swordTransform.gameObject;
                swordObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Could not find sword object named: " + swordObjectName + " under " + weaponHolder.name);
            }
        }

        // Start with sword equipped (WeaponType = 0)
        animator.SetInteger("WeaponType", 0);
    }

    // Recursively search for a child by name
    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            
            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
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
        if (cameraTransform == null) return; // Safety check

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            // Calculate direction relative to camera
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            
            // Keep movement on horizontal plane
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            
            // Calculate desired move direction
            Vector3 moveDir = (forward * v + right * h).normalized;
            
            // Rotate character to face movement direction
            if (moveDir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                float turnSpeed = 360f;
                
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.Euler(0f, targetAngle, 0f),
                    turnSpeed * Time.deltaTime
                );
            }
            
            // Move character
            controller.Move(moveDir * speed * Time.deltaTime);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // Gravity and jumping
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
        // Left click = Slash
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Slash");
        }

        // Right click = Stab
        if (Input.GetButtonDown("Fire2"))
        {
            animator.SetTrigger("Stab");
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        // Hide the sword when picking up gun
        if (swordObject != null)
        {
            swordObject.SetActive(false);
            hasSword = false;
        }

        // Destroy old weapon if switching between non-sword weapons
        if (currentWeapon != null)
        {
            if (currentWeapon.weaponModel != null)
                currentWeapon.weaponModel.SetActive(false);
            
            Destroy(currentWeapon.gameObject);
        }

        // Equip new weapon
        currentWeapon = newWeapon;
        
        // Parent weapon to the weapon holder (RightHand)
        if (weaponHolder != null)
        {
            currentWeapon.transform.SetParent(weaponHolder);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
            currentWeapon.transform.localScale = Vector3.one;
        }
        else
        {
            // Fallback: parent to player root
            currentWeapon.transform.SetParent(transform);
            currentWeapon.transform.localPosition = Vector3.zero;
        }

        // Make sure weapon model is visible
        if (currentWeapon.weaponModel != null)
        {
            currentWeapon.weaponModel.SetActive(true);
        }

        // Update animation state to match new weapon
        SetWeaponAnimationState();

        Debug.Log("Equipped: " + currentWeapon.weaponName);
    }

    void SetWeaponAnimationState()
    {
        if (currentWeapon != null)
        {
            // Set animation parameter based on weapon type
            if (currentWeapon is GunWeapon)
            {
                animator.SetInteger("WeaponType", 1);
                Debug.Log("Animation switched to Gun (WeaponType = 1)");
            }
            else if (currentWeapon is SwordWeapon)
            {
                animator.SetInteger("WeaponType", 0);
                Debug.Log("Animation switched to Sword (WeaponType = 0)");
            }
        }
        else if (hasSword)
        {
            animator.SetInteger("WeaponType", 0);
        }
    }
}