using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trigger op het laatste dak: opent de Chinook, laat de speler instappen
/// en meldt daarna dat het level voltooid is.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private Chinook chinook;

    [Tooltip("Tijd tussen het openen van de laadklep en het instappen van de speler")]
    [SerializeField] private float boardingDelay = 0.6f;

    [Tooltip("Tijd tussen het instappen en het melden dat het level voltooid is")]
    [SerializeField] private float completeDelay = 0.8f;

    // LevelManager koppelt hier later aan, zodat dit script niets van UI of scenes hoeft te weten
    [SerializeField] private UnityEvent onLevelComplete;

    private bool isTriggered;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered)
        {
            return;
        }

        Rigidbody2D body = other.attachedRigidbody;
        if (body == null || !body.CompareTag("Player"))
        {
            return;
        }

        isTriggered = true;
        StartCoroutine(FinishLevel(body));
    }

    private IEnumerator FinishLevel(Rigidbody2D player)
    {
        FreezePlayer(player);

        if (chinook != null)
        {
            chinook.OpenHatch();
        }
        else
        {
            Debug.LogWarning("LevelEndTrigger: geen Chinook gekoppeld");
        }

        yield return new WaitForSeconds(boardingDelay);
        HidePlayer(player.gameObject);

        yield return new WaitForSeconds(completeDelay);
        onLevelComplete.Invoke();
    }

    private void FreezePlayer(Rigidbody2D player)
    {
        player.linearVelocity = Vector2.zero;

        // Tijdens het instappen mag de speler niet meer weglopen, springen of schieten
        DisableComponent<PlayerMovement>(player.gameObject);
        DisableComponent<JumpController>(player.gameObject);
        DisableComponent<WeaponController>(player.gameObject);
    }

    // Alleen de sprites uit, niet het hele object: camera en HUD verwijzen nog naar de speler
    private static void HidePlayer(GameObject player)
    {
        foreach (SpriteRenderer spriteRenderer in player.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.enabled = false;
        }
    }

    private static void DisableComponent<T>(GameObject target) where T : Behaviour
    {
        T component = target.GetComponentInChildren<T>();
        if (component != null)
        {
            component.enabled = false;
        }
    }
}