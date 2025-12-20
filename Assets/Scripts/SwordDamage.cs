using UnityEngine;
using System.Collections;

public class SwordDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float slashDamagePercent = 0.20f;
    public float stabDamagePercent = 0.30f;
    public float attackDistance = 3f;
    public float attackAngle = 30f;

    private float damageMultiplier = 1f;
    private Transform playerTransform;
    private CombatSounds combatSounds;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        combatSounds = FindObjectOfType<CombatSounds>();
    }

    public void PerformSlashAttack()
    {
        Debug.Log("⚔️ SLASH ATTACK!");
        PlaySlashSound();
        DealDamage(slashDamagePercent, "SLASH");
    }

    public void PerformStabAttack()
    {
        Debug.Log("🗡️ STAB ATTACK!");
        PlayStabSound();
        DealDamage(stabDamagePercent, "STAB");
    }

    private void DealDamage(float damagePercent, string attackType)
    {
        if (playerTransform == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, attackDistance);
        bool hitSomething = false;

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                if (IsTargetInAttackCone(col.transform))
                {
                    Health zombieHealth = col.GetComponent<Health>();

                    if (zombieHealth != null)
                    {
                        float damage = zombieHealth.maxHealth * damagePercent * damageMultiplier;
                        zombieHealth.TakeDamage(damage);

                        if (combatSounds != null)
                        {
                            combatSounds.PlayZombieAttackedSound();
                        }

                        Debug.Log($"✅ Hit {col.name} with {attackType}! Damage: {damage:F1}");
                        hitSomething = true;
                    }
                }
            }
        }

        if (!hitSomething)
        {
            Debug.Log($"❌ {attackType} missed");
        }
    }

    private bool IsTargetInAttackCone(Transform target)
    {
        Vector3 directionToTarget = target.position - playerTransform.position;
        float distance = directionToTarget.magnitude;

        if (distance > attackDistance) return false;

        directionToTarget.y = 0;
        Vector3 forward = playerTransform.forward;
        forward.y = 0;

        float angle = Vector3.Angle(forward, directionToTarget);

        return angle <= attackAngle;
    }

    private void PlaySlashSound()
    {
        if (combatSounds != null)
        {
            if (Random.value > 0.5f) combatSounds.PlaySwordSound1();
            else combatSounds.PlaySwordSound2();
        }
    }

    private void PlayStabSound()
    {
        if (combatSounds != null)
        {
            combatSounds.PlaySwordSound1();
        }
    }

    public void ApplyDamageMultiplierSafe(float multiplier, float duration)
    {
        StartCoroutine(ApplyMultiplier(multiplier, duration));
    }

    private IEnumerator ApplyMultiplier(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        Debug.Log($"⚡ Damage multiplier: {multiplier}x for {duration}s!");
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
    }
}