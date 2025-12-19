using UnityEngine;

public class CombatSounds : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioClip swordSwingSound1;
    public AudioClip swordSwingSound2;
    public AudioClip zombieHitSound;
    
    private AudioSource audioSource;

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
        audioSource.PlayOneShot(swordSwingSound1);
    }


    public void PlaySwordSound2()
    {
        audioSource.PlayOneShot(swordSwingSound2);
    }
    public void PlayZombieAttackedSound()
    {
        audioSource.PlayOneShot(zombieHitSound);
    }
}