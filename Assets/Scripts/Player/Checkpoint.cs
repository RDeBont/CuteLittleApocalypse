using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint;

    public Vector3 SpawnPosition => spawnPoint != null ? spawnPoint.position : transform.position;

    private void Reset() => GetComponent<Collider2D>().isTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawn = other.GetComponentInParent<PlayerRespawn>();
        if (respawn == null)
        {
            Debug.LogWarning("Geen PlayerRespawn op: " + other.name, other);
            return;
        }

        if (respawn.ActiveCheckpoint == this) return;

        respawn.SetCheckpoint(this);
    }

    
    public void Deactivate() { }
}