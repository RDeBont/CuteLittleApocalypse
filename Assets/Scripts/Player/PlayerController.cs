using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public SpriteRenderer sr;

    private Rigidbody2D rb;
    private float moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        // zero friction so the player slides down walls instead of sticking
        var mat = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        rb.sharedMaterial = mat;
        foreach (var col in GetComponents<Collider2D>()) col.sharedMaterial = mat;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0) sr.flipX = false;
        else if (moveInput < 0) sr.flipX = true;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}