using UnityEngine;

public class ImpactShake : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.2f;
    private CameraShake cameraShake;

    public int damageAmount = 1;

    void Start()
    {
        cameraShake = FindFirstObjectByType<CameraShake>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} hit {collision.gameObject.name}");

            // trigger camera shake
            if (cameraShake != null)
                cameraShake.TriggerShake(shakeDuration, shakeMagnitude);

            // destroy the player if hit
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log("Player took damage");
                }
                else
                {
                    Debug.LogWarning("No PlayerHealth Component Found.");
                }
            }

            // destroy the falling object itself
            Destroy(gameObject);
        }
    }
}
