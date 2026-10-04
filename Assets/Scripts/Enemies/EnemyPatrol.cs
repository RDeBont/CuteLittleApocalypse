using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private LayerMask groundLayer;

    private bool movingRight = true;
    private bool turning = false;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        float direction = movingRight ? 1f : -1f;

        // Check of er grond onder de voorkant van de pony is
        RaycastHit2D groundCheck = Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            0.6f,
            groundLayer
        );

        // Geen grond = dakrand
        if (groundCheck.collider == null && !turning)
        {
            TurnAround();
            return;
        }

        // Beweeg
        transform.Translate(
            Vector2.right * direction * moveSpeed * Time.deltaTime
        );
    }

    private void TurnAround()
    {
        turning = true;

        movingRight = !movingRight;

        // Sprite + EdgeCheck omdraaien
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        Invoke(nameof(AllowTurning), 0.2f);
    }

    private void AllowTurning()
    {
        turning = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("PONY TRIGGER: " + other.gameObject.name);

        PlayerHealth playerHealth = other.transform.root.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.Log("GEEN PlayerHealth gevonden!");
            return;
        }

        Debug.Log("PONY HIT PLAYER!");
        playerHealth.TakeDamage(1);
    }
}