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
