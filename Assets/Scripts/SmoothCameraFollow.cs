using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The player transform
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Camera position relative to player
    
    [Header("Smoothing")]
    public float positionSmoothing = 10f; // Higher = snappier follow
    public float rotationSmoothing = 5f; // Rotation follow speed
    
    [Header("Optional: Look At Point")]
    public bool useLookAtPoint = true;
    public Vector3 lookAtOffset = new Vector3(0f, 1.5f, 0f); // Point on player to look at (chest height)
    
    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        
        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref currentVelocity, 
            1f / positionSmoothing
        );

        // Optionally look at a point on the player
        if (useLookAtPoint)
        {
            Vector3 lookAtPoint = target.position + lookAtOffset;
            Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSmoothing * Time.deltaTime
            );
        }
    }
}