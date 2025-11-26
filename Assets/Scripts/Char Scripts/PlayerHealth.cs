using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private Animator anim;
    private bool isDead = false;
    public HealthBar healthBar;

    [Header("Invincibility Frames")]
    public float invulnerabilityDuration = 1f;   // seconds of invulnerability after hit
    public float flashInterval = 0.1f;           // how fast to flash
    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimatorBridge animBridge;


    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        anim = GetComponent<Animator>();
        animBridge = GetComponent<PlayerAnimatorBridge>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }


    void Die()
    {
        Debug.Log("Die() called");
        //reset triggers so no animation overrides death
        anim.ResetTrigger("Hurt");
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Die");

        if (animBridge != null)
            animBridge.isDead = true; // flag to stop updates instantly

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        PlayerMovement movementScript = GetComponent<PlayerMovement>();
        if (movementScript != null)
            movementScript.enabled = false;

        GetComponent<Collider2D>().enabled = false;

        Invoke(nameof(ReturnToMainMenu), 2f);
    }


    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        float elapsed = 0f;
        bool flashState = false;

        while (elapsed < invulnerabilityDuration)
        {
            // simple flash effect
            if (spriteRenderer != null)
            {
                flashState = !flashState;
                spriteRenderer.enabled = flashState;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // ensure sprite visible again
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvulnerable = false;
    }

}
