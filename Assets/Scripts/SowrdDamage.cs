using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 1;              // 1 dmg per hit (zombie has 2 health = 2 hits to kill)
    public bool canDamage = false;      // Only true during the swing

    void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;         // Only deal damage while swinging

        ZombieAI zombie = other.GetComponent<ZombieAI>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            Debug.Log("Hit zombie!");
        }
    }

    // These will be called from the sword swing animation
    public void StartDamageWindow()
    {
        canDamage = true;
    }

    public void EndDamageWindow()
    {
        canDamage = false;
    }
}
