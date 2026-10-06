using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint; 

    [Header("Lichtbaken")]
    [SerializeField] private SpriteRenderer beacon;
    [SerializeField] private GameObject glow; 
    [SerializeField] private Color inactiveColor = Color.red;
    [SerializeField] private Color activeColor = Color.green;

    public Vector3 SpawnPosition => spawnPoint != null ? spawnPoint.position : transform.position;

    private void Reset() => GetComponent<Collider2D>().isTrigger = true;

    private void Start() => SetState(false);

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Checkpoint geraakt door: " + other.name);

        var respawn = other.GetComponentInParent<PlayerRespawn>();
        if (respawn == null || respawn.ActiveCheckpoint == this) return;

        respawn.SetCheckpoint(this);
        SetState(true);
    }

    public void Deactivate() => SetState(false);

    private void SetState(bool active)
    {
        if (beacon) beacon.color = active ? activeColor : inactiveColor;
        if (glow) glow.SetActive(active);
    }
}