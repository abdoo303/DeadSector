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
    
    private Transform weaponHolder;
    private GameObject swordObject;
    public Weapon currentWeapon;
    
    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private bool hasSword = true;
    private Transform cameraTransform;
    private bool isDead = false;
    
    private Health healthComponent;
    private SwordDamage swordDamage;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.OnDied += OnPlayerDied;
        }

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null) cameraTransform = mainCamera.transform;

        weaponHolder = transform.Find(rightHandPath);
        if (weaponHolder == null) weaponHolder = FindDeepChild(transform, rightHandPath);

        if (weaponHolder != null)
        {
            Transform swordTransform = weaponHolder.Find(swordObjectName);
            if (swordTransform != null)
            {
                swordObject = swordTransform.gameObject;
                swordObject.SetActive(true);
                swordDamage = swordObject.GetComponentInChildren<SwordDamage>();
            }
        }

        animator.SetInteger("WeaponType", 0);
    }

    void OnDestroy()
    {
        if (healthComponent != null) healthComponent.OnDied -= OnPlayerDied;
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
        if (isDead) return;
        HandleMovement();
        HandleActions();
    }

    void HandleMovement()
    {
        if (cameraTransform == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            
            Vector3 moveDir = (forward * v + right * h).normalized;
            
            if (moveDir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.Euler(0f, targetAngle, 0f),
                    360f * Time.deltaTime
                );
            }
            
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
        if (Input.GetButtonDown("Fire1"))
        {
            if (swordDamage != null) swordDamage.PerformSlashAttack();
            animator.SetTrigger("Slash");
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (swordDamage != null) swordDamage.PerformStabAttack();
            animator.SetTrigger("Stab");
        }
    }

    private void OnPlayerDied()
    {
        isDead = true;
        animator.SetFloat("Speed", 0f);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (swordObject != null)
        {
            swordObject.SetActive(false);
            hasSword = false;
        }

        if (currentWeapon != null)
        {
            if (currentWeapon.weaponModel != null) currentWeapon.weaponModel.SetActive(false);
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = newWeapon;
        
        if (weaponHolder != null)
        {
            currentWeapon.transform.SetParent(weaponHolder);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
            currentWeapon.transform.localScale = Vector3.one;
        }
        else
        {
            currentWeapon.transform.SetParent(transform);
            currentWeapon.transform.localPosition = Vector3.zero;
        }

        if (currentWeapon.weaponModel != null) currentWeapon.weaponModel.SetActive(true);
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