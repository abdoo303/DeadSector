
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public GameObject weaponModel; // The visual model of the weapon
    
    // Override these methods in derived classes
    public abstract void PrimaryAction(Animator animator);
    public abstract string GetIdleAnimationState();
}