using UnityEngine;

public class SwordWeapon : Weapon
{
    void Start()
    {
        weaponName = "Sword";
    }

    public override void PrimaryAction(Animator animator)
    {
        animator.SetTrigger("Slash"); // or Stab
    }

    public override void SecondaryAction(Animator animator)
    {
        animator.SetTrigger("Stab"); // sword right click
    }

    public override string GetIdleAnimationState()
    {
        return "CarryingSwordIdle";
    }
}
