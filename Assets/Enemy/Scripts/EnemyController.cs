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
        // 获取同一个物体上的 CombatController
        combatController = GetComponent<CombatController>();
        selfHealth = GetComponent<EnemyStatus>();
        if (playerTransform == null)
        {
            // 尝试通过Tag自动查找
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("EnemyAIController: 找不到 'Player' Tag 的物体!");
            }
        }

        if (combatController == null)
        {
            Debug.LogError("EnemyAIController: Can't find CombatController! No Attack.");
        }
    }

    void Update()
    {
        // 1) 死亡检查
        if (selfHealth != null && selfHealth.IsDead()) return;

        // 2) 目标检查
        if (targetNearestPlayer && Time.time >= _nextRetargetTime)
        {
            playerTransform = FindNearestAlivePlayer();
            _nextRetargetTime = Time.time + retargetInterval;
        }
        if (playerTransform == null) return;
        // --- 跟踪范围判定 ---
        float distanceToPlayerSqr = (playerTransform.position - transform.position).sqrMagnitude;
        float enterRangeSqr = trackEnterRange * trackEnterRange;
        float exitRangeSqr = trackExitRange * trackExitRange;

        // 当进入范围时开始追踪，超出更远距离时停止追踪（有回滞）
        if (!isTrackingPlayer && distanceToPlayerSqr <= enterRangeSqr)
            isTrackingPlayer = true;
        else if (isTrackingPlayer && distanceToPlayerSqr >= exitRangeSqr)
            isTrackingPlayer = false;

        // 如果当前不在跟踪状态，就不旋转
        if (!isTrackingPlayer)
            return;



        // --- 旋转（用恒定角速度 + 死区） ---
        Vector3 toPlayer = playerTransform.position - transform.position;
        Vector3 flatDir = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flatDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized);
            float currentAngle = Quaternion.Angle(transform.rotation, targetRot);

            // 只有超过死区角再转，避免开场轻微抖动
            const float deadZone = 1f; // 1°
            if (currentAngle > deadZone)
            {
                // RotateTowards：每秒最多转 turnSpeed 度
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime * 60f // turnSpeed 以“每秒度数”为语义更直观
                );
            }
        }

        // --- 攻击 ---
        if (Time.time - aiLastAttackTime < aiAttackInterval) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > aiAttackRangeCheck)
        {
            if (showDebugInfo) Debug.Log($"[AI] 等距离: {distance:F2} / 需要 ≤ {aiAttackRangeCheck}");
            return;
        }

        // 放宽锥角：更容易在移动中满足
        const float attackCone = 45f; // 你可调 30~60
        float angleToPlayer = Vector3.Angle(transform.forward, flatDir);
        if (angleToPlayer > attackCone)
        {
            if (showDebugInfo) Debug.Log($"[AI] 等角度: {angleToPlayer:F1}° / 需要 ≤ {attackCone}°");
            return;
        }

        // 到这里 => 满足距离+角度+冷却，可以攻击
        if (showDebugInfo) Debug.Log($"🤖 攻击 {playerTransform.name} | 距离 {distance:F2} | 角度 {angleToPlayer:F1}°");

        // 播放VFX
        if (attackVFXs != null && attackVFXs.Count > 0)
        {
            foreach (var vfx in attackVFXs) if (vfx != null) vfx.Play();
        }

        // 触发攻击
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
            // 如果你有玩家生命组件，替换成你自己的：
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


    // (Gizmos 也移过来，这样它就只在AI身上显示)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aiAttackRangeCheck);
    }
}