using UnityEngine;

public class RexAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.8f;
    public int damage = 1;

    [Header("Knockback")]
    public float knockbackForce = 6f;   // horizontal push
    public float knockupForce = 2f;     // vertical lift

    private const string PLAYER_TAG = "Player";

    // Called by animation event
    public void DealAttackDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("RexAttack: attackPoint not assigned!");
            return;
        }

        // Check all colliders in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange
        );

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(PLAYER_TAG))
                continue;

            // 1) Apply damage
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Rex hit {player.name} for {damage} damage");
            }

            // 2) Apply knockback
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // direction from Rex to player (push away from Rex)
                float dir = Mathf.Sign(hit.transform.position.x - transform.position.x);
                Vector2 force = new Vector2(dir * knockbackForce, knockupForce);

                rb.linearVelocity = Vector2.zero;                  // reset current motion
                rb.AddForce(force, ForceMode2D.Impulse);          // apply knockback
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
