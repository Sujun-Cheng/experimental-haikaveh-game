using System.Collections;
using UnityEngine;

public class NPCObjectiveTracker : MonoBehaviour
{
    [Header("Objective Tracking")]
    [Tooltip("Enable this to count this NPC for objectives")]
    public bool trackAsObjective = true;

    [Header("Enemy Version Reference")]
    [Tooltip("The enemy GameObject that gets activated on wrong choice")]
    public GameObject enemyVersion;

    private bool hasConversationStarted = false;

  
    public void OnConversationStarted()
    {
        if (!hasConversationStarted && trackAsObjective)
        {
            hasConversationStarted = true;
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.CompleteNPCConversation();
                Debug.Log($"[Objective] Conversation started with {gameObject.name}");
            }
        }
    }

    // friendly choice
    public void OnRightChoice()
    {
        Debug.Log($"[Objective] {gameObject.name} - Right choice made, disappearing peacefully");

        // Notify ObjectiveManager
        if (trackAsObjective && ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.NPCDisappeared();
        }

        StartCoroutine(FadeOutAndDestroy());
    }

    // combat choice
    public void OnWrongChoice()
    {
        Debug.Log($"[Objective] {gameObject.name} - Wrong choice made, activating enemy version");

        if (enemyVersion != null)
        {
            // Make sure enemy version is active (in case it's not already)
            enemyVersion.SetActive(true);

            // Mark the enemy to track for objectives
            EnemyStatus enemyStatus = enemyVersion.GetComponent<EnemyStatus>();
            if (enemyStatus != null && trackAsObjective)
            {
                enemyStatus.isNPCEnemy = true;
                Debug.Log($"[Objective] Enemy {enemyVersion.name} marked for objective tracking");
            }
            else if (enemyStatus == null)
            {
                Debug.LogWarning($"[Objective] Enemy {enemyVersion.name} has no EnemyStatus component!");
            }
        }
        else
        {
            Debug.LogWarning($"[Objective] {gameObject.name} has no enemy version assigned!");
        }

        // Deactivate this NPC GameObject
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // Disable interaction components
        NPCInteraction npcInteraction = GetComponent<NPCInteraction>();
        if (npcInteraction != null)
        {
            npcInteraction.enabled = false;
        }

        if (npcInteraction != null && npcInteraction.interactionPrompt != null)
        {
            npcInteraction.interactionPrompt.SetActive(false);
        }

        // Fade out
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float fadeTime = 1f;
        float elapsed = 0f;

        // Create material instances to avoid modifying shared materials
        Material[][] originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            Material[] newMats = new Material[mats.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                newMats[j] = new Material(mats[j]);
            }
            renderers[i].materials = newMats;
            originalMaterials[i] = newMats;
        }

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

                    // For Standard Shader transparency
                    if (mat.HasProperty("_Mode"))
                    {
                        mat.SetFloat("_Mode", 3);
                        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        mat.SetInt("_ZWrite", 0);
                        mat.DisableKeyword("_ALPHATEST_ON");
                        mat.EnableKeyword("_ALPHABLEND_ON");
                        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        mat.renderQueue = 3000;
                    }
                }
            }

            yield return null;
        }

        // Deactivate
        gameObject.SetActive(false);
    }
}