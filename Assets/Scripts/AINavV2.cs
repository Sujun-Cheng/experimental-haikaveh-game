using System.Collections;
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
    [Tooltip("Start sprinting when further than this")]
    public float sprintDistance = 20f;

    [Tooltip("How fast to move (0-1)")]
    public float moveSpeed = 0.8f;

    [Header("Turning")]
    [Tooltip("How fast to turn toward target")]
    public float turnSpeed = 3f;

    [Header("Debug")]
    public bool showDebug = true;

    private NavMeshAgent navAgent;
    private Animator anim;
    private Rigidbody rb;
    private RootMotionControlScript rootMotion;

    // Reflection fields
    private FieldInfo inputForwardField;
    private FieldInfo inputTurnField;
    private FieldInfo sprintFlag;
    private FieldInfo jumpFlag;

    private float currentForward = 0f;
    private float currentTurn = 0f;
    private bool currentSprint = false;
    private bool isOffMesh = false;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rootMotion = GetComponent<RootMotionControlScript>();
        rb = GetComponent<Rigidbody>();

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
        navAgent.autoTraverseOffMeshLink = false;
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
        sprintFlag = type.GetField("_dash", BindingFlags.NonPublic | BindingFlags.Instance);
        jumpFlag = type.GetField("_jump", BindingFlags.NonPublic | BindingFlags.Instance);
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
        if (player == null || navAgent == null)
        {
            SetInputs(0f, 0f, false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update NavMesh destination
        //if (distanceToPlayer > stopDistance)
        //{
        if (navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(player.position);
        }
        
        if (navAgent.isOnOffMeshLink)
        {
            if (!isOffMesh)
            {
                isOffMesh = true;
                var link = navAgent.currentOffMeshLinkData;
                rb.isKinematic = true;
                Vector3 directionVector = (link.endPos - link.startPos);
                directionVector.y = 0;
                directionVector.Normalize();
                
                inputTurnField.SetValue(rootMotion, 0);
                inputForwardField.SetValue(rootMotion, 0);

                print($"jumpFlag: {jumpFlag}");
                navAgent.updatePosition = true;
                navAgent.updateRotation = true;
                //inputForwardField.SetValue(rootMotion, directionVector.z);
                //inputTurnField.SetValue(rootMotion, directionVector.x);
                //jumpFlag.SetValue(rootMotion, true);
                StartCoroutine(DoOffMeshLink(link, 1f));

            }
        }
        else
        {
            isOffMesh = false;
            jumpFlag.SetValue(rootMotion, false);
            rb.isKinematic = false;
            navAgent.updatePosition = false;
            navAgent.updateRotation = false;
            //}

            if (rootMotion.IsGrounded)
            {
                if (!navAgent.enabled)
                {
                    navAgent.enabled = true;
                }
                // Calculate movement inputs
                CalculateMovement(distanceToPlayer);

                // Apply inputs via reflection
                SetInputs(currentForward, currentTurn, currentSprint);
            } else
            {
                navAgent.enabled = false;
            }
            if (showDebug && Time.frameCount % 60 == 0) // Log every 60 frames
            {
                Debug.Log($"AI {gameObject.name}: Distance={distanceToPlayer:F1}m, Forward={currentForward:F2}, Turn={currentTurn:F2}");
            }
        }
        
    }

    void CalculateMovement(float distanceToPlayer)
    {
        float speedMultiplier = 0;
        currentSprint = false;
        print($"distance between follower and target: {distanceToPlayer}");
        // FORWARD MOVEMENT
        if (distanceToPlayer <= stopDistance)
        {
            print($"distance between follower and target: {distanceToPlayer}, stopping");
            // Too close - stop
            speedMultiplier = 0f;
        }
        else if (distanceToPlayer <= followDistance)
        {
            print($"distance between follower and target: {distanceToPlayer}, slowing down");
            // In range - move slowly
            speedMultiplier = 0.5f;
        }
        else if (distanceToPlayer <= sprintDistance)
        {
            print($"distance between follower and target: {distanceToPlayer}, running: distance to player: {distanceToPlayer}, {sprintDistance}");
            // Far away - move at full speed
            speedMultiplier = moveSpeed;
        } else
        {
            print($"distance between follower and target: {distanceToPlayer}, dashing");
            print($"distance threshold reached. setting speed parameter: {currentSprint}");
            speedMultiplier = 1f;
            currentSprint = true;

        }
            distanceToPlayer = Vector3.Distance(transform.position, navAgent.steeringTarget);
        print($"navAgent.steeringTarget: {navAgent.steeringTarget}");

        // TURNING
        // Calculate direction TO player
        Vector3 directionToPlayer = navAgent.steeringTarget - transform.position;
        directionToPlayer.y = 0; // Ignore height difference
        directionToPlayer.Normalize();

        // Get current forward direction
        //Vector3 currentForward3D = transform.forward;

        // Calculate angle difference (-180 to 180)
        //float angleToPlayer = Vector3.SignedAngle(currentForward3D, directionToPlayer, Vector3.up);

        // Convert angle to turn input (-1 to 1)
        // Positive angle = need to turn right (positive input)
        // Negative angle = need to turn left (negative input)
        //currentTurn = Mathf.Clamp(angleToPlayer / 45f, -1f, 1f) * turnSpeed;

        //// Smooth the turn input
        //currentTurn = Mathf.Clamp(currentTurn, -1f, 1f);
        //currentTurn = angleToPlayer;
        currentForward = directionToPlayer.z * speedMultiplier;

        currentTurn = directionToPlayer.x * speedMultiplier;
        navAgent.nextPosition = anim.rootPosition;

        navAgent.transform.rotation = transform.rotation;
        
    }

    

    void SetInputs(float forward, float turn, bool sprint)
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

            if (sprintFlag != null)
            {
                print($"sprintFlag: {currentSprint}");
                sprintFlag.SetValue(rootMotion, currentSprint);
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

        SetInputs(0f, 0f, false);
    }

    // Gizmos removed - no visual debug lines


    IEnumerator DoOffMeshLink(OffMeshLinkData link, float duration)
    {
        Vector3 directionVector = (link.endPos - navAgent.transform.position);

        Vector3 startPos = transform.position;
        Vector3 endPos = link.endPos;
        navAgent.isStopped = true;
        Vector3 lookAt = endPos;
        

            //directionVector.Normalize();
            
            print($"jumpFlag: {jumpFlag}");
           
        float normalizedTime = 0f;
        while (normalizedTime < 1f)
        {
            float yOffset = 5.0f * (normalizedTime - normalizedTime * normalizedTime);
            transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
            anim.rootPosition = transform.position;
            navAgent.nextPosition = transform.position;
            navAgent.transform.position = transform.position;
            normalizedTime += Time.deltaTime / duration;
            yield return new WaitForSeconds(0);
        }
        navAgent.CompleteOffMeshLink();
        navAgent.isStopped = false;
    }
}