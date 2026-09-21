using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = 0f;
        if (Keyboard.current.aKey.isPressed) moveInput = -1f;
        if (Keyboard.current.dKey.isPressed) moveInput = 1f;

        if (moveInput != 0)
            sr.flipX = moveInput < 0;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}