using UnityEngine;
using Unity.Cinemachine;

public class CamPanOutForArena : MonoBehaviour
{
    [Header("Camera Pan Settings")]
    [Tooltip("X position threshold to trigger camera pan out")]
    public float triggerXPosition = 275f;
    
    [Tooltip("Target orthographic size when panning out")]
    public float targetOrthographicSize = 12f;
    
    [Tooltip("Speed of camera zoom transition")]
    public float zoomSpeed = 2f;

    private CinemachineCamera virtualCamera;
    private Transform playerTransform;
    private float initialOrthographicSize;
    private bool hasPannedOut = false;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        if (virtualCamera == null)
            Debug.LogError("CamPanOutForArena: No CinemachineVirtualCamera found on this GameObject");

        initialOrthographicSize = virtualCamera.Lens.OrthographicSize;

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("CamPanOutForArena: No player found with tag 'Player'");
    }

    void Update()
    {
        if (playerTransform == null || hasPannedOut)
            return;

        // Check if player has passed the trigger x position
        if (playerTransform.position.x >= triggerXPosition)
        {
            hasPannedOut = true;
        }
    }

    void LateUpdate()
    {
        if (virtualCamera == null || !hasPannedOut)
            return;

        // Smoothly transition to target orthographic size
        CinemachinePositionComposer framingTransposer = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        if (framingTransposer != null)
        {
            framingTransposer.CameraDistance = Mathf.Lerp(
                framingTransposer.CameraDistance,
                targetOrthographicSize,
                Time.deltaTime * zoomSpeed
            );

            // Stop lerping once close enough to target
            if (Mathf.Abs(framingTransposer.CameraDistance - targetOrthographicSize) < 0.01f)
            {
                framingTransposer.CameraDistance = targetOrthographicSize;
            }
        }
        else
        {
            // Fallback: directly modify orthographic size
            virtualCamera.Lens.OrthographicSize = Mathf.Lerp(
                virtualCamera.Lens.OrthographicSize,
                targetOrthographicSize,
                Time.deltaTime * zoomSpeed
            );

            if (Mathf.Abs(virtualCamera.Lens.OrthographicSize - targetOrthographicSize) < 0.01f)
            {
                virtualCamera.Lens.OrthographicSize = targetOrthographicSize;
            }
        }
    }
}
