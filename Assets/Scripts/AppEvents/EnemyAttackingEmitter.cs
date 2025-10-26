using UnityEngine;

public class EnemyAttackingEmitter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SignalHostileEnemy()
    {
        print("enemy detected player, attacking");
        EventManager.TriggerEvent<EnemyAttackingEvent, Vector3>(transform.position);
    }
}
