using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab of the enemy to spawn
    public int maxEnemy = 6; // Maximum number of enemies at a time
    public float spawnRange = 20f; // Range for x and z spawning
    public float spawnInterval = 1f; // Time between spawn checks
    public string enemyLayerName = "Enemy"; // Name of the enemy layer

    private int numOfEnemy = 0;
    private List<GameObject> enemyList = new List<GameObject>();

    private void Start()
    {
        // Start the spawn coroutine
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (numOfEnemy < maxEnemy)
            {
                SpawnEnemy();
            }

            // Check for destroyed enemies and clean up the list
            CheckEnemies();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        float x, z;

        // Generate spawn positions relative to the spawner's position
        do
        {
            x = Random.Range(-spawnRange, spawnRange);
        } while (x > -2f && x < 2f);

        do
        {
            z = Random.Range(-spawnRange, spawnRange);
        } while (z > -2f && z < 2f);

        Vector3 spawnPosition = new Vector3(
            transform.position.x + x,
            transform.position.y, // Use the spawner's y-coordinate
            transform.position.z + z
        );

        // Spawn the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Set the enemy's layer
        enemy.layer = LayerMask.NameToLayer(enemyLayerName);

        enemyList.Add(enemy);
        numOfEnemy++;
    }



    private void CheckEnemies()
    {
        for (int i = enemyList.Count - 1; i >= 0; i--)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
                numOfEnemy--;
            }
        }
    }
}
