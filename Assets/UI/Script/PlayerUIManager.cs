using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public HealthBar healthBar;
    private HealthSystem currentHealthSystem;

    public void SetCharacter(GameObject character)
    {
        if (character == null) return;
        nameText.text = character.name;
        if (currentHealthSystem != null)
        {
            currentHealthSystem.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
        currentHealthSystem = character.GetComponent<HealthSystem>();
        if (currentHealthSystem != null)
        {
            healthBar.SetMaxHealth(currentHealthSystem.maxHealth);
            healthBar.SetHealth(currentHealthSystem.currentHealth);
            currentHealthSystem.OnHealthChanged.AddListener(UpdateHealthBar);
        }
    }

    private void UpdateHealthBar(float newHealth)
    {
            healthBar.SetHealth(newHealth);
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentHealthSystem.TakeDamage(20, Vector3.zero);
        }
    }

    private void OnDisable()
    {
        // Prevent event leaks when this UI gets disabled
        if (currentHealthSystem != null)
        {
            currentHealthSystem.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}
