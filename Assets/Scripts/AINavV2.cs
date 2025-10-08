using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI Navigation controller that makes characters follow a target
/// This version uses reflection to control RootMotionControlScript
/// </summary>
public class AINavV2 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    [Tooltip("Stop moving when this close to target")]
    public float stopDistance = 3f;

    [Tooltip("Start moving when further than this")]
    public float followDistance = 5f;

    [Tooltip("How fast to move (0-1)")]
    public float moveSpeed = 0.8f;

    [Header("Turning")]
    [Tooltip("How fast to turn toward target")]
    public float turnSpeed = 3f;

    [Header("Debug")]
    public bool showDebug = true;

    private NavMeshAgent navAgent;
    private Animator anim;
    private RootMotionControlScript rootMotion;

    // Reflection fields
    private FieldInfo inputForwardField;
    private FieldInfo inputTurnField;

    private float currentForward = 0f;
    private float currentTurn = 0f;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rootMotion = GetComponent<RootMotionControlScript>();

        // Configure NavMeshAgent
        if (navAgent != null)
        {
            navAgent.updatePosition = false; // Root motion handles position
            navAgent.updateRotation = false; // We handle rotation
            navAgent.stoppingDistance = stopDistance;
        }
    }

    void Start()
    {
        SetupReflection();

        if (showDebug)
        {
            Debug.Log($"✅ AINavV2 initialized on {gameObject.name}");
            if (player != null)
                Debug.Log($"   Target: {player.name}");
        }
    }

    void SetupReflection()
    {
        if (rootMotion == null)
        {
            Debug.LogError("AINavV2: RootMotionControlScript not found!");
            return;
        }

        System.Type type = typeof(RootMotionControlScript);
        inputForwardField = type.GetField("_inputForward", BindingFlags.NonPublic | BindingFlags.Instance);
        inputTurnField = type.GetField("_inputTurn", BindingFlags.NonPublic | BindingFlags.Instance);

        if (inputForwardField == null || inputTurnField == null)
        {
            Debug.LogError("AINavV2: Could not find _inputForward or _inputTurn fields!");
        }
        else if (showDebug)
        {
            Debug.Log("✅ AINavV2: Reflection setup successful");
        }
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
        if (showDebug)
            Debug.Log($"AINavV2: Target set to {(newTarget != null ? newTarget.name : "null")}");
    }

    void Update()
    {
        if (player == null || navAgent == null || !navAgent.isOnNavMesh)
        {
            SetInputs(0f, 0f);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update NavMesh destination
        if (distanceToPlayer > stopDistance)
        {
            navAgent.SetDestination(player.position);
        }

        // Calculate movement inputs
        CalculateMovement(distanceToPlayer);

        // Apply inputs via reflection
        SetInputs(currentForward, currentTurn);

        if (showDebug && Time.frameCount % 60 == 0) // Log every 60 frames
        {
            Debug.Log($"AI {gameObject.name}: Distance={distanceToPlayer:F1}m, Forward={currentForward:F2}, Turn={currentTurn:F2}");
        }
    }

    void CalculateMovement(float distanceToPlayer)
    {
        // FORWARD MOVEMENT
        if (distanceToPlayer <= stopDistance)
        {
            // Too close - stop
            currentForward = 0f;
        }
        else if (distanceToPlayer <= followDistance)
        {
            // In range - move slowly
            currentForward = 0.3f;
        }
        else
        {
            // Far away - move at full speed
            currentForward = moveSpeed;
        }

        // TURNING
        // Calculate direction TO player
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignore height difference
        directionToPlayer.Normalize();

        // Get current forward direction
        Vector3 currentForward3D = transform.forward;

        // Calculate angle difference (-180 to 180)
        float angleToPlayer = Vector3.SignedAngle(currentForward3D, directionToPlayer, Vector3.up);

        // Convert angle to turn input (-1 to 1)
        // Positive angle = need to turn right (positive input)
        // Negative angle = need to turn left (negative input)
        currentTurn = Mathf.Clamp(angleToPlayer / 45f, -1f, 1f) * turnSpeed;

        // Smooth the turn input
        currentTurn = Mathf.Clamp(currentTurn, -1f, 1f);
    }

    void SetInputs(float forward, float turn)
    {
        if (rootMotion == null) return;

        try
        {
            if (inputForwardField != null)
            {
                inputForwardField.SetValue(rootMotion, forward);
            }

            if (inputTurnField != null)
            {
                inputTurnField.SetValue(rootMotion, turn);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AINavV2: Error setting inputs - {e.Message}");
        }
    }

    void OnEnable()
    {
        if (navAgent != null)
            navAgent.enabled = true;
    }

    void OnDisable()
    {
        if (navAgent != null)
            navAgent.enabled = false;

        SetInputs(0f, 0f);
    }

    // Gizmos removed - no visual debug lines
}