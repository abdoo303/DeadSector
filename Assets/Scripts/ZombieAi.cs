using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;   // Assign at runtime or auto-find by tag "Player"

    [Header("Combat")]
    public float attackRange = 1.4f;
    public float attackCooldown = 1.2f;

    private NavMeshAgent agent;
    private Animator animator;
    private Health healthComponent;
    private CombatSounds combatSounds;
    private float cooldownTimer = 0f;
    private bool isDead = false;

    // 🧩 Added field: each zombie gets a unique offset
    private Vector3 personalOffset;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<Health>();

        // Subscribe to death event from Health component
        if (healthComponent != null)
        {
            healthComponent.OnDied += Die;
        }
    }

    void Start()
    {
        // Find target automatically if not assigned
        if (targetPlayer == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) targetPlayer = p.transform;
        }

        // Find combat sounds
        combatSounds = FindObjectOfType<CombatSounds>();

        // Assign a fixed random offset (so they circle instead of piling)
        personalOffset = Random.insideUnitSphere * 1.5f;  // 1.2–2.0 range feels good
        personalOffset.y = 0;

        // Optional: slightly different movement behaviors
        agent.stoppingDistance = 1f;
    }

    void Update()
    {
        if (isDead) return;
        if (targetPlayer == null) return;

        cooldownTimer -= Time.deltaTime;

        if (Random.value < 0.01f)
        {
            personalOffset = Random.insideUnitSphere * 1.5f;
            personalOffset.y = 0;
        }

        // ✅ Only move if agent is on NavMesh
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position + personalOffset);

            // Update animation movement speed
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude);

            float dist = Vector3.Distance(transform.position, targetPlayer.position);

            if (dist <= attackRange)
            {
                agent.isStopped = true;
                animator.SetBool("isAttacking", true);

                // Attack immediately when in range (no animation event needed)
                if (cooldownTimer <= 0f)
                {
                    PerformAttack();
                    cooldownTimer = attackCooldown;
                }
            }
            else
            {
                agent.isStopped = false;
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            // Optional: debug log if agent not on NavMesh
            Debug.LogWarning($"{gameObject.name} is not on a NavMesh!");
        }
    }


    // Attack player immediately (no animation event needed)
    private void PerformAttack()
    {
        if (isDead || targetPlayer == null) return;

        // Only apply damage if close enough
        if (Vector3.Distance(transform.position, targetPlayer.position) > attackRange + 0.3f)
            return;

        // Play zombie attack sound
        if (combatSounds != null)
        {
            combatSounds.PlayZombieAttackedSound();
        }

        // Damage player using unified Health system
        Health playerHealth = targetPlayer.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10f);   // zombie deals 10 damage
            Debug.Log("🧟 Zombie ATTACK HIT! Dealt 10 damage to player.");
        }
        else
        {
            Debug.LogWarning("⚠️ Player doesn't have Health component! Add Health script to player.");
        }
    }

    // Keep this for backwards compatibility if animation events are set up
    public void OnAttackHit()
    {
        PerformAttack();
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple death calls

        isDead = true;

        // Safely stop agent only if it's enabled and on NavMesh
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        // Update animations
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isDead", true);
            animator.SetFloat("MoveSpeed", 0f);
        }

        // Disable NavMeshAgent to avoid pushing dead bodies
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Optional: remove after 5 seconds
        Destroy(gameObject, 5f);
    }
}
