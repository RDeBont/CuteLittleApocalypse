using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private PlayerHealth health;
    private WeaponController weapon;
    private Rigidbody2D rb;

    private Vector3 checkpointPos;
    private int magazineSnapshot;
    private int reserveSnapshot;

    public Checkpoint ActiveCheckpoint { get; private set; }

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        weapon = GetComponent<WeaponController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
        checkpointPos = transform.position;
        SnapshotAmmo();
    }

    private void OnEnable() { if (health) health.OnDeath += HandleDeath; }
    private void OnDisable() { if (health) health.OnDeath -= HandleDeath; }

    public void SetCheckpoint(Checkpoint cp)
    {
        if (ActiveCheckpoint != null) ActiveCheckpoint.Deactivate();
        ActiveCheckpoint = cp;
        checkpointPos = cp.SpawnPosition;
        SnapshotAmmo();
    }

    private void SnapshotAmmo()
    {
        if (!weapon) return;
        magazineSnapshot = weapon.CurrentMagazine;
        reserveSnapshot = weapon.ReserveAmmo;
    }

    private void HandleDeath()
    {
        Respawn();
        health.ResetHealth();
    }

    public void Respawn()
    {
        transform.position = checkpointPos;
        if (rb) rb.linearVelocity = Vector2.zero;
        if (weapon) weapon.RestoreAmmo(magazineSnapshot, reserveSnapshot);
    }
}