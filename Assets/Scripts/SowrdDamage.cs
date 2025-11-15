using UnityEngine;
using System.Collections;

public class SwordDamage : MonoBehaviour
{
    public int damage = 1;              // 1 dmg per hit (zombie has 2 health = 2 hits to kill)
    public bool canDamage = false;      // Only true during the swing
    private float damageMultiplier = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;         // Only deal damage while swinging

        ZombieAI zombie = other.GetComponent<ZombieAI>();
        if (zombie != null)
        {
            zombie.TakeDamage(Mathf.RoundToInt(damage * damageMultiplier));
            Debug.Log("Hit zombie!");
        }
    }

    // These will be called from the sword swing animation
    public void StartDamageWindow()
    {
        canDamage = true;
    }
    public IEnumerator ApplyDamageMultiplierSafe(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }

    public void EndDamageWindow()
    {
        canDamage = false;
    }
    private IEnumerator ResetDamageAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        damageMultiplier = 1f;
    }
}
