using UnityEngine;

public class SwordWeapon : Weapon
{
    void Start()
    {
        weaponName = "Sword";
    }

    public override void PrimaryAction(Animator animator)
    {
        animator.SetTrigger("Stab");
    }

    public override string GetIdleAnimationState()
    {
        return "CarryingSwordIdle"; // Your sword idle animation state name
    }
}