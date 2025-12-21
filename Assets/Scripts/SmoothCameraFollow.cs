using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 normalTargetOffset = new Vector3(0.5f, 1.5f, 0f); // Right shoulder offset
    public Vector3 aimTargetOffset = new Vector3(0.6f, 1.5f, 0.3f); // Look over shoulder + gun when aiming

    [Header("Camera Distance")]
    [Range(2f, 15f)]
    public float normalDistance = 4f;
    [Range(1f, 10f)]
    public float aimDistance = 1.2f; // Very close for FPS-like view
    [Range(1f, 10f)]
    public float minDistance = 2f;
    [Range(5f, 20f)]
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    [Header("Mouse Controls")]
    [Range(1f, 10f)]
    public float mouseSensitivityX = 3f;
    [Range(1f, 10f)]
    public float mouseSensitivityY = 2f;
    [Range(1f, 10f)]
    public float aimSensitivityX = 1.5f; // Lower sensitivity when aiming
    [Range(1f, 10f)]
    public float aimSensitivityY = 1f;
    [Range(-89f, 0f)]
    public float minVerticalAngle = -30f;
    [Range(0f, 89f)]
    public float maxVerticalAngle = 80f;

    [Header("Smoothing")]
    [Range(1f, 30f)]
    public float positionDamping = 10f;
    [Range(1f, 30f)]
    public float rotationDamping = 10f;

    [Header("Collision")]
    public bool enableCollision = true;
    public LayerMask collisionLayers = -1;
    public float collisionBuffer = 0.3f;

    [Header("Auto-Rotation")]
    public bool autoRotateBehindPlayer = false;
    public float autoRotateSpeed = 2f;
    public float autoRotateDelay = 2f;

    // Private variables
    private float currentX = 0f;
    private float currentY = 20f;
    private float currentDistance;
    private Vector3 currentVelocity;
    private float autoRotateTimer = 0f;
    private bool isAiming = false;
    private float targetDistance;
    private Vector3 targetOffset;

    void Start()
    {
        currentDistance = normalDistance;
        targetDistance = normalDistance;
        targetOffset = normalTargetOffset;

        // Initialize rotation to look at player from behind
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            currentX = angles.y;
            currentY = angles.x;
        }

        // Lock and hide cursor for better gaming experience
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Check if aiming (right mouse button held)
        isAiming = Input.GetButton("Fire2");

        HandleMouseInput();
        HandleZoom();
        UpdateCameraPosition();
    }

    void HandleMouseInput()
    {
        // Use different sensitivity based on aiming state
        float sensX = isAiming ? aimSensitivityX : mouseSensitivityX;
        float sensY = isAiming ? aimSensitivityY : mouseSensitivityY;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * sensY;

        // Reset auto-rotate timer on mouse movement
        if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
        {
            autoRotateTimer = 0f;
        }

        // Update horizontal rotation (always active)
        currentX += mouseX;

        // Update vertical rotation (ONLY when aiming)
        if (isAiming)
        {
            currentY -= mouseY;
            // Clamp vertical rotation
            currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        }

        // Auto-rotate behind player if enabled (but not when aiming)
        if (autoRotateBehindPlayer && !isAiming)
        {
            autoRotateTimer += Time.deltaTime;
            if (autoRotateTimer > autoRotateDelay)
            {
                float targetX = target.eulerAngles.y;
                currentX = Mathf.LerpAngle(currentX, targetX, Time.deltaTime * autoRotateSpeed);
            }
        }
    }

    void HandleZoom()
    {
        // Set target distance based on aiming state
        targetDistance = isAiming ? aimDistance : normalDistance;

        // Mouse scroll wheel zoom (only when not aiming)
        if (!isAiming)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                normalDistance -= scroll * zoomSpeed;
                normalDistance = Mathf.Clamp(normalDistance, minDistance, maxDistance);
                targetDistance = normalDistance;
            }
        }

        // Smooth distance interpolation
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * positionDamping);
    }

    void UpdateCameraPosition()
    {
        // Smoothly transition between normal and aim offset
        Vector3 currentTargetOffset = isAiming ? aimTargetOffset : normalTargetOffset;
        targetOffset = Vector3.Lerp(targetOffset, currentTargetOffset, Time.deltaTime * positionDamping);

        // Calculate pivot point (target position + offset)
        Vector3 pivotPoint = target.position + targetOffset;

        // Calculate desired rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);

        // Calculate desired position (behind and above the pivot)
        Vector3 desiredPosition = pivotPoint - (rotation * Vector3.forward * currentDistance);

        // Handle collision
        if (enableCollision)
        {
            RaycastHit hit;
            Vector3 direction = desiredPosition - pivotPoint;
            float targetDistance = direction.magnitude;

            if (Physics.SphereCast(pivotPoint, collisionBuffer, direction.normalized, out hit, targetDistance, collisionLayers))
            {
                desiredPosition = pivotPoint + direction.normalized * (hit.distance - collisionBuffer);
            }
        }

        // Smooth position movement
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / positionDamping);

        // Smooth rotation
        Quaternion targetRotation = Quaternion.LookRotation(pivotPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);
    }

    // Toggle cursor lock/unlock with Escape key
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}