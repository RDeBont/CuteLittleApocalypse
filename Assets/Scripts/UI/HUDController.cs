using System.Collections;
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
    [Tooltip("Oude cijferweergave. Staat uit zolang er een AmmoBulletDisplay op de HUD staat, tenzij Show Old Ammo Text aan staat.")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private bool showOldAmmoText = false;
    [SerializeField] private Color ammoColor = new Color32(0xF2, 0xB1, 0x38, 0xFF);      // #F2B138
    [SerializeField] private Color ammoEmptyColor = new Color32(0xC9, 0x3B, 0x3B, 0xFF); // #C93B3B

    [Header("Ammo in pixelfont (optioneel)")]
    [Tooltip("apocalypse_font_8x8_solid of _outline (PNG). Leeg = gewoon de TextMeshPro-tekst.")]
    [SerializeField] private Texture2D ammoFont;
    [Tooltip("Het font heeft geen /, daarom een ander scheidingsteken.")]
    [SerializeField] private string ammoSeparator = " : ";
    [Tooltip("1 = even groot als de rest van de pixel-UI, 0.67 = iets kleiner")]
    [SerializeField, Range(0.3f, 1.5f)] private float ammoTextSize = 0.67f;

    [Header("Popup (bv. CHECKPOINT!)")]
    [Tooltip("Leeg = hetzelfde font als de ammo")]
    [SerializeField] private Texture2D popupFont;
    [Tooltip("Plek in canvas-pixels (320x180), vanaf linksboven")]
    [SerializeField] private Rect popupRect = new Rect(0, 40, 320, 10);
    [SerializeField] private Color popupColor = Color.white;
    [SerializeField] private float popupDuration = 1.5f;
    [Tooltip("Aantal keer knipperen aan het eind")]
    [SerializeField] private int popupBlinks = 3;

    private PixelText ammoPixelText;
    private PixelText popupPixelText;
    private Coroutine popupRoutine;

    private bool useOldAmmoText = true;
    PlayerHealth health;

    private void Start()
    {
        // Nieuwe kogelrij aanwezig? Dan de oude cijfers verbergen
        var bullets = GetComponent<AmmoBulletDisplay>();
        useOldAmmoText = showOldAmmoText || bullets == null || !bullets.enabled;
        if (!useOldAmmoText && ammoText != null) ammoText.enabled = false;

        // Pixelfont over de bestaande tekst leggen; de TextMeshPro-tekst zelf gaat onzichtbaar
        if (useOldAmmoText && ammoFont != null && ammoText != null)
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

        if (weapon != null && useOldAmmoText)
        {
            weapon.OnAmmoChanged += HandleAmmoChanged;
            SetAmmo(weapon.CurrentMagazine, weapon.ReserveAmmo);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= SetHealth;
        if (weapon != null)
            weapon.OnAmmoChanged -= HandleAmmoChanged;
    }

    private void HandleAmmoChanged(int magazine, int reserve, bool reloading) => SetAmmo(magazine, reserve);

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
        if (ammoText == null || !useOldAmmoText) return;

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

    // Korte melding midden in beeld, in het pixelfont van de HUD
    public void ShowPopup(string message)
    {
        Texture2D font = popupFont != null ? popupFont : ammoFont;
        if (font == null) return;

        if (popupPixelText == null)
            popupPixelText = PixelText.Create(transform, "PopupText", popupRect, font, PixelText.Align.Center, popupColor, true);

        if (popupRoutine != null) StopCoroutine(popupRoutine);
        popupRoutine = StartCoroutine(PopupRoutine(message));
    }

    private IEnumerator PopupRoutine(string message)
    {
        popupPixelText.gameObject.SetActive(true);
        popupPixelText.Text = message;

        float blinkTime = 0.1f;
        yield return new WaitForSeconds(Mathf.Max(0f, popupDuration - popupBlinks * blinkTime * 2f));

        for (int i = 0; i < popupBlinks; i++)
        {
            popupPixelText.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkTime);
            popupPixelText.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkTime);
        }

        popupPixelText.gameObject.SetActive(false);
        popupRoutine = null;
    }
}
