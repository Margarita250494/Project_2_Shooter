using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Weapon[] availableWeapons; // List of all available weapons
    private Weapon currentWeaponData; // Current weapon's data

    private float nextFireTime = 0f;
    private bool reloading = false;
    private int bulletsLeft;

    private void Start()
    {
        // Initialize with the first weapon (for example, Pistol)
        SetWeapon(availableWeapons[0]);
        bulletsLeft = currentWeaponData.magazineSize; // Start with full ammo
    }

    private void Update()
    {
        // Switch weapons with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(availableWeapons[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(availableWeapons[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeapon(availableWeapons[2]);
        }

        // Reload when R is pressed
        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < currentWeaponData.magazineSize)
        {
            Reload();
        }

        // Handle shooting
        if ((currentWeaponData.allowHoldFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) && Time.time >= nextFireTime && !reloading)
        {
            Shoot();
        }
    }

    private void SetWeapon(Weapon weaponData)
    {
        currentWeaponData = weaponData;
        bulletsLeft = currentWeaponData.magazineSize; // Reset ammo when changing weapons

        // Set the weapon model in the player's hands (or instantiate it, depending on how it's set up)
        // Assuming you have a method to handle the model swapping

        nextFireTime = Time.time + currentWeaponData.fireRate;
    }

    private void Shoot()
    {
        if (bulletsLeft <= 0)
        {
            Debug.Log("Out of Ammo!");
            return; // Prevent shooting if no ammo
        }

        nextFireTime = Time.time + currentWeaponData.fireRate;

        // Loop through the number of bullets per shot (shotgun spread)
        for (int i = 0; i < currentWeaponData.bulletsPerShot; i++)
        {
            // Randomize the shoot direction for spread effect
            Vector3 shootDirection = transform.forward;
            shootDirection.x += Random.Range(-currentWeaponData.spread, currentWeaponData.spread); // Add horizontal spread
            shootDirection.y += Random.Range(-currentWeaponData.spread, currentWeaponData.spread); // Add vertical spread
            shootDirection.z += Random.Range(-currentWeaponData.spread, currentWeaponData.spread); // Add depth spread
            shootDirection.Normalize();

            Ray ray = new Ray(transform.position, shootDirection);
            RaycastHit hit;

            // Perform the raycast and check if it hits something
            Debug.DrawRay(ray.origin, ray.direction * currentWeaponData.range, Color.red, 1f); // Draw red ray for debugging

            if (Physics.Raycast(ray, out hit, currentWeaponData.range))
            {
                Debug.Log("Hit: " + hit.collider.name); // Check what it hits

                // If the ray hits an enemy (layer "Enemy"), apply damage
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyScript enemyScript = hit.collider.GetComponentInParent<EnemyScript>();

                    if (enemyScript != null)
                    {
                        int damageToApply = currentWeaponData.damage;

                        // Check if it's a headshot (adjust damage for headshots)
                        if (hit.collider.name == "EnemyHead")
                        {
                            Debug.Log("Headshot detected!");
                            damageToApply *= 10; // Apply 10x damage for headshots
                        }

                        // Apply the calculated damage
                        enemyScript.TakeDamage(damageToApply);
                        Debug.Log("Damage Applied: " + damageToApply);
                    }
                }
                else
                {
                    Debug.Log("Hit something that is not an enemy");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything");
            }
        }

        bulletsLeft -= currentWeaponData.bulletsPerShot; // Subtract bullets after shooting
    }

    private void Reload()
    {
        if (reloading) return; // Prevent starting reload if already reloading

        reloading = true;
        Debug.Log("Reloading...");

        // Simulate reload time
        Invoke("ReloadFinished", currentWeaponData.reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = currentWeaponData.magazineSize; // Reset to full ammo
        reloading = false;
        Debug.Log("Reload complete.");
    }
}
