using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    public Transform targetPlayer;

    [Header("Combat")]
    public float attackRange = 1.4f;
    public float attackCooldown = 1.2f;
    
    [Header("Damage Settings")]
    public float damageRadius = 2f;
    public float damageAmount = 5f;
    public float damageInterval = 2f;
    private float damageTimer = 0f;

    private NavMeshAgent agent;
    private Animator animator;
    private float cooldownTimer = 0f;
    private bool isDead = false;
    private Vector3 personalOffset;
    private Health playerHealth;
    private Health zombieHealth;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        zombieHealth = GetComponent<Health>();
        if (zombieHealth != null) zombieHealth.OnDied += OnZombieDied;
    }

    void Start()
    {
        if (targetPlayer == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                targetPlayer = p.transform;
                playerHealth = p.GetComponent<Health>();
            }
        }
        else
        {
            playerHealth = targetPlayer.GetComponent<Health>();
        }

        personalOffset = Random.insideUnitSphere * 1.5f;
        personalOffset.y = 0;
        agent.stoppingDistance = 1f;
        damageTimer = damageInterval;
    }

    void OnDestroy()
    {
        if (zombieHealth != null) zombieHealth.OnDied -= OnZombieDied;
    }

    void Update()
    {
        if (isDead || targetPlayer == null) return;

        cooldownTimer -= Time.deltaTime;
        damageTimer -= Time.deltaTime;

        if (Random.value < 0.01f)
        {
            personalOffset = Random.insideUnitSphere * 1.5f;
            personalOffset.y = 0;
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position + personalOffset);
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude);

            float dist = Vector3.Distance(transform.position, targetPlayer.position);

            if (dist <= damageRadius && damageTimer <= 0f && playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                damageTimer = damageInterval;
                Debug.Log($"Zombie dealing {damageAmount} damage to player!");
            }

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
    }

    public void OnAttackHit()
    {
        if (isDead || targetPlayer == null) return;
        if (Vector3.Distance(transform.position, targetPlayer.position) > attackRange + 0.3f) return;
    }

    private void OnZombieDied()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", true);
        animator.SetFloat("MoveSpeed", 0f);
        agent.enabled = false;
        Destroy(gameObject, 5f);
    }
}