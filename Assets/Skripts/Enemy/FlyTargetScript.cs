using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyTargetScript : MonoBehaviour
{
    private float life;
    private float bounceHeight = 5f; 
    private float bounceSpeed = 2f;  

    private float initialY;         

    void Start()
    {
        initialY = transform.position.y;
        life = Random.Range(1, 2);
    }

    void Update()
    {
        float newY = initialY + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        life -= 1;
        Destroy(collision.gameObject);
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
