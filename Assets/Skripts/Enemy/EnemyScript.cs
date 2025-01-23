using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 20; // Enemy's starting health
    private float selfDestructTimer = 25f; // Time until the enemy destroys itself
    private Animator enemy;
    public AudioClip dieSound; // Audio clip for the death sound

    private float speed;
    private bool isDirectionXForHardMode;
    private Vector3 directionForExtremeMode;
    private float timeToChangeDirectionInExtremeMode = 0;

    private float[] RangeX;
    private float[] RangeZ;

    private bool isDead = false;

    private void Start()
    {
        // Schedule self-destruction after the timer expires
        Invoke(nameof(SelfDestruct), selfDestructTimer);
        enemy = GetComponent<Animator>();

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

                isDirectionXForHardMode = Random.Range(0, 2) == 0;

                break;
            case 3:
                do
                {
                    speed = Random.Range(-0.3f, 0.3f);
                } while (speed < 0.2f && speed > -0.2f);

                isDirectionXForHardMode = Random.Range(0, 2) == 0;

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

        Destroy(gameObject);

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
        if (isDirectionXForHardMode)
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
        if(timeToChangeDirectionInExtremeMode <= 0)
        {
            timeToChangeDirectionInExtremeMode = Random.Range(0f, 2f);

            float x = Random.Range(-1f, 1f);
            float z = (Random.Range(0, 2) == 0 ? -1 : 1) * Mathf.Sqrt(1 - x * x);

            directionForExtremeMode = new Vector3(x, 0, z);
        } else
        {
            timeToChangeDirectionInExtremeMode -= Time.deltaTime;

            Vector3 newPos = transform.position + directionForExtremeMode * speed;

            if (newPos.x < RangeX[0] || newPos.x > RangeX[1])
            {
                directionForExtremeMode.x *= -1;
            }
            else if (newPos.z < RangeZ[0] || newPos.z > RangeZ[1])
            {
                directionForExtremeMode.z *= -1;
            }
            else transform.position = newPos;
        }
    }
}
