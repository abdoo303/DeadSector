using UnityEngine;

public class SwordWeapon : Weapon
{
    private SwordDamage swordDamage;

    void Start()
    {
        weaponName = "Sword";

        // Find SwordDamage component on this GameObject
        swordDamage = GetComponent<SwordDamage>();

        if (swordDamage == null)
        {
            Debug.LogError("SwordDamage component not found! Add SwordDamage script to sword GameObject.");
        }
    }

    public override void PrimaryAction(Animator animator)
    {
        animator.SetTrigger("Slash");

        // Call SwordDamage component to perform the attack
        if (swordDamage != null)
        {
            swordDamage.PerformSlashAttack();
        }
    }

    public override void SecondaryAction(Animator animator)
    {
        animator.SetTrigger("Stab");

        // Call SwordDamage component to perform the attack
        if (swordDamage != null)
        {
            swordDamage.PerformStabAttack();
        }
    }

    public override string GetIdleAnimationState()
    {
        return "CarryingSwordIdle";
    }
}
