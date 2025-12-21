using UnityEngine;

public class CombatSounds : MonoBehaviour
{
    public static CombatSounds Instance;

    [Header("Sound Effects")]
    public AudioClip swordSwingSound1;
    public AudioClip swordSwingSound2;
    public AudioClip zombieHitSound;
    public AudioClip zombieAttackSound;
    public AudioClip bossAttackSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySwordSound1()
    {
        if (swordSwingSound1 != null)
            audioSource.PlayOneShot(swordSwingSound1);
    }

    public void PlaySwordSound2()
    {
        if (swordSwingSound2 != null)
            audioSource.PlayOneShot(swordSwingSound2);
    }

    public void PlayZombieAttackedSound()
    {
        if (zombieHitSound != null)
            audioSource.PlayOneShot(zombieHitSound);
    }

    public void PlayZombieAttackSound()
    {
        if (zombieAttackSound != null)
            audioSource.PlayOneShot(zombieAttackSound);
    }

    public void PlayBossAttackSound()
    {
        if (bossAttackSound != null)
            audioSource.PlayOneShot(bossAttackSound);
    }
}