using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public GameObject enemyPrefab; // Prefab of the enemy to spawn
    public GameObject enemyFloor; 
    public int maxEnemy = 6; // Maximum number of enemies at a time
    public float spawnInterval = 1f; // Time between spawn checks
    public string enemyLayerName = "Enemy"; // Name of the enemy layer

    private int numOfEnemy = 0;
    private List<GameObject> enemyList = new List<GameObject>();

    public float[] RangeX = new float[2];
    public float[] RangeZ = new float[2];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Start the spawn coroutine
        StartCoroutine(SpawnEnemies());

        MeshRenderer meshRenderer = enemyFloor.GetComponent<MeshRenderer>();
        Vector3 floorSize = meshRenderer.bounds.size;

        RangeX[0] = transform.position.x - (floorSize.x - 6) / 2;
        RangeX[1] = transform.position.x + (floorSize.x - 6) / 2;
        RangeZ[0] = transform.position.z - (floorSize.z - 6) / 2;
        RangeZ[1] = transform.position.z + (floorSize.z - 6) / 2;
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

        /*
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
        */

        x = Random.Range(RangeX[0], RangeX[1]);
        z = Random.Range(RangeZ[0], RangeZ[1]);

        Vector3 spawnPosition = new Vector3(
            x,
            transform.position.y, // Use the spawner's y-coordinate
            z
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
