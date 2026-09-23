using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public Transform visual; 

    private Rigidbody2D rb;
    private float moveInput;
    private int facingDir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && facingDir != 1)
        {
            facingDir = 1;
            Flip();
        }
        else if (moveInput < 0 && facingDir != -1)
        {
            facingDir = -1;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 s = visual.localScale;
        s.x = Mathf.Abs(s.x) * facingDir;
        visual.localScale = s;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
}