using UnityEngine;
using System.Collections;

public class TrackingDrop : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D objectToDrop;
    public Transform player;

    [Header("Settings")]
    public float delayBeforeMove = 1f; // seconds before the object moves
    public float moveSpeed = 5f;       // speed of movement toward target
    public float returnSpeed = 3f;     // speed when returning
    public float stopDistance = 0.1f;  // distance at which we consider "arrived"

    private Vector2 startPosition;
    private bool isMoving = false;
    private bool isReturning = false;
    private bool canTrigger = true;

    void Start()
    {
        if (objectToDrop != null)
            startPosition = objectToDrop.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canTrigger && !isMoving)
        {
            StartCoroutine(MoveTowardPlayer());
        }
    }

    IEnumerator MoveTowardPlayer()
    {
        canTrigger = false;  // prevent re-entry mid-action
        yield return new WaitForSeconds(delayBeforeMove);

        if (player == null || objectToDrop == null)
        {
            Debug.LogWarning("Missing reference in TrackingDrop.");
            yield break;
        }

        isMoving = true;
        Vector2 targetPosition = player.position;

        // Move toward player's position
        while (Vector2.Distance(objectToDrop.position, targetPosition) > stopDistance)
        {
            objectToDrop.position = Vector2.MoveTowards(objectToDrop.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Once reached, return to start
        isMoving = false;
        isReturning = true;

        while (Vector2.Distance(objectToDrop.position, startPosition) > stopDistance)
        {
            objectToDrop.position = Vector2.MoveTowards(objectToDrop.position, startPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }

        objectToDrop.position = startPosition;
        isReturning = false;
        canTrigger = true;
    }
}
