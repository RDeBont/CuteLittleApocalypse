using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    public int damage = 1;

    void Reset() => GetComponent<Collider2D>().isTrigger = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        
        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) { ; return; }

        health.TakeDamage(damage);

        if (health.CurrentHealth > 0)
        {
            var respawn = health.GetComponentInParent<PlayerRespawn>();
            if (respawn == null) Debug.LogWarning("Geen PlayerRespawn gevonden");
            else respawn.Respawn();
        }
    }
}