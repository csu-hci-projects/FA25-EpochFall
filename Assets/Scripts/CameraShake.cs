using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cineCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine currentShake;

    void Start()
    {
        if (cineCam == null)
            cineCam = FindFirstObjectByType<CinemachineCamera>();

        if (cineCam != null)
            noise = cineCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        if (noise != null)
            noise.AmplitudeGain = 0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            StartCoroutine(ShakeRoutine(0.3f, 1.5f));
    }

    public void TriggerShake(float duration, float magnitude)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        if (noise == null) yield break;

        noise.AmplitudeGain = magnitude;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        noise.AmplitudeGain = 0f;
        currentShake = null;
    }

    void OnDisable()
    {
        if (noise != null)
            noise.AmplitudeGain = 0f;
    }
}
