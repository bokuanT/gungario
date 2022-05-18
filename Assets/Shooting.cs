using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Transform playerPoint;
    public GameObject player; 

    public float bulletForce = 20f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // logic to displace bullet spawn point and rotation
        // Ryan's note: Temporary fix. Without this it collides with the player/gun model
        Vector3 displace = Vector3.right;


        // logic for direction based off 
        GameObject bullet = Instantiate(bulletPrefab, playerPoint.position + displace, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
    }
}
