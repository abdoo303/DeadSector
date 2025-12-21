using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;           // Drag your Char_Cyber here
    public NavMeshAgent agent;         // Drag the NavMeshAgent component here
    public Animator animator;          // Drag the Animator component here

    [Header("Stats")]
    public float detectionRange = 20f; // How far he sees you
    public float attackRange = 2.5f;   // How close to stop and hit
    public float timeBetweenAttacks = 2f; // Cooldown
    public float damageAmount = 20f;
    private int attackCount = 0; // 0 = First hit, 1 = Second hit

    private float attackTimer = 0f;
    private bool isDead = false;
    private bool hasDealtDamage = false;

    void Start()
    {
        // Auto-find player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Make sure the agent doesn't rotate automatically (we do it manually for smoothness)
        agent.updateRotation = false;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. ROTATION: Always look at the player smoothly
        if (distanceToPlayer <= detectionRange)
        {
            RotateTowardsPlayer();
        }

        // 2. BEHAVIOR: Chase or Attack
        if (distanceToPlayer <= attackRange)
        {
            // STOP AND ATTACK
            agent.isStopped = true;
            animator.SetBool("IsRunning", false);

            if (attackTimer <= 0 && !hasDealtDamage)
            {
                PerformAttack();
                DealDamageToPlayer();
                attackTimer = timeBetweenAttacks;
                hasDealtDamage = true;
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // CHASE
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("IsRunning", true);
            hasDealtDamage = false;
        }
        else
        {
            // IDLE (Player is too far)
            agent.isStopped = true;
            animator.SetBool("IsRunning", false);
            hasDealtDamage = false;
        }

        // Cooldown timer
        if (attackTimer > 0) attackTimer -= Time.deltaTime;
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void PerformAttack()
    {
        // Check if the number is even or odd
        if (attackCount % 2 == 0)
        {
            // Even number (0, 2, 4...) -> First Attack
            animator.SetTrigger("Attack1");
            Debug.Log("Boss uses Heavy Smash (Attack 1)!");
        }
        else
        {
            // Odd number (1, 3, 5...) -> Second Attack
            animator.SetTrigger("Attack2");
            Debug.Log("Boss uses Quick Slash (Attack 2)!");
        }

        // Increase the counter for next time
        attackCount++;
    }

    void DealDamageToPlayer()
    {
        if (isDead || player == null) return;

        // Play attack sound using CombatSounds singleton
        if (CombatSounds.Instance != null)
        {
            CombatSounds.Instance.PlayBossAttackSound();
        }

        // Deal damage using Health script
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log($"Boss dealt {damageAmount} damage to player!");
        }
    }

    // Call this from the Health Script when HP reaches 0
    public void OnDeath()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetBool("Die", true);

        // Disable collision so player can walk through corpse
        GetComponent<Collider>().enabled = false;
        agent.enabled = false;
    }
}