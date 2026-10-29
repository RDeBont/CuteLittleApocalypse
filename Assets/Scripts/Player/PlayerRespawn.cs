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
    private int healthSnapshot;

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
        SnapshotHealth();

        if (LevelManager.Instance != null) LevelManager.Instance.OnRetry += HandleRetry;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnRetry -= HandleRetry;
    }

    private void OnEnable() { if (health) health.OnDeath += HandleDeath; }
    private void OnDisable() { if (health) health.OnDeath -= HandleDeath; }

    public void SetCheckpoint(Checkpoint cp)
    {
        if (ActiveCheckpoint != null) ActiveCheckpoint.Deactivate();
        ActiveCheckpoint = cp;
        checkpointPos = cp.SpawnPosition;
        SnapshotAmmo();
        SnapshotHealth();

        if (LevelManager.Instance != null) LevelManager.Instance.SetCheckpoint();
    }

    private void SnapshotAmmo()
    {
        if (!weapon) return;
        magazineSnapshot = weapon.CurrentMagazine;
        reserveSnapshot = weapon.ReserveAmmo;
    }

    private void SnapshotHealth()
    {
        if (health) healthSnapshot = health.CurrentHealth;
    }

    private void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (sprite) sprite.enabled = false;
        if (weapon) weapon.enabled = false;   // niet schieten terwijl je dood bent of op een knop klikt
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        yield return new WaitForSeconds(respawnDelay);

        // Met LevelManager: game over-scherm tonen, respawnen pas na "Opnieuw proberen" (HandleRetry)
        if (LevelManager.Instance != null)
        {
            int ammoLeft = weapon ? weapon.CurrentMagazine + weapon.ReserveAmmo : 0;
            LevelManager.Instance.GameOver(ammoLeft);
            yield break;
        }

        ReviveAtCheckpoint();
    }

    private void HandleRetry()
    {
        StopAllCoroutines();
        ReviveAtCheckpoint();
    }

    private void ReviveAtCheckpoint()
    {
        Respawn();
        health.RestoreHealth(healthSnapshot);

        if (rb) rb.simulated = true;
        if (sprite) sprite.enabled = true;
        health.StartInvincibility();
        StartCoroutine(EnableWeaponNextFrame());
    }

    // Een frame later, anders telt de klik op "Opnieuw proberen" ook als schot
    private IEnumerator EnableWeaponNextFrame()
    {
        yield return null;
        if (weapon) weapon.enabled = true;
    }

    public void Respawn()
    {
        transform.position = checkpointPos;
        if (rb) rb.linearVelocity = Vector2.zero;
        if (weapon) weapon.RestoreAmmo(magazineSnapshot, reserveSnapshot);
    }
}