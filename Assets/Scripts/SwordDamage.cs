using UnityEngine;
using System.Collections;

public class SwordDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float slashDamagePercent = 0.20f;
    public float stabDamagePercent = 0.30f;
    public float attackDistance = 3f;
    public float attackAngle = 30f;

    [Header("Ammo Reward")]
    public int ammoPerKill = 10;

    private float damageMultiplier = 1f;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
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

        Debug.Log($"🔍 Found {hitColliders.Length} colliders in range");

        foreach (Collider col in hitColliders)
        {
            Debug.Log($"🔍 Checking collider: {col.name}, Tag: {col.tag}");

            if (col.CompareTag("Enemy"))
            {
                if (IsTargetInAttackCone(col.transform))
                {
                    Health zombieHealth = col.GetComponent<Health>();

                    if (zombieHealth != null)
                    {
                        float damage = zombieHealth.maxHealth * damagePercent * damageMultiplier;

                        // Check if this will kill the zombie
                        bool willKill = zombieHealth.currentHealth <= damage;

                        zombieHealth.TakeDamage(damage);

                        if (CombatSounds.Instance != null)
                        {
                            CombatSounds.Instance.PlayZombieAttackedSound();
                        }

                        // Give ammo reward if killed
                        if (willKill)
                        {
                            GiveAmmoReward();
                        }

                        Debug.Log($"✅ Hit {col.name} with {attackType}! Damage: {damage:F1}");
                        hitSomething = true;
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ {col.name} has Enemy tag but NO Health component!");
                    }
                }
                else
                {
                    Debug.Log($"❌ {col.name} not in attack cone");
                }
            }
        }

        if (!hitSomething)
        {
            Debug.Log($"❌ {attackType} missed - no valid enemies hit");
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
        if (CombatSounds.Instance != null)
        {
            if (Random.value > 0.5f) CombatSounds.Instance.PlaySwordSound1();
            else CombatSounds.Instance.PlaySwordSound2();
        }
    }

    private void PlayStabSound()
    {
        if (CombatSounds.Instance != null)
        {
            CombatSounds.Instance.PlaySwordSound1();
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

    // Permanently increase damage by multiplying the current multiplier
    public void IncreaseDamagePermanently(float factor)
    {
        damageMultiplier *= factor;
        Debug.Log($"⚡ Damage permanently increased! New multiplier: {damageMultiplier}x");
    }

    // Give ammo reward to the player's gun
    private void GiveAmmoReward()
    {
        if (playerTransform == null) return;

        // Find the gun weapon
        GunWeapon gun = playerTransform.GetComponentInChildren<GunWeapon>(true); // true = include inactive
        if (gun != null)
        {
            gun.AddAmmo(ammoPerKill);
            Debug.Log($"💰 Sword kill! Gained {ammoPerKill} ammo");
        }
    }
}