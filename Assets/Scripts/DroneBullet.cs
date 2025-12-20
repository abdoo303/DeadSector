using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    public float speed = 40f;
    public float damage = 5f;
    public float lifeTime = 6f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Drone")) return;

        if (other.CompareTag("Enemy"))
        {
            Health zombieHealth = other.GetComponentInParent<Health>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damage);
                Debug.Log($"Drone dealt {damage} damage!");
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}