using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-10000)]
public class EnemyStatus : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator anim;
    private bool isDead = false;

    [Header("Disappear Settings")]
    public float disappearDelay = 2f; // Time before disappearing after death
    public bool isNPCEnemy = false; // Set true if this enemy came from NPC dialogue
    public int npcCountValue = 2; // How many NPCs this counts as when killed (default: 2 for paired NPCs)

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

        // Notify ObjectiveManager that NPC disappeared (killed)
        if (isNPCEnemy && ObjectiveManager.Instance != null)
        {
            // Add the specified count (default 2 for paired NPCs)
            for (int i = 0; i < npcCountValue; i++)
            {
                ObjectiveManager.Instance.NPCDisappeared();
            }
            Debug.Log($"[EnemyStatus] {gameObject.name} killed - counted as {npcCountValue} NPCs disappeared");
        }

        // Start disappear coroutine
        StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        // Wait for death animation to play
        yield return new WaitForSeconds(disappearDelay);

        // Optional: Fade out effect
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float fadeTime = 0.5f;
        float elapsed = 0f;

        // Store original colors
        Material[][] originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }

        // Fade out
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeTime);

            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color color = mat.color;
                        color.a = alpha;
                        mat.color = color;
                    }
                }
            }

            yield return null;
        }

        // Destroy the game object
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
}