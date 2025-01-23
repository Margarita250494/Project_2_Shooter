using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 20; // Enemy's starting health
    private float selfDestructTimer = 25f; // Time until the enemy destroys itself
    private Animator enemy;
    private AudioSource audioSource; // AudioSource component reference
    public AudioClip dieSound; // Audio clip for the death sound

    private float speed;
    private bool isDirectionXForHardOrExtremeMode; // 0 for X and 1 for Z

    private float[] RangeX;
    private float[] RangeZ;

    private bool isDead = false;

    private void Start()
    {
        // Schedule self-destruction after the timer expires
        Invoke(nameof(SelfDestruct), selfDestructTimer);
        enemy = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        RangeX = EnemySpawner.Instance.RangeX;
        RangeZ = EnemySpawner.Instance.RangeZ;

        switch (ModeManager.Instance.ModeNr)
        {
            case 1:
                speed = Random.Range(-0.1f, 0.1f);
                break;
            case 2:
                do
                {
                    speed = Random.Range(-0.2f, 0.2f);
                } while (speed < 0.1f && speed > -0.1f);

                isDirectionXForHardOrExtremeMode = Random.Range(0, 2) == 0;

                break;
            case 3:
                do
                {
                    speed = Random.Range(-0.3f, 0.3f);
                } while (speed < 0.2f && speed > -0.2f);

                isDirectionXForHardOrExtremeMode = Random.Range(0, 2) == 0;

                break;
        }
    }

    private void Update()
    {
        switch (ModeManager.Instance.ModeNr)
        {
            case 1:
                MovementForMediumMode();
                break;
            case 2:
                MovementForHardMode();
                break;
            case 3:
                MovementForExtremeMode();
                break;
        }
    }


    // This function is called when the enemy is shot
    public void TakeDamage(int damage)
    {
        health -= damage; // Decrease health by the damage received
        Debug.Log("Enemy hit! Remaining Health: " + health);

        if (health <= 0)
        {
            CancelInvoke(nameof(SelfDestruct)); // Cancel self-destruction if the enemy is killed
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple calls to Die
        isDead = true;

        Debug.Log("Enemy died!");

        // Play the death animation
        if (enemy != null)
        {
            enemy.SetTrigger("Die");
        }

        // Play the death sound
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Destroy(gameObject, audioSource.clip.length); // Delay destruction until sound finishes
        }
        else
        {
            Debug.LogWarning("AudioSource or clip is missing. Destroying immediately.");
            Destroy(gameObject); // Fallback if no audio source or clip is set
        }

        GameUIManager.Instance.GainScore(1);
    }


    private void SelfDestruct()
    {
        Debug.Log("Enemy self-destructed due to timeout.");
        Destroy(gameObject); // Destroy the enemy after the timer expires
    }

    private void MovementForMediumMode()
    {
        float z = transform.position.z + speed;
        if (z > RangeZ[1] || z < RangeZ[0])
        {
            speed = -speed;
            z = transform.position.z + speed;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    private void MovementForHardMode()
    {
        if (isDirectionXForHardOrExtremeMode)
        {
            float x = transform.position.x + speed;
            if (x > RangeX[1] || x < RangeX[0])
            {
                speed = -speed;
                x = transform.position.x + speed;
            }

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        else MovementForMediumMode();
    }

    private void MovementForExtremeMode()
    {
        if (isDirectionXForHardOrExtremeMode)
        {
            float x = transform.position.x + speed;
            if (x > RangeX[1] || x < RangeX[0])
            {
                speed = -speed;
                x = transform.position.x + speed;
            }

            float deltaZ = (Random.Range(0, 3) - 1) * speed;

            float z = transform.position.z + deltaZ;
            if (z > RangeZ[1] || z < RangeZ[0])
            {
                z = transform.position.z;
            }

            transform.position = new Vector3(x, transform.position.y, z);
        }
        else {
            float z = transform.position.z + speed;
            if (z > RangeZ[1] || z < RangeZ[0])
            {
                speed = -speed;
                z = transform.position.z + speed;
            }

            float deltaX = (Random.Range(0, 3) - 1) * speed;

            float x = transform.position.x + deltaX;
            if (x > RangeX[1] || x < RangeX[0])
            {
                x = transform.position.x;
            }

            transform.position = new Vector3(x, transform.position.y, z);
        }
    }
}
