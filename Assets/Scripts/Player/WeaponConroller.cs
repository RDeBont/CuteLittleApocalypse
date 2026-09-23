using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class WeaponController : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int magazineSize = 7;
    [SerializeField] private int startReserveAmmo = 14;
    [SerializeField] private int maxReserveAmmo = 21;

    [Header("Reload")]
    [SerializeField] private float reloadTime = 1.2f;

    [Header("Weapon")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float maxRange = 100f;
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("UI")]
    [SerializeField] private TMP_Text ammoText;

    private int currentMagazine;
    private int reserveAmmo;

    private bool isReloading;

    private Camera mainCamera;

    private void Awake()
    {
        currentMagazine = magazineSize;
        reserveAmmo = Mathf.Clamp(
            startReserveAmmo,
            0,
            maxReserveAmmo
        );

        mainCamera = Camera.main;

        UpdateAmmoUI();
    }

    private void Update()
    {
        // Linkermuisknop
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryFire();
        }

        // R
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            TryReload();
        }
    }

    private void TryFire()
    {
        // Tijdens reload niet schieten
        if (isReloading)
            return;

        // Geen kogels
        if (currentMagazine <= 0)
        {
            TryReload();
            return;
        }

        Fire();
    }

    private void Fire()
    {
        // Eén kogel gebruiken
        currentMagazine--;

        UpdateAmmoUI();

        // Muzzle positie
        Vector2 origin = muzzlePoint.position;

        // Richting naar muis
        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

        Vector2 direction =
            ((Vector2)mouseWorldPosition - origin).normalized;

        // 2D Raycast
        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            maxRange,
            hitMask
        );

        if (hit.collider != null)
        {
            HandleHit(hit);
        }

        // Debug ray in Scene view
        Debug.DrawRay(
            origin,
            direction * maxRange,
            Color.red,
            0.5f
        );
    }

    private void HandleHit(RaycastHit2D hit)
    {
        Debug.Log("Hit: " + hit.collider.name);

        // Hier kun je later damage toevoegen.
        //
        // Bijvoorbeeld:
        //
        // Enemy enemy = hit.collider.GetComponent<Enemy>();
        // if (enemy != null)
        // {
        //     enemy.TakeDamage(25);
        // }
    }

    private void TryReload()
    {
        // Al aan het reloaden
        if (isReloading)
            return;

        // Magazijn al vol
        if (currentMagazine >= magazineSize)
            return;

        // Geen reserve ammo
        if (reserveAmmo <= 0)
            return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        UpdateAmmoUI();

        // Wacht 1.2 seconden
        yield return new WaitForSeconds(reloadTime);

        // Hoeveel kogels missen we?
        int neededAmmo =
            magazineSize - currentMagazine;

        // Hoeveel kunnen we daadwerkelijk laden?
        int ammoToLoad =
            Mathf.Min(
                neededAmmo,
                reserveAmmo
            );

        // Ammo verplaatsen
        currentMagazine += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;

        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText == null)
            return;

        if (isReloading)
        {
            ammoText.text = "RELOADING...";
        }
        else
        {
            ammoText.text =
                currentMagazine +
                " / " +
                reserveAmmo;
        }
    }

    public bool AddReserveAmmo(int amount)
    {
        if (reserveAmmo >= maxReserveAmmo)
            return false;

        reserveAmmo = Mathf.Clamp(
            reserveAmmo + amount,
            0,
            maxReserveAmmo
        );

        Debug.Log("Reserve ammo: " + reserveAmmo);

        UpdateAmmoUI();
        return true;
    }

    public int CurrentMagazine
    {
        get { return currentMagazine; }
    }

    public int ReserveAmmo
    {
        get { return reserveAmmo; }
    }

    public bool IsReloading
    {
        get { return isReloading; }
    }
}