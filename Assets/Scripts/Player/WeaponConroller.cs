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
    [SerializeField] private Projectile projectilePrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text ammoText;

    private int currentMagazine;
    private int reserveAmmo;
    private bool isReloading;
    private Camera mainCamera;

    private void Awake()
    {
        currentMagazine = magazineSize;
        reserveAmmo = Mathf.Clamp(startReserveAmmo, 0, maxReserveAmmo);
        mainCamera = Camera.main;
        UpdateAmmoUI();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryFire();
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            TryReload();
        }
    }

    private void TryFire()
    {
        if (isReloading)
            return;

        if (currentMagazine <= 0)
        {
            TryReload();
            return;
        }

        Fire();
    }

    private void Fire()
    {
        if (projectilePrefab == null || muzzlePoint == null)
        {
            Debug.LogWarning("WeaponController: projectilePrefab of muzzlePoint niet ingesteld.");
            return;
        }

        currentMagazine--;
        UpdateAmmoUI();

        Vector2 origin = muzzlePoint.position;
        Vector2 direction = GetAimDirection(origin);

        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.Launch(direction);
    }

    private Vector2 GetAimDirection(Vector2 origin)
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (Vector2)mouseWorldPosition - origin;

        // Muis precies op de loop: schiet naar rechts i.p.v. richting (0,0)
        if (direction.sqrMagnitude < 0.0001f)
            return Vector2.right;

        return direction.normalized;
    }

    private void TryReload()
    {
        if (isReloading)
            return;

        if (currentMagazine >= magazineSize)
            return;

        if (reserveAmmo <= 0)
            return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        UpdateAmmoUI();

        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = magazineSize - currentMagazine;
        int ammoToLoad = Mathf.Min(neededAmmo, reserveAmmo);

        currentMagazine += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText == null)
            return;

        ammoText.text = isReloading
            ? "RELOADING..."
            : currentMagazine + " / " + reserveAmmo;
    }

    public bool AddReserveAmmo(int amount)
    {
        if (reserveAmmo >= maxReserveAmmo)
            return false;

        reserveAmmo = Mathf.Clamp(reserveAmmo + amount, 0, maxReserveAmmo);
        Debug.Log("Reserve ammo: " + reserveAmmo);
        UpdateAmmoUI();
        return true;
    }

    public int CurrentMagazine => currentMagazine;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;
}