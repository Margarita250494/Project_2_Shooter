using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Weapon[] availableWeapons; // List of all available weapons
    private Weapon currentWeaponData; // Current weapon's data
    private GameObject currentWeaponModel;
    public Transform weaponHolder;
    private Animation animator;

    private float nextFireTime = 0f;
    private bool reloading = false;
    public int bulletsLeft;

    private GameManager gameManager; // Reference to GameManager

    private void Start()
    {
        // Initialize with the first weapon (for example, Pistol)
        SetWeapon(availableWeapons[0]);
        bulletsLeft = currentWeaponData.magazineSize; // Start with full ammo

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Find GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    private void Update()
    {
        // Skip input handling if the game is paused
        if (gameManager != null && !gameManager.TimerOn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Switch weapons with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(availableWeapons[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(availableWeapons[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeapon(availableWeapons[3]);
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
        bulletsLeft = currentWeaponData.magazineSize;

        // Destroy the previous weapon model
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }

        // Instantiate the new weapon model as a child of the weapon holder or PlayerCam
        if (currentWeaponData.weaponModel != null)
        {
            currentWeaponModel = Instantiate(currentWeaponData.weaponModel, weaponHolder);
            currentWeaponModel.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("Weapon model is null!");
        }

        nextFireTime = Time.time + currentWeaponData.fireRate;
    }

    private void Shoot()
    {
        if (bulletsLeft == 1)
        {
            Debug.Log("Ammo! 1");
            Reload();
            return;
        }

        nextFireTime = Time.time + currentWeaponData.fireRate;

        for (int i = 0; i < currentWeaponData.bulletsPerShot; i++)
        {
            Vector3 shootDirection = transform.forward;
            shootDirection.x += Random.Range(-currentWeaponData.spread, currentWeaponData.spread);
            shootDirection.y += Random.Range(-currentWeaponData.spread, currentWeaponData.spread);
            shootDirection.z += Random.Range(-currentWeaponData.spread, currentWeaponData.spread);
            shootDirection.Normalize();

            Ray ray = new Ray(transform.position, shootDirection);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * currentWeaponData.range, Color.red, 1f);

            if (Physics.Raycast(ray, out hit, currentWeaponData.range))
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyScript enemyScript = hit.collider.GetComponentInParent<EnemyScript>();

                    if (enemyScript != null)
                    {
                        int damageToApply = currentWeaponData.damage;

                        if (hit.collider.name == "EnemyHead")
                        {
                            Debug.Log("Headshot detected!");
                            damageToApply *= 10;
                        }

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

        bulletsLeft -= currentWeaponData.bulletsPerShot;
        PlaySound(currentWeaponData.shootSound);
        //playerControllerUi.OnWeaponShoot(int weaponID)
        PlayMuzzleFlash();
        ApplyRecoil();
    }

    private void Reload()
    {
        if (reloading) return;
        PlaySound(currentWeaponData.reloadSound);

        reloading = true;
        Debug.Log("Reloading...");

        Invoke("ReloadFinished", currentWeaponData.reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = currentWeaponData.magazineSize;
        reloading = false;
        Debug.Log("Reload complete.");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            GetComponent<AudioSource>().PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("No audio clip assigned for this action.");
        }
    }

    private void PlayMuzzleFlash()
    {
        if (currentWeaponData.muzzleFlashPrefab != null && currentWeaponModel != null)
        {
            Transform muzzlePosition = currentWeaponModel.transform.Find("Muzzle");

            if (muzzlePosition != null)
            {
                GameObject flash = Instantiate(currentWeaponData.muzzleFlashPrefab, muzzlePosition.position, muzzlePosition.rotation);
                flash.transform.parent = muzzlePosition;
                Destroy(flash, 5f);
            }
            else
            {
                Debug.LogWarning("Muzzle position not found on the weapon model.");
            }
        }
    }

    private void ApplyRecoil()
    {
        float horizontalRecoil = Random.Range(-currentWeaponData.recoilX, currentWeaponData.recoilX);
        float verticalRecoil = Random.Range(0, currentWeaponData.recoilY);

        FindObjectOfType<PlayerCam>().ApplyRecoil(verticalRecoil, horizontalRecoil);
    }
}
