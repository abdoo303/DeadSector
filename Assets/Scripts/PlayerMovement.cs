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
    public Transform weaponHolder; // Empty GameObject where weapons will be parented
    public Weapon currentWeapon; // The currently equipped weapon

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Initialize with starting weapon (sword) if it exists
        if (currentWeapon != null)
        {
            SetWeaponAnimationState();
        }
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
        if (currentWeapon != null)
        {
            // Use appropriate input for your game (left mouse button shown here)
            if (Input.GetButtonDown("Fire1")) // Or use GetKeyDown for specific key
            {
                currentWeapon.PrimaryAction(animator);
            }
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        // Unequip current weapon
        if (currentWeapon != null)
        {
            // Optionally: Drop current weapon or destroy it
            if (currentWeapon.weaponModel != null)
                currentWeapon.weaponModel.SetActive(false);
            
            Destroy(currentWeapon.gameObject);
        }

        // Equip new weapon
        currentWeapon = newWeapon;
        
        // Parent weapon to the weapon holder
        if (weaponHolder != null)
        {
            currentWeapon.transform.SetParent(weaponHolder);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
        else
        {
            currentWeapon.transform.SetParent(transform);
        }

        // Show weapon model
        if (currentWeapon.weaponModel != null)
            currentWeapon.weaponModel.SetActive(true);

        // Update animation state
        SetWeaponAnimationState();

        Debug.Log("Equipped: " + currentWeapon.weaponName);
    }

    void SetWeaponAnimationState()
    {
        if (currentWeapon != null)
        {
            // You can use different methods depending on your animation setup:
            
            // Method 1: Using an integer parameter in animator
            if (currentWeapon is SwordWeapon)
            {
                animator.SetInteger("WeaponType", 0); // 0 = Sword
            }
            else if (currentWeapon is GunWeapon)
            {
                animator.SetInteger("WeaponType", 1); // 1 = Gun
            }

            // Method 2: Using a string parameter
            // animator.SetString("WeaponState", currentWeapon.GetIdleAnimationState());

            // Method 3: Using a boolean
            // animator.SetBool("IsCarryingGun", currentWeapon is GunWeapon);
        }
    }
}