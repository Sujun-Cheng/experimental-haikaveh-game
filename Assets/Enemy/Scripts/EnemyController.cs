using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("AI Combat Settings")]
    public float aiAttackInterval = 2f;
    public float aiAttackRangeCheck = 2.5f;
    public LayerMask enemyLayer;
    [Header("VFX")]
    public List<ParticleSystem> attackVFXs;
    [Header("AI Targeting")]
    public Transform playerTransform;
    public float turnSpeed = 5f;
    [Header("Targeting Mode")]
    [SerializeField] bool targetNearestPlayer = true;
    [SerializeField] float retargetInterval = 0.2f;
    [Header("Tracking Range")]
    public float trackEnterRange = 3f;
    public float trackExitRange = 3f;
    private bool isTrackingPlayer = false;
    float _nextRetargetTime;
    [Header("Debug")]
    public bool showDebugInfo = true;


    private CombatController combatController;
    private EnemyStatus selfHealth; 
    // ----------------

    private float aiLastAttackTime;

    void Awake()
    {
        // get CombatController on same GameObject
        combatController = GetComponent<CombatController>();
        selfHealth = GetComponent<EnemyStatus>();
        if (playerTransform == null)
        {
            // use the first found Player if not assigned
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("EnemyController: can't find 'Player' Tag object!");
            }
        }

        if (combatController == null)
        {
            Debug.LogError("EnemyController: Can't find CombatController! No Attack.");
        }
    }

    void Update()
    {
        // dead check
        if (selfHealth != null && selfHealth.IsDead()) return;

        // 2) objectives check
        if (targetNearestPlayer && Time.time >= _nextRetargetTime)
        {
            playerTransform = FindNearestAlivePlayer();
            _nextRetargetTime = Time.time + retargetInterval;
        }
        if (playerTransform == null) return;
        // tracking 
        float distanceToPlayerSqr = (playerTransform.position - transform.position).sqrMagnitude;
        float enterRangeSqr = trackEnterRange * trackEnterRange;
        float exitRangeSqr = trackExitRange * trackExitRange;

        // tracking when enter, stop tracking when exit
        if (!isTrackingPlayer && distanceToPlayerSqr <= enterRangeSqr)
            isTrackingPlayer = true;
        else if (isTrackingPlayer && distanceToPlayerSqr >= exitRangeSqr)
            isTrackingPlayer = false;

        // No turning/attacking if not tracking
        if (!isTrackingPlayer)
            return;



        // turn to face player
        Vector3 toPlayer = playerTransform.position - transform.position;
        Vector3 flatDir = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flatDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized);
            float currentAngle = Quaternion.Angle(transform.rotation, targetRot);

            // dead zone to prevent jittering
            const float deadZone = 1f; // 1°
            if (currentAngle > deadZone)
            {
                // RotateTowards: smooth turning
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime * 60f 
                );
            }
        }

        // Attack 
        if (Time.time - aiLastAttackTime < aiAttackInterval) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > aiAttackRangeCheck)
        {
            if (showDebugInfo) Debug.Log($"[AI] current distance: {distance:F2} / should ≤ {aiAttackRangeCheck}");
            return;
        }
        // check attack angle
        const float attackCone = 45f; 
        float angleToPlayer = Vector3.Angle(transform.forward, flatDir);
        if (angleToPlayer > attackCone)
        {
            if (showDebugInfo) Debug.Log($"[AI] current angle: {angleToPlayer:F1}° / should ≤ {attackCone}°");
            return;
        }

        // ready to attack
        if (showDebugInfo) Debug.Log($"🤖 attack {playerTransform.name} | distance {distance:F2} | angle {angleToPlayer:F1}°");

        // play VFX
        if (attackVFXs != null && attackVFXs.Count > 0)
        {
            foreach (var vfx in attackVFXs) if (vfx != null) vfx.Play();
        }

        // initiate attack
        combatController.ForceAttack();
        aiLastAttackTime = Time.time;
    }
    Transform FindNearestAlivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var go in players)
        {
            var t = go.transform;

            var health = t.GetComponent<HealthSystem>();
            if (health != null && health.IsDead()) continue;

            float d = Vector3.SqrMagnitude(t.position - transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }
        return best;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aiAttackRangeCheck);
    }
}