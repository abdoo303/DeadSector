using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public float distance = 5f;
    public float height = 2f;

    [Header("Mouse Control")]
    public float mouseSensitivity = 3f;
    public float aimMouseSensitivity = 2f;
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

    [Header("Smoothing")]
    public float positionSmoothing = 10f;
    public float rotationSmoothing = 5f;

    [Header("Aiming Settings")]
    public float normalDistance = 7f;
    public float aimDistance = 4.5f;
    public float normalHeight = 2.5f;
    public float aimHeight = 2f;
    public float zoomSpeed = 8f;

    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private Vector3 currentVelocity;
    private PlayerMovement playerMovement;

    void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            currentYaw = angles.y;
            currentPitch = angles.x;
            playerMovement = target.GetComponent<PlayerMovement>();
        }

        normalDistance = distance;
        normalHeight = height;
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool isAiming = playerMovement != null && playerMovement.GetComponent<Animator>().GetBool("Aiming");

        // Mouse input for camera rotation
        float sensitivity = isAiming ? aimMouseSensitivity : mouseSensitivity;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // Smoothly transition distance and height when aiming
        float targetDistance = isAiming ? aimDistance : normalDistance;
        float targetHeight = isAiming ? aimHeight : normalHeight;
        distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * zoomSpeed);
        height = Mathf.Lerp(height, targetHeight, Time.deltaTime * zoomSpeed);

        // Calculate camera position based on angles
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        // Smooth camera movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / positionSmoothing
        );

        // Look at target
        Vector3 lookAtPoint = target.position + Vector3.up * 1.5f;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothing * Time.deltaTime
        );
    }

    public float GetCameraYaw()
    {
        return currentYaw;
    }
}