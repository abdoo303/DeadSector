using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;   // Assign at runtime or auto-find by tag "Player"

    [Header("Combat")]
    public float attackRange = 1.4f;
    public float attackCooldown = 1.2f;
    public int health = 100;

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

        // Optional: occasionally re-randomize offset to look dynamic
        if (Random.value < 0.01f)
        {
            personalOffset = Random.insideUnitSphere * 1.5f;
            personalOffset.y = 0;
        }

        // Move towards the target with personal offset (prevents "basket" pile)
        agent.isStopped = false;
        agent.SetDestination(targetPlayer.position + personalOffset);

        // Update animation movement speed
        animator.SetFloat("MoveSpeed", agent.velocity.magnitude);

        float dist = Vector3.Distance(transform.position, targetPlayer.position);

        // ✅ Simplified attack trigger logic (no cooldown blocking animation)
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

    // 🎯 Called by Animation Event (OnAttackHit)
    public void OnAttackHit()
    {
        if (isDead || targetPlayer == null) return;

        // Only apply damage if close enough
        if (Vector3.Distance(transform.position, targetPlayer.position) > attackRange + 0.3f)
            return;

        // TODO: Deal damage to player
        // Example: targetPlayer.GetComponent<PlayerHealth>()?.TakeDamage(10);
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
