using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class AIMovement : MonoBehaviour
{
    [Header("AI Movement Settings")]
    public Transform player;
    public float followDistance = 0.5f;
    public float stopDistance = 0.1f;
    public float moveSpeed = 3.0f;
    public float turnSensitivity = 1.5f;

    [Header("AI Behavior")]
    public bool enableFollowing = true;
    public float updateRate = 0.1f;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private CharacterInputController inputController;
    private RootMotionControlScript rootMotionScript;
    private float lastUpdateTime;
    private Vector3 targetDirection;
    private float distanceToPlayer;

    // Reflection fields for accessing private members
    private FieldInfo inputForwardField;
    private FieldInfo inputTurnField;

    void Start()
    {
        // Get required components
        inputController = GetComponent<CharacterInputController>();
        rootMotionScript = GetComponent<RootMotionControlScript>();

        if (inputController == null)
        {
            Debug.LogError("AIMovement: CharacterInputController component not found!");
            return;
        }

        if (rootMotionScript == null)
        {
            Debug.LogError("AIMovement: RootMotionControlScript component not found!");
            return;
        }

        // Set up reflection to access RootMotionControlScript's cached input values
        SetupReflection();

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("AIMovement: Found player automatically");
            }
        }

        if (player == null)
        {
            Debug.LogWarning("AIMovement: No player assigned and none found with 'Player' tag!");
        }
    }

    void SetupReflection()
    {
        System.Type rootMotionType = typeof(RootMotionControlScript);

        // Get the private cached input fields from RootMotionControlScript
        inputForwardField = rootMotionType.GetField("_inputForward",
            BindingFlags.NonPublic | BindingFlags.Instance);
        inputTurnField = rootMotionType.GetField("_inputTurn",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (inputForwardField == null || inputTurnField == null)
        {
            Debug.LogError("AIMovement: Could not find required private fields in RootMotionControlScript.");
            Debug.LogError("Make sure the field names '_inputForward' and '_inputTurn' exist in RootMotionControlScript.");
        }
        else
        {
            Debug.Log("AIMovement: Reflection setup successful");
        }
    }

    void Update()
    {
        if (!enableFollowing || player == null || rootMotionScript == null)
        {
            // Set zero input when not following
            SetAIInput(0f, 0f);
            return;
        }

        // Update AI logic at specified rate for performance
        if (Time.time - lastUpdateTime >= updateRate)
        {
            UpdateAILogic();
            lastUpdateTime = Time.time;
        }

        // Apply the calculated input
        ApplyAIInput();
    }

    void UpdateAILogic()
    {
        // Calculate distance and direction to player
        Vector3 directionToPlayer = player.position - transform.position;
        distanceToPlayer = directionToPlayer.magnitude;

        // Remove Y component for horizontal movement calculation
        directionToPlayer.y = 0;
        targetDirection = directionToPlayer.normalized;

        if (showDebugInfo)
        {
            Debug.Log($"Distance to player: {distanceToPlayer:F2}");
        }
    }

    void ApplyAIInput()
    {
        float forwardInput = 0f;
        float turnInput = 0f;

        // Determine forward movement based on distance to player
        if (distanceToPlayer > followDistance)
        {
            // Too far from player, move toward them
            forwardInput = moveSpeed;
            if (showDebugInfo) Debug.Log("AI: Moving toward player");
        }
        else if (distanceToPlayer < stopDistance)
        {
            // Too close to player, stop
            forwardInput = 0f;
            if (showDebugInfo) Debug.Log("AI: Too close, stopping");
        }
        else
        {
            // In acceptable range, move slowly
            //float distanceRatio = (distanceToPlayer - stopDistance) / (followDistance - stopDistance);
            //forwardInput = moveSpeed * distanceRatio * 0.3f;
            //if (showDebugInfo) Debug.Log($"AI: In range, slow movement: {forwardInput:F2}");

            // In range, keep moving at normal speed
            forwardInput = moveSpeed;
        }

        // Calculate turn input based on angle to target
        if (targetDirection != Vector3.zero && distanceToPlayer > stopDistance * 0.5f)
        {
            float angleToTarget = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);

            // Convert angle to turn input (-1 to 1)
            turnInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f) * turnSensitivity;

            if (showDebugInfo) Debug.Log($"AI: Angle to target: {angleToTarget:F1}°, Turn input: {turnInput:F2}, following player: {player}");
        }

        // Apply the input
        SetAIInput(forwardInput, turnInput);
    }

    void SetAIInput(float forward, float turn)
    {
        if (rootMotionScript == null) return;

        try
        {
            // Directly set the cached input values in RootMotionControlScript
            if (inputForwardField != null)
            {
                inputForwardField.SetValue(rootMotionScript, forward);
            }
            if (inputTurnField != null)
            {
                inputTurnField.SetValue(rootMotionScript, turn);
            }

            if (showDebugInfo && (forward != 0 || turn != 0))
            {
                Debug.Log($"AI Input Set - Forward: {forward:F2}, Turn: {turn:F2}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AIMovement: Error setting AI input: {e.Message}");
        }
    }

    // Public methods for external control
    public void SetFollowing(bool follow)
    {
        enableFollowing = follow;
        if (!follow)
        {
            SetAIInput(0f, 0f);
        }
        Debug.Log($"AIMovement: Following set to {follow}");
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
        Debug.Log($"AIMovement: Target set to {(newTarget ? newTarget.name : "null")}");
    }

    // Visualize AI behavior in Scene view
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Draw follow distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        // Draw stop distance  
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Draw line to player
        Gizmos.color = distanceToPlayer > followDistance ? Color.red :
                      distanceToPlayer < stopDistance ? Color.blue : Color.green;
        Gizmos.DrawLine(transform.position, player.position);

        // Draw forward direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // Draw target direction
        if (targetDirection != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, targetDirection * 1.5f);
        }

        // Draw distance info
        UnityEngine.GUIStyle style = new UnityEngine.GUIStyle();
        style.normal.textColor = Color.white;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
            $"Dist: {distanceToPlayer:F1}m", style);
    }

    void OnDrawGizmos()
    {
        // Always show basic info
        if (player != null)
        {
            Gizmos.color = enableFollowing ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.1f,
                           player.position + Vector3.up * 0.1f);
        }
    }
}