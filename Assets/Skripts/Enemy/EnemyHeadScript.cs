using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadScript : MonoBehaviour
{
    public EnemyScript enemyScript;
    private int damage;

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit Head");
        enemyScript.TakeDamage(damage*10);
        Destroy(collision.gameObject);
    }
    */
}
