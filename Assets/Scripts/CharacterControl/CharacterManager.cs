using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages multiple characters in the scene
/// Character 1 is always player-controlled
/// Character 2+ are AI companions
/// </summary>
public class CharacterManager : MonoBehaviour
{
    [Header("Character References")]
    [Tooltip("The main player character (always player-controlled)")]
    public GameObject playerCharacter;

    [Tooltip("AI companion characters that follow the player")]
    public List<GameObject> aiCompanions = new List<GameObject>();

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;

    private Transform currentCameraTarget;

    void Start()
    {
        // Validate setup
        if (playerCharacter == null)
        {
            Debug.LogError("❌ CharacterManager: Player Character not assigned!");
            return;
        }

        // Set camera to follow player
        currentCameraTarget = playerCharacter.transform;
        Debug.Log($"✅ Camera following: {playerCharacter.name}");

        // Setup AI companions to follow player
        SetupAICompanions();
    }

    void SetupAICompanions()
    {
        if (playerCharacter == null) return;

        foreach (GameObject companion in aiCompanions)
        {
            if (companion == null) continue;

            MainCharacterController mainChar = companion.GetComponent<MainCharacterController>();
            if (mainChar != null)
            {
                // Set AI to follow the player
                mainChar.SetPlayerTarget(playerCharacter.transform);
                mainChar.switchToAIControlledFollowing(playerCharacter.transform);
                Debug.Log($"✅ AI Companion {companion.name} set to follow player");
            }
            else
            {
                Debug.LogWarning($"⚠️ {companion.name} has no MainCharacterController - cannot set as AI companion");
            }
        }
    }

    void LateUpdate()
    {
        // Update camera position to follow current target
        if (currentCameraTarget != null && mainCamera != null)
        {
            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        Vector3 targetPosition = currentCameraTarget.position;
        Vector3 desiredPosition = targetPosition - currentCameraTarget.forward * cameraDistance + Vector3.up * cameraHeight;

        // Smooth camera movement
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            desiredPosition,
            Time.deltaTime * 5f
        );

        mainCamera.transform.LookAt(targetPosition + Vector3.up * 1.5f);
    }

    // Public methods to manage companions

    /// <summary>
    /// Add a new AI companion at runtime
    /// </summary>
    public void AddCompanion(GameObject companion)
    {
        if (companion == null) return;

        if (!aiCompanions.Contains(companion))
        {
            aiCompanions.Add(companion);

            MainCharacterController mainChar = companion.GetComponent<MainCharacterController>();
            if (mainChar != null && playerCharacter != null)
            {
                mainChar.SetPlayerTarget(playerCharacter.transform);
                mainChar.switchToAIControlledFollowing(playerCharacter.transform);
                Debug.Log($"✅ Added companion: {companion.name}");
            }
        }
    }

    /// <summary>
    /// Remove a companion from management
    /// </summary>
    public void RemoveCompanion(GameObject companion)
    {
        if (aiCompanions.Contains(companion))
        {
            aiCompanions.Remove(companion);
            Debug.Log($"Removed companion: {companion.name}");
        }
    }

    /// <summary>
    /// Set all AI companions to idle state
    /// </summary>
    public void SetAllCompanionsIdle()
    {
        foreach (GameObject companion in aiCompanions)
        {
            if (companion == null) continue;

            MainCharacterController mainChar = companion.GetComponent<MainCharacterController>();
            if (mainChar != null)
            {
                mainChar.switchToAIControlledIdle();
            }
        }
        Debug.Log("All companions set to idle");
    }

    /// <summary>
    /// Set all AI companions to follow player
    /// </summary>
    public void SetAllCompanionsFollowing()
    {
        if (playerCharacter == null) return;

        foreach (GameObject companion in aiCompanions)
        {
            if (companion == null) continue;

            MainCharacterController mainChar = companion.GetComponent<MainCharacterController>();
            if (mainChar != null)
            {
                mainChar.switchToAIControlledFollowing(playerCharacter.transform);
            }
        }
        Debug.Log("All companions following player");
    }

    /// <summary>
    /// Command all companions to attack a specific target
    /// </summary>
    public void CommandCompanionsAttack(Transform target)
    {
        if (target == null) return;

        foreach (GameObject companion in aiCompanions)
        {
            if (companion == null) continue;

            MainCharacterController mainChar = companion.GetComponent<MainCharacterController>();
            if (mainChar != null)
            {
                mainChar.switchToAIControlledFighting(target);
            }
        }
        Debug.Log($"All companions attacking: {target.name}");
    }

    // Getters
    public GameObject GetPlayerCharacter()
    {
        return playerCharacter;
    }

    public List<GameObject> GetAICompanions()
    {
        return aiCompanions;
    }
}