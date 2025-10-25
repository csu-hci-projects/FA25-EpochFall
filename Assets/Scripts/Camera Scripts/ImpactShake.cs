using UnityEngine;

public class ImpactShake : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.2f;
    private CameraShake cameraShake;

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
                Destroy(collision.gameObject);
                Debug.Log("Player destroyed!");
            }

            // destroy the falling object itself
            Destroy(gameObject);
        }
    }
}
