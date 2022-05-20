using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDestroyable
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

    // Update is called once per frame
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
        Debug.Log("You died!");
        Destroy(gameObject);
        destroyed = true;
    }
}
