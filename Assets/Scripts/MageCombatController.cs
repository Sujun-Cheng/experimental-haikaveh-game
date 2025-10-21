using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCombatController : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 15f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;
    public LayerMask enemyLayer;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 20f;
    public float projectileLifetime = 3f;
    public float projectileSpawnHeight = 1.5f;
    public float projectileSpawnForward = 0.5f;

    [Header("AI Combat Settings")]
    public bool isAIControlled = false;
    public float aiAttackInterval = 2.5f;
    public float aiMinAttackRange = 5f;
    public float aiMaxAttackRange = 15f;
    private float aiLastAttackTime;

    [Header("Spell System")]
    public float spellCastTime = 0.3f;
    public int maxSpellLevel = 3;
    public float spellChargeTime = 2f;

    [Header("Debug")]
    public bool showDebugInfo = true;
    public bool drawGizmos = true;

    private Animator anim;
    private CharacterInputController cinput;
    private RootMotionControlScript rootMotion;
    private MainCharacterController mainCharController;

    private float lastAttackTime;
    private int currentSpellLevel = 1;
    private bool isCasting = false;
    private bool canAttack = true;
    private float spellChargeStartTime;

    void Awake()
    {
        anim = GetComponent<Animator>();
        cinput = GetComponent<CharacterInputController>();
        rootMotion = GetComponent<RootMotionControlScript>();
        mainCharController = GetComponent<MainCharacterController>();

        // If no projectile spawn point is set, create one
        if (projectileSpawnPoint == null)
        {
            GameObject sp = new GameObject("ProjectileSpawnPoint");
            sp.transform.parent = transform;
            sp.transform.localPosition = Vector3.forward * projectileSpawnForward + Vector3.up * projectileSpawnHeight;
            projectileSpawnPoint = sp.transform;
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
        Vector3 targetPosition = Vector3.zero;
        Transform targetEnemy = null;

        // PLAYER CONTROLLED: Check for player input
        if (!isAIControlled && cinput != null && cinput.enabled)
        {
            if (cinput.Attack && canAttack)
            {
                shouldAttack = true;
                // For player, aim in forward direction
                targetPosition = transform.position + transform.forward * attackRange;
            }
        }
        // AI CONTROLLED: Check for nearby enemies
        else if (isAIControlled)
        {
            if (canAttack && Time.time - aiLastAttackTime >= aiAttackInterval)
            {
                targetEnemy = FindTargetEnemy();
                if (targetEnemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);

                    // Attack if enemy is within range (not too close, not too far)
                    if (distanceToEnemy >= aiMinAttackRange && distanceToEnemy <= aiMaxAttackRange)
                    {
                        shouldAttack = true;
                        targetPosition = targetEnemy.position;
                        aiLastAttackTime = Time.time;
                    }
                }
            }
        }

        // Execute attack if conditions are met
        if (shouldAttack)
        {
            CastSpell(targetPosition, targetEnemy);
        }

        // Update animator parameters
        if (anim != null)
        {
            anim.SetBool("isCasting", isCasting);
            anim.SetInteger("spellLevel", currentSpellLevel);
        }
    }

    Transform FindTargetEnemy()
    {
        // Find all enemies in range
        Collider[] enemies = Physics.OverlapSphere(transform.position, aiMaxAttackRange, enemyLayer);

        Transform target = null;
        float bestScore = float.MinValue;

        foreach (Collider enemy in enemies)
        {
            // Don't attack self
            if (enemy.transform == transform)
                continue;

            // Check if it's actually an enemy (has HealthSystem and is alive)
            HealthSystem health = enemy.GetComponent<HealthSystem>();
            if (health != null && health.IsDead())
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Skip if too close or too far
            if (distance < aiMinAttackRange || distance > aiMaxAttackRange)
                continue;

            // Check if enemy is roughly in front
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);

            // Prefer enemies in front and at medium range
            float angleScore = Mathf.Max(0, 1f - (angle / 180f));
            float distanceScore = 1f - (distance / aiMaxAttackRange);
            float totalScore = angleScore * 0.7f + distanceScore * 0.3f;

            if (totalScore > bestScore)
            {
                bestScore = totalScore;
                target = enemy.transform;
            }
        }

        return target;
    }

    void CastSpell(Vector3 targetPosition, Transform targetEnemy = null)
    {
        if (!canAttack || isCasting)
            return;

        // Trigger casting animation
        if (anim != null)
        {
            anim.ResetTrigger("cast");
            anim.SetTrigger("cast");
            anim.SetInteger("spellLevel", currentSpellLevel);
            anim.SetBool("isCasting", true);
        }

        isCasting = true;
        lastAttackTime = Time.time;

        // Start casting coroutine
        StartCoroutine(CastSpellRoutine(targetPosition, targetEnemy));
    }

    IEnumerator CastSpellRoutine(Vector3 targetPosition, Transform targetEnemy)
    {
        canAttack = false;

        // Wait for spell cast time (animation wind-up)
        yield return new WaitForSeconds(spellCastTime);

        // Launch projectile
        LaunchProjectile(targetPosition, targetEnemy);

        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown);

        // Reset casting state
        isCasting = false;
        canAttack = true;

        if (anim != null)
        {
            anim.SetBool("isCasting", false);
        }
    }

    void LaunchProjectile(Vector3 targetPosition, Transform targetEnemy)
    {
        if (projectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogWarning("Projectile prefab or spawn point not set!");
            return;
        }

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        // Calculate direction
        Vector3 direction;
        if (targetEnemy != null)
        {
            // Aim at target with prediction
            direction = (targetEnemy.position - projectileSpawnPoint.position).normalized;
        }
        else
        {
            // Aim at target position
            direction = (targetPosition - projectileSpawnPoint.position).normalized;
        }

        // Set projectile rotation to face direction
        projectile.transform.rotation = Quaternion.LookRotation(direction);

        // Get or add projectile component
        MageProjectile projScript = projectile.GetComponent<MageProjectile>();
        if (projScript == null)
        {
            projScript = projectile.AddComponent<MageProjectile>();
        }

        // Initialize projectile
        projScript.Initialize(direction, projectileSpeed, attackDamage, projectileLifetime, enemyLayer, transform);
    }

    // Public methods for animation events
    public void OnSpellCastPoint()
    {
        // Called by animation event at the moment of spell release
        // Can be used for additional effects or early projectile launch
    }

    public void OnSpellComplete()
    {
        // Called by animation event when casting animation finishes
        isCasting = false;
    }

    // Public method to manually trigger spell (for AI or other systems)
    public void ForceCast(Vector3 targetPosition)
    {
        if (canAttack)
        {
            CastSpell(targetPosition);
        }
    }

    // Gizmos for visualization
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        // Draw attack range (max)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw projectile spawn point
        if (projectileSpawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(projectileSpawnPoint.position, 0.2f);
            Gizmos.DrawRay(projectileSpawnPoint.position, transform.forward * 2f);
        }

        // Draw AI detection range
        if (isAIControlled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, aiMaxAttackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aiMinAttackRange);
        }
    }

    void OnDrawGizmos()
    {
        // Show AI status
        if (isAIControlled)
        {
            Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, aiMaxAttackRange);
        }
    }
}

// Projectile script for mage spells
public class MageProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float lifetime;
    private LayerMask targetLayer;
    private Transform caster;
    private float spawnTime;

    public void Initialize(Vector3 dir, float spd, float dmg, float life, LayerMask layer, Transform source)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        lifetime = life;
        targetLayer = layer;
        caster = source;
        spawnTime = Time.time;
    }

    void Update()
    {
        // Move projectile
        transform.position += direction * speed * Time.deltaTime;

        // Check lifetime
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if hit is on target layer
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // Don't hit caster
            if (caster != null && other.transform == caster)
                return;

            // Apply damage
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform.position);
            }

            // Destroy projectile on hit
            Destroy(gameObject);
        }
        // Also destroy on environment collision
        else if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            Destroy(gameObject);
        }
    }
}