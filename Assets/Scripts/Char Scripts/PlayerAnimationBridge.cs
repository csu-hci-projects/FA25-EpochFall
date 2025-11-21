using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimatorBridge : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;
    bool isAttacking = false;

    [HideInInspector] public bool isDead = false;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckOffset = 0.05f;
    public int stableFramesNeeded = 3;
    int groundedFrames = 0;
    bool isGrounded;
    Vector3 baseScale;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (isDead) return; //stop animator updates immediately 
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Attack"))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
        // Ground check origin at the bottom of the collider
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);

        bool groundHit = Physics2D.BoxCast(
            origin,
            new Vector2(col.bounds.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            groundCheckOffset,
            groundLayer
        );
        // Stable-frame smoothing
        if (groundHit)
            groundedFrames = Mathf.Min(groundedFrames + 1, stableFramesNeeded);
        else
            groundedFrames = Mathf.Max(groundedFrames - 1, 0);

        isGrounded = groundedFrames >= stableFramesNeeded / 2;

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));


        if (rb.linearVelocity.x > 0.05f)
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        else if (rb.linearVelocity.x < -0.05f)
            transform.localScale = new Vector3(-Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
    }

    void Update()
    {
        if (isAttacking) return;
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
    }

    // Draw ground check box for debugging
    void OnDrawGizmos()
    {
        if (col == null) return;

        Gizmos.color = Color.green;
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 size = new Vector2(col.bounds.size.x * 0.9f, 0.1f);
        Gizmos.DrawWireCube(origin + Vector2.down * groundCheckOffset, size);
    }
}
