using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject door;
    public float openRot = 115;
    public float closeRot = 0;
    public float speed = 2;
    public bool opening;

    public Transform player;
    public float activationDistance = 3f;

    public bool reverseRotation = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        float currentY = door.transform.localEulerAngles.y;

        // --- Determine target ---
        float targetRot = opening ? openRot : closeRot;

        if (reverseRotation)
            targetRot = -targetRot;

        // --- Smooth rotation (this prevents infinite rotation) ---
        float newY = Mathf.MoveTowardsAngle(currentY, targetRot, speed * 100 * Time.deltaTime);

        door.transform.localEulerAngles = new Vector3(
            door.transform.localEulerAngles.x,
            newY,
            door.transform.localEulerAngles.z
        );

        // Interaction
        if (distance <= activationDistance && Input.GetKeyDown(KeyCode.F))
        {
            ToggleDoor();
        }
    }

    public void ToggleDoor()
    {
        opening = !opening;
    }
}
