using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(SlimeCombat))]
public class SlimeAIController : MonoBehaviour
{
    // --- Component Refs ---
    private NavMeshAgent agent;
    private Animator anim;
    private EnemyStatus status;
    private SlimeCombat slimeCombat;
    private Transform playerTarget;

    // --- AI State ---
    private enum AIState { Idle, Patrol, Sensing, Chase, Attack }
    private AIState currentState;
    private bool hasSensedPlayer = false;

    // --- AI Targeting ---
    [Header("AI Targeting")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float senseRange = 8f;
    [SerializeField] private float retargetInterval = 0.2f;

    // --- AI Combat ---
    [Header("AI Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackInterval = 2f;
    private float attackCooldownTimer = 0f;

    // --- AI Patrol ---
    [Header("AI Patrol")]
    [SerializeField] private float idleTime = 5f;
    [SerializeField] private float sensingDuration = 1.5f;
    [SerializeField] private float patrolRange = 7f;
    private Vector3 startingPosition;
    private float stateTimer = 0f;
    private float retargetTimer = 0f;

    // --- AI Movement ---
    [Header("AI Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // --- CHANGE: Use GetComponentInChildren for consistency ---
        anim = GetComponentInChildren<Animator>(); 
        status = GetComponent<EnemyStatus>();
        slimeCombat = GetComponent<SlimeCombat>(); 

        startingPosition = transform.position;
        SwitchState(AIState.Idle);
    }

    void Update()
    {
        if (status.IsDead())
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.enabled = false;
                this.enabled = false;
            }
            return;
        }

        // Timers
        attackCooldownTimer -= Time.deltaTime;
        stateTimer -= Time.deltaTime;     // For idle and sensing
        retargetTimer -= Time.deltaTime;  // For targeting

        // Targeting as long as not in Sensing state
        if (currentState != AIState.Sensing && retargetTimer <= 0f)
        {
            UpdatePlayerTarget();
            retargetTimer = retargetInterval; // reset target timer
        }
        
        // State Machine
        switch (currentState)
        {
            case AIState.Idle:
                if (playerTarget != null)
                {
                    // 1. Spotted player
                    if (!hasSensedPlayer) SwitchState(AIState.Sensing); // only sense once in its lifetime
                    else SwitchState(AIState.Chase);
                }
                else if (stateTimer <= 0f)
                {
                    // 2. Idle time over
                    SwitchState(AIState.Patrol);
                }
                break;

            case AIState.Patrol:
                if (playerTarget != null)
                {
                    // 1. Spotted player
                    if (!hasSensedPlayer) SwitchState(AIState.Sensing);
                    else SwitchState(AIState.Chase);
                }
                else if (HasArrivedAtDestination())
                {
                    // 2. Got to patrol point
                    SwitchState(AIState.Idle);
                }
                break;

            case AIState.Sensing:
                if (playerTarget == null)
                {
                    // 1. Lost sight of player
                    SwitchState(AIState.Idle);
                }
                else if (stateTimer <= 0f)
                {
                    // 2. Sensing time over
                    hasSensedPlayer = true;
                    SwitchState(AIState.Chase);
                }
                else
                {
                    // Face the player while sensing
                    transform.LookAt(playerTarget);
                }
                break;

            case AIState.Chase:
                if (playerTarget == null)
                {
                    // 1. Lost player
                    SwitchState(AIState.Idle);
                }
                else
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
                    if (distanceToPlayer <= attackRange)
                    {
                        // 2. In attack range
                        SwitchState(AIState.Attack);
                    }
                    else
                    {
                        // 3. Continue chasing
                        if (agent.isOnNavMesh)
                        {
                            agent.SetDestination(playerTarget.position);
                        }
                    }
                }
                break;

            case AIState.Attack:
                if (playerTarget == null)
                {
                    // 1. Lost player
                    SwitchState(AIState.Idle);
                }
                else
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
                    if (distanceToPlayer > attackRange)
                    {
                        // 2. Out of attack range
                        SwitchState(AIState.Chase);
                    }
                    else
                    {
                        // 3. Attack!
                        PerformAttack();
                    }
                }
                break;
        }
        // Update animation speed parameter
        float currentAgentSpeed = agent.velocity.magnitude;
        anim.SetFloat("Speed", currentAgentSpeed, 0.1f, Time.deltaTime);
    }

    private void SwitchState(AIState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        
        switch (currentState)
        {
            case AIState.Idle:
            {
                agent.isStopped = true;
                agent.ResetPath();
                agent.speed = 0f;
                stateTimer = idleTime;
                break;
            }

            case AIState.Patrol:
            {
                agent.isStopped = false;
                agent.speed = patrolSpeed; 
                
                Vector3 randomPoint = startingPosition + (Random.insideUnitSphere * patrolRange);
                randomPoint.y = startingPosition.y;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, patrolRange, NavMesh.AllAreas))
                {
                    if (agent.isOnNavMesh)
                    {
                        agent.SetDestination(hit.position);
                    }
                }
                else
                {
                    SwitchState(AIState.Idle);
                }
                break;
            }

            case AIState.Sensing:
            {
                agent.isStopped = true; 
                agent.speed = 0f;
                
                if (playerTarget != null)
                {
                    transform.LookAt(playerTarget);
                }
                anim.SetTrigger("Sense");                
                stateTimer = sensingDuration;
                break;
            }

            case AIState.Chase:
            {
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                break;
            }

            case AIState.Attack:
            {
                agent.isStopped = true;
                agent.speed = 0f;
                break;
            }
        }
    }

    private void PerformAttack()
    {
        //transform.LookAt(playerTarget);


        if (attackCooldownTimer <= 0f)
        {
            transform.LookAt(playerTarget);
            anim.SetTrigger("Attack");
            attackCooldownTimer = attackInterval;
            
            slimeCombat.ExecuteAttack();
        }
    }

    private bool HasArrivedAtDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    private void UpdatePlayerTarget()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, senseRange, playerLayer);

        float closestDist = float.MaxValue;
        Transform closestTarget = null;

        foreach (var playerCollider in players)
        {
            float dist = Vector3.Distance(transform.position, playerCollider.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestTarget = playerCollider.transform;
            }
        }
        
        playerTarget = closestTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, senseRange); // Sense Range

        // Gizmos.color = Color.red; // This is now drawn by SlimeCombat.cs
        // Gizmos.DrawWireSphere(transform.position, attackRange); 
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startingPosition, patrolRange); // Patrol Range
    }
}