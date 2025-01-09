using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyTargetSpawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;

    private float spawnTimer = 0f;
    private float spawnRate;
    private GameObject createdFlyTarget = null;

    // Add a new LayerMask for the flying enemies to spawn

    void Start()
    {
        spawnRate = Random.Range(5f, 15f);
    }

    void Update()
    {
        if (createdFlyTarget == null) spawnTimer += Time.deltaTime;
        else CheckEnemy();

        if (createdFlyTarget == null && spawnTimer >= spawnRate)
        {
            float x;
            do
            {
                x = Random.Range(-20f, 20f);
            } while (x > -5f && x < 5f);

            float z;
            do
            {
                z = Random.Range(-20f, 20f);
            } while (z > -5f && z < 5f);

            createdFlyTarget = Instantiate(enemyPrefab, new Vector3(x, 10f, z), new Quaternion());
            spawnTimer = 0f;
            spawnRate = Random.Range(5f, 15f);
        }
    }

    private void CheckEnemy()
    {
        if (createdFlyTarget == null)
        {
            return;
        }

        // Optionally, you could check the flying enemy's health or state here
        if (createdFlyTarget.IsDestroyed())
        {
            createdFlyTarget = null;
        }
    }
}
