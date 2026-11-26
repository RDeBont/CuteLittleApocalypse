using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    [Header("Omdraaien")]
    [Tooltip("Worden gespiegeld als je naar links kijkt. Leeg = de children 'Visual' en 'Weapon'.")]
    [SerializeField] private Transform[] flipTargets;
    [Tooltip("Collider van het lijf; het midden daarvan is de spiegelas, zodat de speler niet verspringt bij draaien. Leeg = de eerste BoxCollider2D in de children.")]
    [SerializeField] private BoxCollider2D bodyCollider;

    private Rigidbody2D rb;
    private float moveInput;
    private bool facingRight = true;

    private float mirrorX;              // lokale x van het midden van het lijf
    private Vector3[] startPositions;
    private Vector3[] startScales;

    public bool FacingRight => facingRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // zero friction so the player slides down walls instead of sticking
        var mat = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        rb.sharedMaterial = mat;
        foreach (var col in GetComponents<Collider2D>()) col.sharedMaterial = mat;

        SetupFlip();
    }

    private void SetupFlip()
    {
        if (flipTargets == null || flipTargets.Length == 0)
        {
            var found = new System.Collections.Generic.List<Transform>();
            Transform visual = transform.Find("Visual");
            Transform weapon = transform.Find("Weapon");
            if (visual != null) found.Add(visual);
            if (weapon != null) found.Add(weapon);
            flipTargets = found.ToArray();
        }

        if (bodyCollider == null) bodyCollider = GetComponentInChildren<BoxCollider2D>();
        mirrorX = bodyCollider != null
            ? transform.InverseTransformPoint(bodyCollider.bounds.center).x
            : 0f;

        startPositions = new Vector3[flipTargets.Length];
        startScales = new Vector3[flipTargets.Length];
        for (int i = 0; i < flipTargets.Length; i++)
        {
            if (flipTargets[i] == null) continue;
            startPositions[i] = flipTargets[i].localPosition;
            startScales[i] = flipTargets[i].localScale;
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && !facingRight) SetFacing(true);
        else if (moveInput < 0 && facingRight) SetFacing(false);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    // Spiegelt het lijf en het wapen rond het midden van de collider. Het wapen leest de
    // kijkrichting uit Visual.localScale.x, dus schieten gaat automatisch de goede kant op.
    private void SetFacing(bool right)
    {
        facingRight = right;

        for (int i = 0; i < flipTargets.Length; i++)
        {
            Transform t = flipTargets[i];
            if (t == null) continue;

            Vector3 pos = startPositions[i];
            Vector3 scale = startScales[i];
            if (!right)
            {
                pos.x = 2f * mirrorX - pos.x;
                scale.x = -scale.x;
            }
            t.localPosition = pos;
            t.localScale = scale;
        }
    }
}
