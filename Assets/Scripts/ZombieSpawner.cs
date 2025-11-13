using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public Transform player;
    public int numberToSpawn = 10;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || zombiePrefab == null) return;

        for (int i = 0; i < numberToSpawn; i++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject z = Instantiate(zombiePrefab, sp.position, sp.rotation);

            var ai = z.GetComponent<ZombieAI>();
            if (ai != null) ai.targetPlayer = player;

            NavMeshAgent agent = z.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.avoidancePriority = Random.Range(20, 80);
            }
        }
    }
}
