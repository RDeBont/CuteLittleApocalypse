using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponController weapon;

    [Header("Health")]
    [SerializeField] private Image healthBarImage;
    [Tooltip("Op volgorde: hp_0 (leeg), hp_1, hp_2, hp_3 (vol)")]
    [SerializeField] private Sprite[] healthFrames = new Sprite[4];

    [Header("Ammo")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Color ammoColor = new Color32(0xF2, 0xB1, 0x38, 0xFF);      // #F2B138
    [SerializeField] private Color ammoEmptyColor = new Color32(0xC9, 0x3B, 0x3B, 0xFF); // #C93B3B

    private int lastMagazine = -1;
    private int lastReserve = -1;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (weapon == null)
            weapon = FindFirstObjectByType<WeaponController>();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += SetHealth;
            SetHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
        else
        {
            SetHealth(3, 3);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= SetHealth;
    }

    private void Update()
    {
        // Ammo automatisch bijwerken zonder WeaponController aan te passen
        if (weapon == null) return;

        if (weapon.CurrentMagazine != lastMagazine || weapon.ReserveAmmo != lastReserve)
        {
            lastMagazine = weapon.CurrentMagazine;
            lastReserve = weapon.ReserveAmmo;
            SetAmmo(lastMagazine, lastReserve);
        }
    }

    public void SetHealth(int current, int max)
    {
        if (healthBarImage == null || healthFrames == null || healthFrames.Length == 0) return;

        int lastFrame = healthFrames.Length - 1;
        float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
        int index = Mathf.CeilToInt(ratio * lastFrame);

        if (healthFrames[index] != null)
            healthBarImage.sprite = healthFrames[index];
    }

    public void SetAmmo(int current, int max)
    {
        if (ammoText == null) return;

        current = Mathf.Max(0, current);
        ammoText.text = $"{current:D2} / {max:D2}";
        ammoText.color = current == 0 ? ammoEmptyColor : ammoColor;
    }
}