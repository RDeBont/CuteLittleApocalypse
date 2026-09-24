using UnityEngine;

public class EnemyPatrol : EnemyBase
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;

    private Vector3 startPosition;
    private bool movingRight = true;

    private void Start()
    {
        base.Start();
        startPosition = transform.position;
    }

    private void Update()
    {
        float direction = movingRight ? 1f : -1f;

        transform.Translate(
            Vector2.right * direction * moveSpeed * Time.deltaTime
        );

        float distanceFromStart = transform.position.x - startPosition.x;

        if (movingRight && distanceFromStart >= patrolDistance)
        {
            TurnAround();
        }
        else if (!movingRight && distanceFromStart <= -patrolDistance)
        {
            TurnAround();
        }
    }

    private void TurnAround()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Pony damaged player!");
        }
    }
}