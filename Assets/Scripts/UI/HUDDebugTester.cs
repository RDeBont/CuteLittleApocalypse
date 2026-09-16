using UnityEngine;

// Tijdelijk: testen van de HUD zonder PlayerHealth/WeaponController.
// Rechtsklik op het component in de Inspector (Play Mode) en kies een actie.
[RequireComponent(typeof(HUDController))]
public class HUDDebugTester : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int maxAmmo = 14;

    private HUDController hud;
    private int health;
    private int ammo;

    private void Awake()
    {
        hud = GetComponent<HUDController>();
        health = maxHealth;
        ammo = maxAmmo;
    }

    [ContextMenu("Neem schade")]
    private void TakeDamage() => hud.SetHealth(health = Mathf.Max(0, health - 1), maxHealth);

    [ContextMenu("Heal")]
    private void Heal() => hud.SetHealth(health = Mathf.Min(maxHealth, health + 1), maxHealth);

    [ContextMenu("Schiet")]
    private void Shoot() => hud.SetAmmo(ammo = Mathf.Max(0, ammo - 1), maxAmmo);

    [ContextMenu("Herlaad")]
    private void Reload() => hud.SetAmmo(ammo = maxAmmo, maxAmmo);
}