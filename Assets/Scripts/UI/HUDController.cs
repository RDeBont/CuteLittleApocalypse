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

    [Header("Ammo in pixelfont (optioneel)")]
    [Tooltip("apocalypse_font_8x8_solid of _outline (PNG). Leeg = gewoon de TextMeshPro-tekst.")]
    [SerializeField] private Texture2D ammoFont;
    [Tooltip("Het font heeft geen /, daarom een ander scheidingsteken.")]
    [SerializeField] private string ammoSeparator = " : ";
    [Tooltip("1 = even groot als de rest van de pixel-UI, 0.67 = iets kleiner")]
    [SerializeField, Range(0.3f, 1.5f)] private float ammoTextSize = 0.67f;

    private PixelText ammoPixelText;

    private int lastMagazine = -1;
    private int lastReserve = -1;
    private bool lastReloading;
    PlayerHealth health;

    private void Start()
    {
        // Pixelfont over de bestaande tekst leggen; de TextMeshPro-tekst zelf gaat onzichtbaar
        if (ammoFont != null && ammoText != null)
        {
            ammoPixelText = PixelText.Attach(ammoText.rectTransform, ammoFont, PixelText.Align.Left, Color.white);
            ammoPixelText.SizeMultiplier = ammoTextSize;
            ammoText.enabled = false;
        }

        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (weapon == null)
            weapon = FindAnyObjectByType<WeaponController>();

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

        if (weapon.CurrentMagazine != lastMagazine || weapon.ReserveAmmo != lastReserve || weapon.IsReloading != lastReloading)
        {
            lastMagazine = weapon.CurrentMagazine;
            lastReserve = weapon.ReserveAmmo;
            lastReloading = weapon.IsReloading;
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

        if (ammoPixelText != null)
        {
            // Font is al oranje, dus alleen leeg magazijn rood kleuren
            bool reloading = weapon != null && weapon.IsReloading;
            ammoPixelText.Text = reloading ? "HERLADEN..." : $"{current:D2}{ammoSeparator}{max:D2}";
            ammoPixelText.Color = current == 0 && !reloading ? ammoEmptyColor : Color.white;
        }
    }
}