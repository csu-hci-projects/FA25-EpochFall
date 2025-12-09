using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private PlayerAnimatorBridge animBridge;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animBridge = GetComponent<PlayerAnimatorBridge>();   // ← get the attack status
    }

    void Update()
    {
        // If player is attacking, completely stop horizontal movement
        if (animBridge != null && animBridge.isAttacking)
        {
            // lock x movement, keep y for gravity
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;    // ✨ skip all movement input
        }

        // 2. Normal movement when NOT attacking
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // 3. Jump
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
