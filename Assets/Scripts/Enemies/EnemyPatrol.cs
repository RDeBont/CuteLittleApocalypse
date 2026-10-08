using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Edge Check")]
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float edgeCheckDistance = 0.8f;

    [Header("Wall Check")]
    [SerializeField] private float wallCheckDistance = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;

    private float direction = 1f;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (rb == null || edgeCheck == null || enemyCollider == null)
        {
            return;
        }

        Bounds bounds = enemyCollider.bounds;

        Vector2 wallOrigin = new Vector2(
            bounds.center.x + direction * (bounds.extents.x + 0.02f),
            bounds.center.y
        );

        Vector2 wallSize = new Vector2(
            wallCheckDistance,
            bounds.size.y * 0.6f
        );

        RaycastHit2D wall = Physics2D.BoxCast(
            wallOrigin,
            wallSize,
            0f,
            Vector2.right * direction,
            wallCheckDistance,
            groundLayer
        );

        if (wall.collider != null)
        {
            TurnAround();
            return;
        }

        RaycastHit2D ground = Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        if (ground.collider == null)
        {
            TurnAround();
            return;
        }

        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void TurnAround()
    {
        direction *= -1f;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0f;
        }

        Vector3 localPosition = edgeCheck.localPosition;
        localPosition.x = Mathf.Abs(localPosition.x) * direction;
        edgeCheck.localPosition = localPosition;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Player hit by Pink Pony");
        }
    }
}