using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public GameObject hitEffect;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit!");
        
        // reduces hp of target collided with
        Debug.Log("Collided with " + collision.gameObject);
        collision.gameObject.SendMessage("TakeDamage", 20);

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(effect, 1f);
    }

}
