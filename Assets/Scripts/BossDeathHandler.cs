using UnityEngine;
using UnityEngine.AI;

public class BossDeathHandler : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip deathSound;
    public OutroSequence outroSequence;

    // NEW: Reference to the Brain so we can turn it off
    public BossController bossController;

    private Health health;
    private Collider[] allColliders;
    private NavMeshAgent navAgent;

    void Start()
    {
        // 1. Find the Health script
        health = GetComponent<Health>();
        if (health == null) health = GetComponentInParent<Health>();

        // 2. Find Navigation
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null) navAgent = GetComponentInParent<NavMeshAgent>();

        // 3. Find Colliders
        allColliders = GetComponentsInChildren<Collider>();

        // 4. NEW: Find the BossController (Brain)
        if (bossController == null) bossController = GetComponent<BossController>();

        // 5. Subscribe to death
        if (health != null)
        {
            health.OnDied += HandleDeath;
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void OnDestroy()
    {
        if (health != null) health.OnDied -= HandleDeath;
    }

    void HandleDeath()
    {
        Debug.Log("☠️ BOSS DIED!");

        // 1. NEW: KILL THE BRAIN
        // This stops him from trying to attack you while he is dying
        if (bossController != null)
        {
            bossController.StopAllCoroutines(); // Cancel any queued attacks
            bossController.enabled = false;     // Turn off the AI
        }

        // 2. Hide from systems
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");

        // 3. Play Animation
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            // OR animator.SetTrigger("Die"); // Use whichever you set up in Animator

            StartCoroutine(FreezeAnimatorRoutine(3.0f));
        }

        // 4. Stop Moving (Uncommented and Fixed)
        if (navAgent != null)
        {
            // SAFETY CHECK: Only stop if it's actually active to prevent errors
            if (navAgent.isActiveAndEnabled)
            {
                navAgent.isStopped = true;
                navAgent.velocity = Vector3.zero;
            }
            navAgent.enabled = false; // Disable entirely
        }

        // 5. Disable Colliders
        foreach (Collider col in allColliders)
        {
            col.enabled = false;
        }

        // 6. Play Sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (outroSequence != null)
        {
            Debug.Log("Signal sent to Outro Sequence.");
            outroSequence.StartOutro();
        }
        else
        {
            Debug.LogError("Assign the OutroSequence script to the BossDeathHandler!");
        }
    }

    System.Collections.IEnumerator FreezeAnimatorRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (animator != null)
        {
            animator.enabled = false; // Freezes the mesh in place
        }
    }
}