using UnityEngine;

public class SpelerAnimatie : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private JumpController springBesturing;
    private WeaponController wapen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        springBesturing = GetComponent<JumpController>();
        wapen = GetComponent<WeaponController>();
    }

    private void Update()
    {
        animator.SetFloat("Snelheid", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("VerticaleSnelheid", rb.linearVelocity.y);
        animator.SetBool("OpGrond", springBesturing.IsGrounded);
        animator.SetBool("Herlaadt", wapen.IsReloading);
    }

    public void SpeelSchietAnimatie()
    {
        animator.SetTrigger("Schiet");
    }
}