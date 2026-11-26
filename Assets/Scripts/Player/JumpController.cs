
using UnityEngine;

public class JumpController : MonoBehaviour
{
    [Header("Sprong")]
    [Tooltip("Hoogte (units) als je de springknop ingedrukt houdt. 1 tegel = 1 unit.")]
    [SerializeField] private float jumpHeight = 2.6f;
    [Tooltip("Tijd (s) tot het hoogste punt. Korter = strakker, langer = zweveriger.")]
    [SerializeField] private float timeToApex = 0.4f;
    [Tooltip("Loslaten tijdens het stijgen: snelheid omhoog x deze waarde. 0.5 = korte tik geeft ongeveer een kwart van de hoogte.")]
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;

    [Header("Vallen")]
    [Tooltip("Zwaartekracht bij vallen = zwaartekracht bij stijgen x deze waarde. Boven 1.5 wordt het gat van 4 tegels in Level_01 (x 148-151) krap.")]
    [SerializeField] private float fallMultiplier = 1.5f;
    [Tooltip("Maximale valsnelheid (units/s).")]
    [SerializeField] private float maxFallSpeed = 18f;

    [Header("Vergevingsgezind")]
    [Tooltip("Hoe lang (s) je na het verlaten van een rand nog mag springen.")]
    [SerializeField] private float coyoteTime = 0.1f;
    [Tooltip("Hoe lang (s) een te vroege druk op springen onthouden wordt tot je landt.")]
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Ground Detection")]
    [Tooltip("Collider van het lijf. Leeg = de eerste BoxCollider2D in de children.")]
    [SerializeField] private BoxCollider2D bodyCollider;
    [Tooltip("Alleen gebruikt als er geen bodyCollider is: midden van de voeten.")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Breedte van de check t.o.v. het lijf. Kleiner dan 1, zodat muren naast je niet als grond tellen.")]
    [SerializeField, Range(0.3f, 1f)] private float groundCheckWidth = 0.9f;
    [Tooltip("Hoe ver (units) onder de voeten gezocht wordt.")]
    [SerializeField] private float groundCheckDepth = 0.06f;

    [Header("Input")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private bool jumpHeld;
    private bool jumpReleased;
    private float lastJumpPressedTime = float.NegativeInfinity;
    private float lastGroundedTime = float.NegativeInfinity;

    private float riseGravityScale;
    private float jumpVelocity;

    public bool IsGrounded => isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (bodyCollider == null) bodyCollider = GetComponentInChildren<BoxCollider2D>();
        if (bodyCollider == null && groundCheck == null)
            Debug.LogError("JumpController: geen bodyCollider en geen groundCheck gevonden!");

        CalculateJump();
    }

    void OnValidate()
    {
        timeToApex = Mathf.Max(0.05f, timeToApex);
        jumpHeight = Mathf.Max(0.1f, jumpHeight);
        fallMultiplier = Mathf.Max(1f, fallMultiplier);
        if (rb != null) CalculateJump();
    }

    void OnDisable()
    {
        if (rb != null) rb.gravityScale = riseGravityScale;
    }

    // Zwaartekracht en springsnelheid uit hoogte en tijd: g = 2h / t², v = g * t
    private void CalculateJump()
    {
        float gravity = 2f * jumpHeight / (timeToApex * timeToApex);
        jumpVelocity = gravity * timeToApex;

        float worldGravity = Mathf.Abs(Physics2D.gravity.y);
        riseGravityScale = worldGravity > 0f ? gravity / worldGravity : 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(jumpKey)) lastJumpPressedTime = Time.time;
        if (Input.GetKeyUp(jumpKey)) jumpReleased = true;
        jumpHeld = Input.GetKey(jumpKey);
    }

    void FixedUpdate()
    {
        float now = Time.time;
        Vector2 velocity = rb.linearVelocity;

        isGrounded = velocity.y <= 0.01f && CheckGround();
        if (isGrounded)
        {
            lastGroundedTime = now;
            isJumping = false;
        }

        bool buffered = now - lastJumpPressedTime <= jumpBufferTime;
        bool coyote = now - lastGroundedTime <= coyoteTime;

        if (buffered && coyote && !isJumping)
        {
            velocity.y = jumpVelocity;
            isJumping = true;

            // Springgeluid afspelen
            AudioManager.Instance?.PlaySFX(jumpSound);

            isGrounded = false;
            lastJumpPressedTime = float.NegativeInfinity;
            lastGroundedTime = float.NegativeInfinity;

            if (!jumpHeld) jumpReleased = true;
        }

        if (jumpReleased)
        {
            if (isJumping && velocity.y > 0f) velocity.y *= jumpCutMultiplier;
            jumpReleased = false;
        }

        rb.gravityScale = velocity.y < 0f ? riseGravityScale * fallMultiplier : riseGravityScale;
        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        rb.linearVelocity = velocity;
    }

    private bool CheckGround()
    {
        GetGroundBox(out Vector2 center, out Vector2 size);
        return Physics2D.OverlapBox(center, size, 0f, groundLayer) != null;
    }

    private void GetGroundBox(out Vector2 center, out Vector2 size)
    {
        if (bodyCollider != null)
        {
            Bounds b = bodyCollider.bounds;
            size = new Vector2(b.size.x * groundCheckWidth, groundCheckDepth);
            center = new Vector2(b.center.x, b.min.y - groundCheckDepth * 0.5f);
        }
        else
        {
            Vector2 feet = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
            size = new Vector2(0.7f * groundCheckWidth, groundCheckDepth);
            center = feet + Vector2.down * (groundCheckDepth * 0.5f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (bodyCollider == null) bodyCollider = GetComponentInChildren<BoxCollider2D>();
        GetGroundBox(out Vector2 center, out Vector2 size);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
