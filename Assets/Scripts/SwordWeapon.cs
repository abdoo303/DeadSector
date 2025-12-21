using UnityEngine;

public class SwordWeapon : Weapon
{
    private SwordDamage swordDamage;

    void Start()
    {
        weaponName = "Sword";
        swordDamage = GetComponentInParent<SwordDamage>();
        if (swordDamage == null)
        {
            Debug.LogError("SwordDamage component not found! Make sure it's on the player.");
        }
    }

    public override void PrimaryAction(Animator animator)
    {
        animator.SetTrigger("Slash");
        // Call damage immediately
        if (swordDamage != null)
        {
            swordDamage.PerformSlashAttack();
        }
    }

    public override void SecondaryAction(Animator animator)
    {
        animator.SetTrigger("Stab");
        // Call damage immediately
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
