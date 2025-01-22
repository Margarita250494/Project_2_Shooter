using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public GameObject weaponModel; // Prefab for the weapon model
    public float range;
    public int damage;
    public float fireRate;
    public int magazineSize;
    public float reloadTime;
    public float spread;
    public int bulletsPerShot;
    public AudioClip shootSound; 
    public AudioClip reloadSound;
    public GameObject muzzleFlashPrefab;

    // New property to choose if the weapon should fire with the button held down
    public bool allowHoldFire = false; // Default to false (click to fire)

    // Recoil properties
    public float recoilX; // Horizontal recoil (left/right)
    public float recoilY; // Vertical recoil (up/down)
    public float recoilZ; // Forward/backward recoil (kickback)

    private int bulletLeftForUI;
    private int AmmoCapacityForUI;

    public void EnableWeapon()
    {
        if(bulletsPerShot == 0) return;

        AmmoCapacityForUI = magazineSize / bulletsPerShot;
        bulletLeftForUI = AmmoCapacityForUI;
        
    }

    public void Fire()
    {
        bulletLeftForUI--;
    }

    public void Reload()
    {
        bulletLeftForUI = magazineSize / bulletsPerShot;
    }

    public int GetBulletLeftForUI()
    {
        return bulletLeftForUI;
    }

    public float GetReloadTime()
    {
        if (bulletLeftForUI == 0) return reloadTime;
        else return (int)(((float)(magazineSize / bulletsPerShot) - bulletLeftForUI) / magazineSize * reloadTime);
    }

    public bool IsAmmoFull()
    {
        return bulletLeftForUI == magazineSize / bulletsPerShot;
    }

    public int GetAmmoCapacityForUI()
    {
        return AmmoCapacityForUI;
    }
}
