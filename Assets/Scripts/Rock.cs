using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, IDestroyable
{
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;
    bool destroyed = false;
    

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (currentHealth <= 0 && !destroyed)
        {
            Destroy();
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void Destroy() 
    {
        Debug.Log("Rock has been destroyed");
        Destroy(gameObject);
        destroyed = true;
    }
}
