using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    [Header("Damage Response")]
    public bool canTakeDamage = true;
    public float invincibilityDuration = 0.5f;
    public bool showDamageDebug = true;
    [Header("Death Settings")]
    public bool destroyOnDeath = false;
    public float destroyDelay = 3f;
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnDeath;
    private Animator anim;
    private bool isDead = false;
    private bool isInvincible = false;
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }
    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        if (!canTakeDamage || isInvincible || isDead) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        if (showDamageDebug)
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        // Trigger events
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        // Play hit animation
        if (anim != null)
        {
            anim.SetTrigger("hit");
        }
        // Start invincibility frames
        StartCoroutine(InvincibilityRoutine());
        // Check for death
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
    void Die()
    {
        isDead = true;
        if (showDamageDebug)
            Debug.Log($"{gameObject.name} has died!");
        // Trigger death event
        OnDeath?.Invoke();
        EventManager.TriggerEvent<DeathEvent, GameObject>(gameObject);
        // Play death animation
        if (anim != null)
        {
            anim.SetTrigger("death");
            anim.SetBool("isDead", true);
        }
        // Disable components
        var combatController = GetComponent<CombatController>();
        if (combatController != null)
            combatController.enabled = false;
        var inputController = GetComponent<CharacterInputController>();
        if (inputController != null)
            inputController.enabled = false;

        // Destroy after delay if set
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        if (showDamageDebug)
            Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}/{maxHealth}");
    }
    public bool IsDead()
    {
        return isDead;
    }

    public void Revive()
    {
        isDead = false;
        var combatController = GetComponent<CombatController>();
        if (combatController != null)
            combatController.enabled = true;
        var inputController = GetComponent<CharacterInputController>();
        if (inputController != null)
            inputController.enabled = true;
    }
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}