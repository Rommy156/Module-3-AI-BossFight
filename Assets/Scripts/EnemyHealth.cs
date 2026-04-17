using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;

    public Scrollbar healthBar;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(gameObject.name + " took damage: " + amount);
        if (healthBar != null)
            healthBar.size = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");

        // Option 1: destroy enemy
        Destroy(gameObject);

        // Option 2 (later): play animation, drop loot, etc.
    }
}