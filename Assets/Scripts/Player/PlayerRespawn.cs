using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 1f;

    private PlayerHealth health;
    private WeaponController weapon;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private Vector3 checkpointPos;
    private int magazineSnapshot;
    private int reserveSnapshot;

    public Checkpoint ActiveCheckpoint { get; private set; }
    public Vector3 RespawnPosition => checkpointPos;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        weapon = GetComponent<WeaponController>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
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
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (sprite) sprite.enabled = false;
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        yield return new WaitForSeconds(respawnDelay);

        Respawn();
        health.ResetHealth();

        if (rb) rb.simulated = true;
        if (sprite) sprite.enabled = true;
        health.StartInvincibility();
    }

    public void Respawn()
    {
        transform.position = checkpointPos;
        if (rb) rb.linearVelocity = Vector2.zero;
        if (weapon) weapon.RestoreAmmo(magazineSnapshot, reserveSnapshot);
    }
}