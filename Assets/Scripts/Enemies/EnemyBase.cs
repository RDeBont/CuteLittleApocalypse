using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] protected int contactDamage = 1;

    protected int currentHealth;
    private bool isDead;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        // Twee kogels in hetzelfde frame mogen niet dubbel tellen
        if (isDead) return;
        isDead = true;

        if (LevelManager.Instance != null) LevelManager.Instance.RegisterPonyKilled();

        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D c) => DealContactDamage(c.collider);
    protected virtual void OnCollisionStay2D(Collision2D c) => DealContactDamage(c.collider);
    protected virtual void OnTriggerEnter2D(Collider2D o) => DealContactDamage(o);
    protected virtual void OnTriggerStay2D(Collider2D o) => DealContactDamage(o);

    private void DealContactDamage(Collider2D other)
    {
        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        health.TakeDamage(contactDamage);
    }
}