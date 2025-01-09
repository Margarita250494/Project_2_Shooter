using System.Collections;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public float range = 100f;
    public int damage = 10;
    public LayerMask hitLayers;

    public Camera fpsCam;

    public int magazineSize = 30;
    private int bulletsLeft;

    public bool allowHoldFire = false;
    private bool readyToShoot = true;
    private bool reloading = false;

    public float timeBetweenShots = 0.1f;
    public int bulletsPerShot = 1;
    public float spread = 0.05f;

    public float reloadTime = 2f;

    // Muzzle Flash Variables
    public ParticleSystem muzzleFlashParticle; // Reference to the Particle System for muzzle flash

    // Bullet Hole Variables
    public GameObject bulletHolePrefab;
    public LayerMask mapLayer;

    private void Awake()
    {
        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < magazineSize)
        {
            Reload();
        }

        if ((allowHoldFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) && readyToShoot && bulletsLeft > 0 && !reloading)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shootDirection = fpsCam.transform.forward;
            shootDirection.x += Random.Range(-spread, spread);
            shootDirection.y += Random.Range(-spread, spread);
            shootDirection.z += Random.Range(-spread, spread);
            shootDirection.Normalize();

            Ray ray = new Ray(fpsCam.transform.position, shootDirection);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

            if (Physics.Raycast(ray, out hit, range, hitLayers))
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyScript enemyScript = hit.collider.GetComponentInParent<EnemyScript>();

                    if (enemyScript != null)
                    {
                        int finalDamage = damage;

                        if (hit.collider.name == "EnemyHead")
                        {
                            Debug.Log("Headshot detected!");
                            finalDamage *= 10;
                        }
                        else if (hit.collider.name == "EnemyBody")
                        {
                            Debug.Log("Body shot detected.");
                        }

                        enemyScript.TakeDamage(finalDamage);
                    }
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Map"))
                {
                    Debug.Log("Hit map surface!");
                    SpawnBulletHole(hit.point, hit.normal);
                }
            }
        }

        bulletsLeft -= bulletsPerShot;

        PlayMuzzleFlash();

        Invoke("ResetShoot", timeBetweenShots);

        if (bulletsLeft <= 0)
        {
            Debug.Log("Out of Ammo!");
        }
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlashParticle != null)
        {
            muzzleFlashParticle.Stop(); // Ensure the particle system is stopped before restarting
            muzzleFlashParticle.Play(); // Play the particle system
        }
    }

    private void SpawnBulletHole(Vector3 position, Vector3 normal)
    {
        if (bulletHolePrefab != null)
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, position, Quaternion.LookRotation(normal));
            Destroy(bulletHole, 10f);
        }
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Debug.Log("Reloading...");
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        Debug.Log("Reload complete.");
    }
}
