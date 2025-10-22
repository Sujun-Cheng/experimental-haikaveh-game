using UnityEngine;

/// <summary>
/// Controls AI character behavior - attach ONLY to AI companions
/// </summary>
public class MainCharacterController : MonoBehaviour
{
    public string Name = "AI Companion";
    public bool useV2 = true;

    [Header("AI Settings")]
    [Tooltip("The player character this AI should follow")]
    public Transform playerTarget;

    [Tooltip("What state should this AI start in?")]
    public ControllState initialControlState = ControllState.AIControlledFollowing;

    private CharacterInputController cinput;
    private RootMotionControlScript rootMotionControlScript;
    private AINavV2 aiFollowController;
    private AIMovement aiMovementScript;
    private CombatController combatController;

    public enum ControllState
    {
        PlayerControlled,
        AIControlledIdle,
        AIControlledFollowing,
        AIControlledFighting
    };

    public ControllState currentControlState;

    private void Start()
    {
        // Get all components
        cinput = GetComponent<CharacterInputController>();
        rootMotionControlScript = GetComponent<RootMotionControlScript>();
        aiFollowController = GetComponent<AINavV2>();
        aiMovementScript = GetComponent<AIMovement>();
        combatController = GetComponent<CombatController>();

        // Set initial state
        currentControlState = initialControlState;

        // Apply the initial state
        switch (initialControlState)
        {
            case ControllState.AIControlledIdle:
                switchToAIControlledIdle();
                Debug.Log($"✅ {Name} started as AIControlledIdle");
                break;

            case ControllState.AIControlledFollowing:
                switchToAIControlledFollowing(playerTarget);
                Debug.Log($"✅ {Name} started as AIControlledFollowing");
                break;

            case ControllState.AIControlledFighting:
                switchToAIControlledFighting(null);
                Debug.Log($"⚠️ {Name} started in Fighting state but has no target");
                break;
        }
    }

    private void Update()
    {
        // Continuously update target position for following
        if (currentControlState == ControllState.AIControlledFollowing && playerTarget != null)
        {
            if (useV2 && aiFollowController != null && aiFollowController.enabled)
            {
                // AINavV2 handles continuous updates in its own Update
            }
            else if (aiMovementScript != null && aiMovementScript.enabled)
            {
                // AIMovement handles continuous updates in its own Update
            }
        }
    }

    // ===== PUBLIC SWITCHING METHODS =====
    public void switchToPlayerControlled()
    {
        Debug.Log($"Switching {Name} to PlayerControlled");

        // Disable player input
        if (cinput != null)
            cinput.enabled = true;

        // Configure AI based on version
        if (aiFollowController != null)
            aiFollowController.enabled = false;
        if (aiMovementScript != null)
            aiMovementScript.enabled = false;

        // Disable combat in idle state
        if (combatController != null)
            combatController.enabled = true;

        currentControlState = ControllState.PlayerControlled;

        Debug.Log($"✅ {Name} is now PlayerControlled");
    }
    public void switchToAIControlledIdle()
    {
        Debug.Log($"🤖 Switching {Name} to AIControlledIdle");

        // Disable player input
        if (cinput != null)
            cinput.enabled = false;

        // Configure AI based on version
        if (useV2)
        {
            if (aiFollowController != null)
                aiFollowController.enabled = true;
            if (aiMovementScript != null)
                aiMovementScript.enabled = false;
        }
        else
        {
            if (aiMovementScript != null)
                aiMovementScript.enabled = true;
            if (aiFollowController != null)
                aiFollowController.enabled = false;
        }

        // Disable combat in idle state
        if (combatController != null)
            combatController.enabled = false;

        currentControlState = ControllState.AIControlledIdle;

        Debug.Log($"✅ {Name} is now AIControlledIdle");
    }

    public void switchToAIControlledFollowing(Transform target)
    {
        Debug.Log($"🤖 Switching {Name} to AIControlledFollowing");

        // Disable player input
        if (cinput != null)
            cinput.enabled = false;

        // Enable appropriate AI system
        if (useV2 && aiFollowController != null)
        {
            aiFollowController.enabled = true;
            if (target != null)
            {
                aiFollowController.SetTarget(target);
                playerTarget = target;
            }

            if (aiMovementScript != null)
                aiMovementScript.enabled = false;
        }
        else if (aiMovementScript != null)
        {
            aiMovementScript.enabled = true;
            if (target != null)
            {
                aiMovementScript.SetTarget(target);
                playerTarget = target;
            }

            if (aiFollowController != null)
                aiFollowController.enabled = false;
        }

        // Enable combat for companion protection
        if (combatController != null)
            combatController.enabled = true;

        currentControlState = ControllState.AIControlledFollowing;

        Debug.Log($"✅ {Name} is now AIControlledFollowing {(target != null ? target.name : "no target")}");
    }

    public void switchToAIControlledFighting(Transform target)
    {
        Debug.Log($"⚔️ Switching {Name} to AIControlledFighting");

        // Disable player input
        if (cinput != null)
            cinput.enabled = false;

        // Enable appropriate AI system and set target
        if (useV2 && aiFollowController != null)
        {
            aiFollowController.enabled = true;
            if (target != null)
                aiFollowController.SetTarget(target);

            if (aiMovementScript != null)
                aiMovementScript.enabled = false;
        }
        else if (aiMovementScript != null)
        {
            aiMovementScript.enabled = true;
            if (target != null)
                aiMovementScript.SetTarget(target);

            if (aiFollowController != null)
                aiFollowController.enabled = false;
        }

        // Always enable combat in fighting state
        if (combatController != null)
            combatController.enabled = true;

        currentControlState = ControllState.AIControlledFighting;

        Debug.Log($"✅ {Name} is now AIControlledFighting {(target != null ? target.name : "no target")}");
    }

    // ===== UTILITY METHODS =====

    public bool IsAIControlled()
    {
        return currentControlState != ControllState.PlayerControlled; // Always true for AI companions
    }

    public void SetControlState(ControllState newState)
    {
        switch (newState)
        {
            case ControllState.AIControlledIdle:
                switchToAIControlledIdle();
                break;
            case ControllState.AIControlledFollowing:
                switchToAIControlledFollowing(playerTarget);
                break;
            case ControllState.AIControlledFighting:
                switchToAIControlledFighting(null);
                break;
            case ControllState.PlayerControlled:
                switchToPlayerControlled();
                break;
        }
    }

    public void SetPlayerTarget(Transform target)
    {
        playerTarget = target;
        if (currentControlState == ControllState.AIControlledFollowing)
        {
            switchToAIControlledFollowing(target);
        }
    }
}