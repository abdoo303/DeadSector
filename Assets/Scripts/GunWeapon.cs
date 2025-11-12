using UnityEngine;

public class GunWeapon : Weapon
{
    void Start()
    {
        weaponName = "Gun";
    }

    public override void PrimaryAction(Animator animator)
    {
        // This will trigger the "Fire" animation
        animator.SetTrigger("Fire");
        
        // Add your gun shooting logic here:
        // - Raycast for hit detection
        // - Spawn bullet projectile
        // - Play shooting sound/VFX
        // - Reduce ammo, etc.
    }

    public override string GetIdleAnimationState()
    {
        return "CarryingGunIdle"; // Your gun idle animation state name
    }

    // Optional: Add gun-specific methods
    public void Shoot()
    {
        Debug.Log("Gun fired!");
        // Add shooting logic here
    }
}