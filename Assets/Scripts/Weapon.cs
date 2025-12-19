using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public GameObject weaponModel;

    // Must implement primary action
    public abstract void PrimaryAction(Animator animator);

    // Optional secondary action (default does nothing)
    public virtual void SecondaryAction(Animator animator) { }

    public abstract string GetIdleAnimationState();
}
