using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("AI Combat Settings")]
    public bool isAIControlled = false;
    public float aiAttackInterval = 2f;
    public float aiAttackRangeCheck = 2.5f;
    private float aiLastAttackTime;

    [Header("Combo System")]
    public float comboResetTime = 1.5f;
    public int maxComboCount = 3;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool drawGizmos = true;

    private Animator anim;
    private CharacterInputController cinput;
    private RootMotionControlScript rootMotion;
    private MainCharacterController mainCharController;

    private float lastAttackTime;
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool canAttack = true;

    // Attack hit detection
    public Transform attackPoint;
    public float attackRadius = 1.5f;
    public float attackPointHeight = 1f;
    public float attackPointSide = 0f;
    public float attackPointForward = 1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        cinput = GetComponent<CharacterInputController>();
        rootMotion = GetComponent<RootMotionControlScript>();
        mainCharController = GetComponent<MainCharacterController>();

        // If no attack point is set, create one at character position
        if (attackPoint == null)
        {
            GameObject ap = new GameObject("AttackPoint");
            ap.transform.parent = transform;
            ap.transform.localPosition = Vector3.forward * attackPointForward + Vector3.up * attackPointHeight + Vector3.right * attackPointSide;
            attackPoint = ap.transform;
        }
    }

    void Update()
    {
        // Determine if this character is AI-controlled
        if (mainCharController != null)
        {
            isAIControlled = mainCharController.IsAIControlled();
        }

        bool shouldAttack = false;
        // PLAYER CONTROLLED: Check for player input
        if (!isAIControlled && cinput != null && cinput.enabled)
        {
            if (cinput.Attack && canAttack)
            {
                shouldAttack = true;
            }
        }
        // AI CONTROLLED: Check for nearby enemies
        else if (isAIControlled)
        {
            if (canAttack && Time.time - aiLastAttackTime >= aiAttackInterval)
            {
                Transform nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.position);

                    if (distanceToEnemy <= aiAttackRangeCheck)
                    {
                        // Check if enemy is in front
                        Vector3 directionToEnemy = (nearestEnemy.position - transform.position).normalized;
                        float angle = Vector3.Angle(transform.forward, directionToEnemy);

                        if (angle < 60f) // Enemy is in front
                        {
                            shouldAttack = true;
                            aiLastAttackTime = Time.time;
                        }
                    }
                }
            }
        }

        // Execute attack if conditions are met
        if (shouldAttack)
        {
            TryAttack();
        }

        // Reset combo if too much time has passed
        if (Time.time - lastAttackTime > comboResetTime && currentComboIndex > 0)
        {
            ResetCombo();
        }

        // Update animator parameters
        if (anim != null)
        {
            anim.SetBool("isAttacking", isAttacking);
            anim.SetInteger("comboIndex", currentComboIndex);
        }
    }

    Transform FindNearestEnemy()
    {
        // Find all enemies in range
        Collider[] enemies = Physics.OverlapSphere(transform.position, aiAttackRangeCheck * 2f, enemyLayer);

        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider enemy in enemies)
        {
            // Don't attack self or friendly characters
            if (enemy.transform == transform)
                continue;

            // Check if it's actually an enemy (has HealthSystem and is alive)
            HealthSystem health = enemy.GetComponent<HealthSystem>();
            if (health != null && health.IsDead())
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    void TryAttack()
    {
        // Increment combo
        currentComboIndex++;
        if (currentComboIndex > maxComboCount)
            currentComboIndex = 1;

        // Trigger attack animation
        if (anim != null)
        {
            // Reset the trigger first to ensure clean state
            anim.ResetTrigger("attack");

            // Then set it
            anim.SetTrigger("attack");
            anim.SetInteger("comboIndex", currentComboIndex);
            anim.SetBool("isAttacking", true);
        }

        isAttacking = true;
        lastAttackTime = Time.time;

        // Start attack coroutine
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        // Wait a bit before doing hit detection (animation wind-up)
        yield return new WaitForSeconds(0.2f);

        // Perform hit detection
        PerformAttackHitDetection();

        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown);

        // Reset attack state
        isAttacking = false;
        canAttack = true;

        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
    }

    void PerformAttackHitDetection()
    {
        if (attackPoint == null) return;

        // Detect enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider enemy in hitEnemies)
        {
            // Don't hit self
            if (enemy.transform == transform)
                continue;

            // Check if enemy is in front of attacker
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            if (angle < 90f) // Enemy is in front
            {

                // Apply damage to enemy
                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage, transform.position);
                }

                // Alternative: Send message if enemy doesn't implement IDamageable
                //enemy.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void ResetCombo()
    {
        currentComboIndex = 0;
    }

    // Public methods for animation events
    public void OnAttackHit()
    {
        // Called by animation event at the moment of impact
        PerformAttackHitDetection();
    }

    public void OnAttackComplete()
    {
        // Called by animation event when attack animation finishes
        isAttacking = false;
    }

    // Public method to manually trigger attack (for AI or other systems)
    public void ForceAttack()
    {
        if (canAttack)
        {
            TryAttack();
        }
    }

    // Gizmos for visualization
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || attackPoint == null) return;

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        // Draw attack direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * attackRange);

        // Draw AI detection range
        if (isAIControlled)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, aiAttackRangeCheck);
        }
    }

    void OnDrawGizmos()
    {
        // Show AI status
        if (isAIControlled && attackPoint != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, aiAttackRangeCheck);
        }
    }
}

// Interface for damageable entities
public interface IDamageable
{
    void TakeDamage(float damage, Vector3 hitPosition);
}