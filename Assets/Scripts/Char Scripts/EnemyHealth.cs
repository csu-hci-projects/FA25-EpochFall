using UnityEngine;
using Assets.PixelFantasy.PixelMonsters.Common.Scripts.ExampleScripts;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    public HealthBar healthBar;

    private MonsterAnimation monsterAnim;
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        monsterAnim = GetComponent<MonsterAnimation>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        // play hit reaction 
        if (monsterAnim != null)
        {
            monsterAnim.Hit();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died!");


        if (monsterAnim != null)
        {
            monsterAnim.Die();   // sets MonsterState.Die → Animator bool "Die" = true
        }

        healthBar.gameObject.SetActive(false);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        // if (col != null) col.enabled = false;  
        Destroy(gameObject, 0.7f);
    }
}
