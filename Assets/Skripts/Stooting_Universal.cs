using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    // Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;

    // Bools
    private bool shooting, readyToShoot, reloading;

    // References
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask whatIsEnemy;

    // Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    // Internal variables
    private int lastBulletCount;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        lastBulletCount = bulletsLeft;
    }

    private void Update()
    {
        HandleInput();

        // Update bullet count UI only if bulletsLeft changes
        if (text != null && bulletsLeft != lastBulletCount)
        {
            text.SetText(bulletsLeft + " / " + magazineSize);
            lastBulletCount = bulletsLeft;
        }
    }

    private void HandleInput()
    {
        // Shooting input
        shooting = allowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        // Reload input
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // Shooting logic
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = Mathf.Min(bulletsPerTap, bulletsLeft);
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate direction with spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        // Raycast
        if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            // Damage enemy
            if (rayHit.collider.CompareTag("Enemy"))
            {
                var enemy = rayHit.collider.GetComponent<ShootingAi>();
                if (enemy != null) enemy.TakeDamage(damage);
            }

            // Bullet hole graphic
            if (bulletHoleGraphic != null)
            {
                Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
        }

        // Camera shake
        if (camShake != null)
        {
            camShake.Shake(camShakeDuration, camShakeMagnitude);
        }

        // Muzzle flash
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot--;

        // Reset shot after delay
        Invoke(nameof(ResetShot), timeBetweenShooting);

        // Continue shooting if more bullets need to be shot
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;

        // Simulate reload time
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
