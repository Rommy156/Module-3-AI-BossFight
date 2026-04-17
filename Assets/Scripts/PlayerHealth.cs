using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public float maxHealth = 100f;
    public float currentHealth;

    public Scrollbar healthBar;


    void Awake()
    {
        
            if (Instance == null)
                Instance = this;

            currentHealth = maxHealth;
        
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.size = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("");
        }
    }
}