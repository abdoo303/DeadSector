using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;   // Assign at runtime or auto-find by tag "Player"

    [Header("Combat")]
    public float attackRange = 1.4f;
    public float attackCooldown = 1.2f;
    public int health = 2;

    private NavMeshAgent agent;
    private Animator animator;
    private float cooldownTimer = 0f;
    private bool isDead = false;

    // 🧩 Added field: each zombie gets a unique offset
    private Vector3 personalOffset;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Find target automatically if not assigned
        if (targetPlayer == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) targetPlayer = p.transform;
        }

        // Assign a fixed random offset (so they circle instead of piling)
        personalOffset = Random.insideUnitSphere * 1.5f;  // 1.2–2.0 range feels good
        personalOffset.y = 0;

        // Optional: slightly different movement behaviors
    }

    void Update()
    {
        if (isDead) return;
        if (targetPlayer == null) return;

        cooldownTimer -= Time.deltaTime;

        // Random offset logic (Keep this if you like it)
        if (Random.value < 0.01f)
        {
            personalOffset = Random.insideUnitSphere * 1.5f;
            personalOffset.y = 0;
        }

        if (agent.isOnNavMesh)
        {
            float dist = Vector3.Distance(transform.position, targetPlayer.position);

            // 1. Check Distance FIRST
            if (dist <= attackRange)
            {
                // CLOSE ENOUGH: Stop and Attack
                agent.isStopped = true;
                animator.SetBool("isAttacking", true);

                // FORCE the run animation to stop immediately
                animator.SetFloat("MoveSpeed", 0f);

                // Optional: Make him look at you while attacking
                Vector3 direction = (targetPlayer.position - transform.position).normalized;
                direction.y = 0; // Keep flat
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }
            else
            {
                // TOO FAR: Run to player
                agent.isStopped = false;
                animator.SetBool("isAttacking", false);

                agent.SetDestination(targetPlayer.position + personalOffset);
                animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
            }
        }
    }

    // 🎯 Called by Animation Event (OnAttackHit)
    public void OnAttackHit()
    {
        if (isDead || targetPlayer == null) return;

        // Only apply damage if close enough
        if (Vector3.Distance(transform.position, targetPlayer.position) > attackRange + 0.3f)
            return;

        PlayerHealth playerHealth = targetPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10);   // zombie deals 10 damage
        }
        Debug.Log("Zombie ATTACK HIT!");
    }

    // 🧠 Simple damage system
    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        health -= dmg;
        if (health <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", true);
        animator.SetFloat("MoveSpeed", 0f);

        // Disable NavMeshAgent to avoid pushing dead bodies
        agent.enabled = false;

        // Optional: remove after 5 seconds
        Destroy(gameObject, 5f);
    }
}
