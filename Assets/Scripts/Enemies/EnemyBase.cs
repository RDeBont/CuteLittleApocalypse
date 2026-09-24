using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 1;

    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage: " + damage + " (health left: " + currentHealth + ")");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}