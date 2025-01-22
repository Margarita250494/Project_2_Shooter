using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 20; // Enemy's starting health
    private float selfDestructTimer = 10f; // Time until the enemy destroys itself
    private Animator enemy;
    private AudioSource audioSource; // AudioSource component reference
    public AudioClip dieSound; // Audio clip for the death sound

    private void Start()
    {
        // Schedule self-destruction after the timer expires
        Invoke(nameof(SelfDestruct), selfDestructTimer);
        enemy = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();


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
        Debug.Log("Enemy died!");
        // Play the death animation
       /* if (enemy != null)
        {
           enemy.SetTrigger("Die"); // Trigger the "Die" animation
        }*/
        // Play the death sound
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound); // Play the death sound
        }
        Destroy(gameObject); // Destroy the enemy immediately upon death

    }

    private void SelfDestruct()
    {
        Debug.Log("Enemy self-destructed due to timeout.");
        Destroy(gameObject); // Destroy the enemy after the timer expires
    }
}
