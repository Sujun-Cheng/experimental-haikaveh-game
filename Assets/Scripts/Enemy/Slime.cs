using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Slime : MonoBehaviour
{
    public Transform player;
    public Animator animator;

    [Header("Ranges")]
    public float detectRadius = 10f;      // Sense Range
    public float attackRange  = 2.2f;     // Attack Range

    [Header("Attack")]
    public float attackCooldown = 1.0f;

    const string SenseStateName = "SenseSomethingST";

    NavMeshAgent agent;
    bool isSensing = false;
    bool hasEnteredSense = false; 
    bool isChasing = false;
    float nextAttackTime = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (animator) animator.applyRootMotion = false;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // 1. Sense
        if (!isSensing && !isChasing && dist <= detectRadius)
        {
            isSensing = true;
            hasEnteredSense = false;
            animator.SetTrigger("Sense");
        }

        if (isSensing)
        {
            agent.isStopped = true;
            SetMoveParams(Vector3.zero);
            animator.SetFloat("Speed", 0f);

            var info = animator.GetCurrentAnimatorStateInfo(0);
            bool inSense = info.IsName(SenseStateName) || info.IsName("Base Layer." + SenseStateName);

            if (inSense) hasEnteredSense = true;

            
            if (hasEnteredSense && !inSense)
            {
                isSensing = false;
                if (dist <= detectRadius)
                {
                    isChasing = true;
                    agent.isStopped = false;
                }
            }
            return; 
        }

        // 2. Chase & Attack
        if (isChasing)
        {
            if (dist <= attackRange)
            {
                agent.isStopped = true;
                SetMoveParams(Vector3.zero);

                if (Time.time >= nextAttackTime)
                {
                    animator.SetTrigger("Attack");
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                SetMoveParams(agent.desiredVelocity);
            }
        }
        else
        {
            agent.isStopped = true;
            SetMoveParams(Vector3.zero);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void SetMoveParams(Vector3 worldVel)
    {
        Vector3 local = transform.InverseTransformDirection(worldVel);
        Vector2 dir = new Vector2(local.x, local.z);
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveZ", dir.y);
    }
}
