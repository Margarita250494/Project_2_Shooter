using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyScript : MonoBehaviour
{
    public EnemyScript enemyScript;
    private int damage;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit Body");
        enemyScript.TakeDamage(damage);
        Destroy(collision.gameObject);
    }
}
