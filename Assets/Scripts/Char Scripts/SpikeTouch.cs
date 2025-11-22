using UnityEngine;

public class SpikeTouch : MonoBehaviour
{
    [Tooltip("Damage applied to player when touching a spike")]
    public int damageAmount = 3;

    private PlayerHealth playerHealth;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
            Debug.LogWarning("SpikeTouch: No PlayerHealth component found on the same GameObject.");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAndApplySpikeDamage(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CheckAndApplySpikeDamage(other.gameObject);
    }

    private void CheckAndApplySpikeDamage(GameObject other)
    {
        if (other == null) return;

        bool isSpike = false;

        // 1) common: objects explicitly tagged as Spike
        if (other.CompareTag("Spike"))
            isSpike = true;

        // 2) world-side SpikeDamage component (use string lookup to avoid compile-order/type reference issues)
        if (!isSpike && other.GetComponent("SpikeDamage") != null)
            isSpike = true;

        // 3) fallback: name contains "spike"
        if (!isSpike && other.name.ToLower().Contains("spike"))
            isSpike = true;

        if (isSpike)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log($"Player touched spike {other.name} and took {damageAmount} damage.");
            }
            else
            {
                Debug.LogWarning("SpikeTouch: Can't apply damage because PlayerHealth is missing.");
            }
        }
    }
}
