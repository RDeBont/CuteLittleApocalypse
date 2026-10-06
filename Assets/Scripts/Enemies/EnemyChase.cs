using UnityEngine;

public class EnemyChase : EnemyBase
{
    [Header("Chase Settings")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Detection Indicator")]
    [SerializeField] private GameObject detectionIndicator;
    [SerializeField] private float indicatorDuration = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float direction = 1f;

    private bool playerDetected = false;
    private bool warningActive = false;
    private float warningTimer = 0f;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' was not found!");
        }

        if (detectionIndicator != null)
        {
            detectionIndicator.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null || edgeCheck == null)
        {
            StopMoving();
            return;
        }

        float horizontalDistance = player.position.x - transform.position.x;

        if (Mathf.Abs(horizontalDistance) > detectionRange)
        {
            playerDetected = false;
            warningActive = false;
            warningTimer = 0f;

            if (detectionIndicator != null)
            {
                detectionIndicator.SetActive(false);
            }

            StopMoving();
            return;
        }

        if (!playerDetected)
        {
            playerDetected = true;
            warningActive = true;
            warningTimer = indicatorDuration;

            if (detectionIndicator != null)
            {
                detectionIndicator.SetActive(true);
            }

            StopMoving();
            return;
        }

        if (warningActive)
        {
            warningTimer -= Time.fixedDeltaTime;

            StopMoving();

            if (warningTimer <= 0f)
            {
                warningActive = false;

                if (detectionIndicator != null)
                {
                    detectionIndicator.SetActive(false);
                }
            }

            return;
        }

        if (horizontalDistance > 0.05f)
        {
            direction = 1f;
        }
        else if (horizontalDistance < -0.05f)
        {
            direction = -1f;
        }
        else
        {
            StopMoving();
            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0f;
        }

        Vector3 localPosition = edgeCheck.localPosition;
        localPosition.x = Mathf.Abs(localPosition.x) * direction;
        edgeCheck.localPosition = localPosition;

        RaycastHit2D ground = Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (ground.collider == null)
        {
            StopMoving();
            return;
        }

        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayerContact(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckPlayerContact(collision.collider);
    }

    private void CheckPlayerContact(Collider2D other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Player hit by Purple Pony");
        }
    }
}