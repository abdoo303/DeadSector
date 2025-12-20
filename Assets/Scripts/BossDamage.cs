using UnityEngine;

public class BossDamage : MonoBehaviour
{
    [Header("Settings")]
    public float damageAmount = 20f;
    public float attackRange = 2.0f;
    public Transform attackPoint; // The center of the hit sphere (e.g., the fist)
    public LayerMask playerLayer; // To ensure we only hit the player

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    // This is the function called by the Animation Event
    public void DealDamage()
    {
        // 1. Detect Player in range
        // If attackPoint is not assigned, use the Boss's own position
        Vector3 point = attackPoint != null ? attackPoint.position : transform.position + transform.forward;

        Collider[] hitPlayers = Physics.OverlapSphere(point, attackRange, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            // 2. Check for Health Script
            // We check specific player script or generic Health
            var health = player.GetComponent<Health>();
            // If you have a specific "PlayerHealth" script, assume it inherits or check for it too:
            // var pHealth = player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damageAmount);
                Debug.Log($"🥊 BOSS HIT PLAYER! Damage: {damageAmount}");

                // Play sound
                if (audioSource != null && hitSound != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
        }
    }

    // Visualize the attack range in Scene view
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}