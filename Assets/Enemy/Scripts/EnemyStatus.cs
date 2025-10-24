using UnityEngine;
[DefaultExecutionOrder(-10000)] 
public class EnemyStatus: MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (anim != null)
            {
                anim.SetTrigger("hit"); 
            }
        }
    }

    void Die()
    {
        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("death");
        }
    }
    public bool IsDead()
    {
        return isDead;
    }
}