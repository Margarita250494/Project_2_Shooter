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
    public GameObject bulletHolePrefab;

    // New property to choose if the weapon should fire with the button held down
    public bool allowHoldFire = false; // Default to false (click to fire)

    // Recoil properties
    public float recoilX; // Horizontal recoil (left/right)
    public float recoilY; // Vertical recoil (up/down)
    public float recoilZ; // Forward/backward recoil (kickback)
}
