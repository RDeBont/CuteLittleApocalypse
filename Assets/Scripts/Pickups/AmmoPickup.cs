using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 7;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Collider kan op een child zitten, dus pak het object met de Rigidbody2D
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null || !rb.CompareTag("Player")) return;

        WeaponController weapon = rb.GetComponent<WeaponController>();
        if (weapon == null) return;

        if (!weapon.AddReserveAmmo(ammoAmount)) return; // vol, laten liggen

        Destroy(gameObject);
    }
}