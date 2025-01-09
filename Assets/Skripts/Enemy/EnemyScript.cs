using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 20; // Enemy's starting health

    // This function is called when the enemy is shot
    public void TakeDamage(int damage)
    {
        health -= damage; // Decrease health by the damage received
        Debug.Log("Enemy hit! Remaining Health: " + health);

        if (health <= 0)
        {
            Die(); // If health is 0 or less, the enemy dies
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        // Add your death logic here, like playing animations or destroying the object
        Destroy(gameObject);
    }
}
