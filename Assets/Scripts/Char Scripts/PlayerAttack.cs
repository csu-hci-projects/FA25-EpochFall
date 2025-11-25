using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;          // where the sword is during impact
    public float attackRange = 0.5f;       // radius of hit area
    public int damage = 1;
    public LayerMask enemyLayers;          // which layers count as enemies

    // Called from Animation Events (twice per swing)
    public void DealAttackDamage()
    {
        // Find all colliders in the attack area
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    // Just to see it in the editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
