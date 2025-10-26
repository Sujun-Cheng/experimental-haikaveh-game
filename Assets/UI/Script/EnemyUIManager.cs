using TMPro;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    public EnemyStatus enemy;
    public HealthBar healthBar;
    public TextMeshProUGUI nameText;
    private float lastHealth;

    void Start()
    {
        if (enemy == null)
        {
            Debug.LogError("No Enemy component found.");
            enabled = false;
            return;
        }

        if (healthBar == null)
        {
            Debug.LogError("No HealthBar component found.");
            enabled = false;
            return;
        }
        nameText.text = enemy.name;
        healthBar.SetMaxHealth(enemy.maxHealth);
        healthBar.SetHealth(enemy.maxHealth);
        lastHealth = enemy.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float currentHealth = enemy.currentHealth;
        if (Mathf.Abs(currentHealth - lastHealth) > Mathf.Epsilon)
        {
            healthBar.SetHealth(currentHealth);
            lastHealth = currentHealth;
        }

        if (currentHealth <= 0 && healthBar.gameObject.activeSelf)
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}

