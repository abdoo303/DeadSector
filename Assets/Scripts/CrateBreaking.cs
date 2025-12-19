using UnityEngine;
using UnityEngine.UI;

public class CrateInteract : MonoBehaviour
{
    public bool restoresHealth = true;
    public float interactDistance = 3f;
    public Text interactText;

    private Transform player;
    private Health playerHealth;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<Health>();
        }

        if (interactText != null) interactText.enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (interactText != null) interactText.enabled = distance <= interactDistance;

        if (Input.GetKeyDown(KeyCode.E) && distance <= interactDistance)
        {
            InteractWithCrate();
        }
    }

    void InteractWithCrate()
    {
        if (restoresHealth && playerHealth != null)
        {
            playerHealth.HealToMax();
            Debug.Log("✅ Health restored!");
        }

        gameObject.SetActive(false);
    }
}