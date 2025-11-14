using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    [Header("Weapon Settings")]
    [Tooltip("Path to right hand in hierarchy, e.g., 'Armature/Hips/Spine/RightShoulder/RightArm/RightHand'")]
    public string rightHandPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"; 
    
    [Tooltip("Name of your sword GameObject under RightHand")]
    public string swordObjectName = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/WeaponHolder"; // Name of sword GameObject in scene
    //Char_cyber/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/
    private Transform weaponHolder; // Will be found at runtime
    private GameObject swordObject; // Will be found at runtime
    public Weapon currentWeapon; // The currently equipped weapon (starts null)
    
    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private bool hasSword = true; // Start with sword

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Find the right hand transform at runtime
        weaponHolder = transform.Find(rightHandPath);
        if (weaponHolder == null)
        {
            // Try alternative search methods
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0f, v).normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * Time.deltaTime);
            animator.SetFloat("Speed", 1f);
        }
        else animator.SetFloat("Speed", 0f);

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
        // // Handle sword attacks when carrying sword
        // if (hasSword && currentWeapon == null)
        // {
            if (Input.GetButtonDown("Fire1")) // Or your attack input
            {
                animator.SetTrigger("Stab");
            }
        // }
        // Handle gun fire when carrying gun
        // else if (currentWeapon != null && currentWeapon is GunWeapon)
        // {
        //     if (Input.GetButtonDown("Fire1"))
        //     {
        //         currentWeapon.PrimaryAction(animator);
        //     }
        // }
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
                animator.SetInteger("WeaponType", 1); // Switch to gun idle animation
                Debug.Log("Animation switched to Gun (WeaponType = 1)");
            }
            else if (currentWeapon is SwordWeapon)
            {
                animator.SetInteger("WeaponType", 0); // Switch to sword idle animation
                Debug.Log("Animation switched to Sword (WeaponType = 0)");
            }
        }
        else if (hasSword)
        {
            // Default to sword animation if no weapon equipped but sword is active
            animator.SetInteger("WeaponType", 0);
        }
    }
}