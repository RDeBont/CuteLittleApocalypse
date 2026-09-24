using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask wallLayers;

    private Rigidbody2D rb;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction)
    {
        direction = direction.normalized;
        rb.linearVelocity = direction * speed;
        transform.right = direction;

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            Hit();
            target.TakeDamage(damage);
            return;
        }

        if (IsInLayerMask(other.gameObject.layer, wallLayers))
        {
            Hit();
        }
    }

    private void Hit()
    {
        hasHit = true;
        Destroy(gameObject);
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}