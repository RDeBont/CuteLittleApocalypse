using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint;

    [Header("Vlag")]
    [Tooltip("Animator op de vlag (CheckpointAnimator: Inactive -> Raising -> Active)")]
    [SerializeField] private Animator flagAnimator;
    [Tooltip("Optioneel: gaat aan zodra de checkpoint actief is (bv. de Light2D)")]
    [SerializeField] private GameObject activeLight;

    [Header("Feedback")]
    [Tooltip("Tekst in de HUD bij activeren. Het pixelfont kent alleen hoofdletters, cijfers en ! , . : ?")]
    [SerializeField] private string popupText = "CHECKPOINT!";
    [Tooltip("Optioneel: speelt af bij activeren. Leeg = geen geluid.")]
    [SerializeField] private AudioSource activateSound;

    private static readonly int ActivateHash = Animator.StringToHash("Activate");

    private bool activated;

    public bool IsActivated => activated;
    public Vector3 SpawnPosition => spawnPoint != null ? spawnPoint.position : transform.position;

    private void Reset() => GetComponent<Collider2D>().isTrigger = true;

    private void Awake()
    {
        if (activeLight != null) activeLight.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Elke checkpoint activeert maar één keer; terugrennen naar een oude zet de respawn niet terug
        if (activated) return;

        var respawn = other.GetComponentInParent<PlayerRespawn>();
        if (respawn == null) return;

        activated = true;
        respawn.SetCheckpoint(this);
        PlayFeedback();
    }

    private void PlayFeedback()
    {
        if (flagAnimator != null) flagAnimator.SetTrigger(ActivateHash);
        if (activeLight != null) activeLight.SetActive(true);
        if (activateSound != null) activateSound.Play();

        var hud = FindAnyObjectByType<HUDController>();
        if (hud != null) hud.ShowPopup(popupText);
    }

    // Wordt aangeroepen als een nieuwere checkpoint het overneemt. De vlag blijft bewust omhoog.
    public void Deactivate() { }
}
