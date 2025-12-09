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

    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isAttacking = false;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckOffset = 0.05f;
    public int stableFramesNeeded = 3;
    int groundedFrames = 0;
    bool isGrounded;

    Vector3 baseScale;

    [Header("Attack Phases")]
    public float accelPhaseDuration = 0.15f;   // how long the "startup" phase lasts
    public float runSpeedThreshold = 2.0f;     // speed considered "running"

    float accelTimer = 0f;
    bool inAccelPhase = false;
    bool wasMovingHoriz = false;

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
        if (isDead) return; // stop animator updates if dead
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        isAttacking = state.IsTag("Attack");


        bool movingHoriz = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

        // just started moving this frame -> enter accel phase
        if (movingHoriz && !wasMovingHoriz)
        {
            accelTimer = 0f;
            inAccelPhase = true;
        }

        if (inAccelPhase)
        {
            accelTimer += Time.fixedDeltaTime;

            // leave accel phase once timer expires or we stop
            if (accelTimer >= accelPhaseDuration || !movingHoriz)
            {
                inAccelPhase = false;
            }
        }

        // if we fully stop, reset
        if (!movingHoriz)
        {
            inAccelPhase = false;
            accelTimer = 0f;
        }

        wasMovingHoriz = movingHoriz;

        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);

        bool groundHit = Physics2D.BoxCast(
            origin,
            new Vector2(col.bounds.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            groundCheckOffset,
            groundLayer
        );

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
        if (isDead) return;

        // do not start a new attack while already attacking
        if (isAttacking) return;

        if (Input.GetMouseButtonDown(0))
        {
            float speedX = Mathf.Abs(rb.linearVelocity.x);

            bool runningEnough = speedX > runSpeedThreshold && !inAccelPhase;

            if (runningEnough)
            {
                // already fully running -> dash attack
                anim.SetTrigger("DashAttack");
            }
            else
            {
                // standing still or in early accel -> normal standing attack
                anim.SetTrigger("Attack");
            }
        }
    }

    void OnDrawGizmos()
    {
        if (col == null) return;

        Gizmos.color = Color.green;
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 size = new Vector2(col.bounds.size.x * 0.9f, 0.1f);
        Gizmos.DrawWireCube(origin + Vector2.down * groundCheckOffset, size);
    }
}
