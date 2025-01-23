using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 20; // Enemy's starting health
    private float selfDestructTimer = 25f; // Time until the enemy destroys itself
    private Animator enemy;
    private AudioSource audioSource; // AudioSource component reference
   
    private bool isDead = false;

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
}
