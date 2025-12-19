using UnityEngine;

public class CompanionDrone : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Movement")]
    public Vector3 followOffset = new Vector3(0, 3, -2);
    public float smoothSpeed = 0.125f;
    private Vector3 velocity = Vector3.zero;

    [Header("Combat")]
    public float detectionRange = 10f;
    public float fireRate = 1f;
    public float verticalAimOffset = 1.5f;
    private float fireCountdown = 0f;

    // VARIABLES TO SHARE STATE BETWEEN UPDATE AND FIXEDUPDATE
    private Transform targetEnemy; // The currently detected enemy
    private bool isTargeting = false; // Is an enemy in range?

    private Rigidbody rb;

    void Start()
    {
        // Safety check to ensure the Rigidbody exists
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Drone Rigidbody is missing! Adding one now.");
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            // Set constraints programmatically for safety
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    // --------------------------------------------------------
    // MOVEMENT & AIMING (Physics Loop)
    // --------------------------------------------------------
    void FixedUpdate()
    {
        Vector3 targetPosition;
        Quaternion targetRotation;

        if (isTargeting && targetEnemy != null)
        {
            // ATTACK MODE
            // Hover near player
            targetPosition = player.position + Vector3.up * 3f;

            // --- START CHANGES HERE ---
            // 1. Calculate the enemy's chest/head height
            Vector3 enemyTargetPoint = targetEnemy.position + Vector3.up * verticalAimOffset;

            // 2. Calculate the direction vector from the drone to that chest/head point
            Vector3 directionToEnemy = enemyTargetPoint - transform.position;

            // 3. Create the rotation quaternion
            targetRotation = Quaternion.LookRotation(directionToEnemy);
            // --- END CHANGES HERE ---
        }
        else
        {
            // FOLLOW MODE
            targetPosition = player.position + player.TransformDirection(followOffset);
            targetRotation = Quaternion.LookRotation(player.forward);
        }

        // Apply smooth movement
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
        rb.MovePosition(smoothedPosition);

        // Apply smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    // --------------------------------------------------------
    // DETECTION & SHOOTING (Game Logic Loop)
    // --------------------------------------------------------
    void Update()
    {
        // 1. Detection
        targetEnemy = FindClosestEnemy();
        isTargeting = (targetEnemy != null); // Update the state flag

        // 2. Shooting Logic
        if (isTargeting)
        {
            // Debugging: If this doesn't run, detection failed (Tag/Layer issue)
            // Debug.Log("Attempting to shoot!");

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
        // If not targeting, we just let the timer decrease slower (or stop it)
        else if (fireCountdown > 0f)
        {
            fireCountdown -= Time.deltaTime;
        }
    }

    // --------------------------------------------------------
    // SHOOTING FUNCTION
    // --------------------------------------------------------
    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // This instantiates the bullet using the current rotation set in FixedUpdate
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    // --------------------------------------------------------
    // HELPER FUNCTION
    // --------------------------------------------------------
    Transform FindClosestEnemy()
    {
        // ... (This function stays exactly the same)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    closestEnemy = hit.transform;
                    minDistance = dist;
                }
            }
        }
        return closestEnemy;
    }

    void OnDrawGizmosSelected()
    {
        // ... (The Gizmos function stays the same)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}