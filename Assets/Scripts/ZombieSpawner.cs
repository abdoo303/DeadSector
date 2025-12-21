using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public Transform player;

    [Header("Spawn Settings")]
    [Tooltip("If true, spawns one zombie per spawn point. If false, uses numberToSpawn.")]
    public bool spawnOnePerPoint = true;
    [Tooltip("Only used if spawnOnePerPoint is false")]
    public int numberToSpawn = 10;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || zombiePrefab == null) return;

        if (spawnOnePerPoint)
        {
            // Spawn exactly one zombie at each spawn point
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                SpawnZombieAtPoint(spawnPoints[i]);
            }
        }
        else
        {
            // Old behavior: spawn random number at random points
            for (int i = 0; i < numberToSpawn; i++)
            {
                Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
                SpawnZombieAtPoint(sp);
            }
        }
    }

    void SpawnZombieAtPoint(Transform spawnPoint)
    {
        GameObject z = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        var ai = z.GetComponent<ZombieAI>();
        if (ai != null) ai.targetPlayer = player;

        NavMeshAgent agent = z.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.avoidancePriority = Random.Range(20, 80);
        }
    }
}