using UnityEngine;

// This script only executes the attack, it doesn't decide when.
public class SlimeCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRadius = 1.8f;
    [SerializeField] private LayerMask playerLayer; // Set this to "player"

    /// <summary>
    /// This function is called by SlimeAIController via 'ExecuteAttack()'
    /// </summary>
    public void ExecuteAttack()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            IDamageable playerHealth = player.GetComponent<IDamageable>(); 
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage, transform.position); 
                
                break;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f); // Use a semi-transparent red
        Gizmos.DrawSphere(transform.position, attackRadius);
    }
}