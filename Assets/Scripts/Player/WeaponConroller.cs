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
    [Tooltip("Niet-flippend referentiepunt (bv. de player root) waar de offset vandaan gemeten wordt.")]
    [SerializeField] private Transform muzzleAnchor;
    [Tooltip("Offset t.o.v. muzzleAnchor als je naar RECHTS kijkt. X wordt automatisch omgedraaid als je naar links kijkt; Y blijft altijd gelijk.")]
    [SerializeField] private Vector2 muzzleOffset = new Vector2(0.5f, 0f);
    [SerializeField] private Projectile projectilePrefab;

    [Header("Aiming")]
    [Tooltip("Transform waarvan localScale.x de kijkrichting bepaalt (positief = rechts, negatief = links). Leeg = deze transform.")]
    [SerializeField] private Transform facingReference;

    [Header("UI")]
    [SerializeField] private TMP_Text ammoText;

    private int currentMagazine;
    private int reserveAmmo;
    private bool isReloading;
    private bool facingRight = true;

    private void Awake()
    {
        currentMagazine = magazineSize;
        reserveAmmo = Mathf.Clamp(startReserveAmmo, 0, maxReserveAmmo);

        if (facingReference == null)
            facingReference = transform;

        UpdateAmmoUI();
    }

    private void Update()
    {
        UpdateFacing();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryFire();
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            TryReload();
        }
    }

    private void UpdateFacing()
    {
        float scaleX = facingReference.localScale.x;
        if (Mathf.Abs(scaleX) > 0.0001f)
            facingRight = scaleX > 0f;
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
        if (projectilePrefab == null || muzzleAnchor == null)
        {
            Debug.LogWarning("WeaponController: projectilePrefab of muzzleAnchor niet ingesteld.");
            return;
        }

        currentMagazine--;
        UpdateAmmoUI();

        Vector2 origin = GetMuzzleOrigin();
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.Launch(direction);
    }

    private Vector2 GetMuzzleOrigin()
    {
        float xOffset = facingRight ? muzzleOffset.x : -muzzleOffset.x;
        return (Vector2)muzzleAnchor.position + new Vector2(xOffset, muzzleOffset.y);
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