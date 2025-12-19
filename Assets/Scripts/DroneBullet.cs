using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    // These are public so you can set them on the prefab
    public float speed = 40f;
    public int damage = 10;
    public float lifeTime = 6f;

    private Rigidbody rb;

    // Start runs once when the bullet is created
    void Start()
    {
        // 1. Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Safety check
        if (rb == null)
        {
            Debug.LogError("Bullet is missing Rigidbody component!");
            return;
        }

        // 2. Set the velocity using the object's forward direction
        // This is the CRITICAL change: it hands movement control to the physics engine
        rb.linearVelocity = transform.forward * speed;

        // 3. Destroy the bullet automatically after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }

    // REMOVED: The empty Update() function, since physics handles movement

    void OnTriggerEnter(Collider other)
    {
        // Ignore the Player/Drone (Layers should handle this, but it's a safety check)
        if (other.CompareTag("Player") || other.CompareTag("Drone"))
        {
            return;
        }

        // If we hit an Enemy
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Confirmed Hit by Velocity Bullet on: " + other.name);
            // Damage logic here...
            Destroy(gameObject);
        }

        // Hitting walls/ground (anything that isn't the enemy)
        else
        {
            Destroy(gameObject);
        }
    }
}