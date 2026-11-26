using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Munitie in de HUD als rij kogeltjes: één icoon per kogel in het magazijn (vol of leeg),
/// met daarachter de reserve als "X14". Luistert naar WeaponController.OnAmmoChanged, dus geen Update().
/// Is het magazijn groter dan maxIcons, dan komt het aantal in het magazijn als getal achter de icoontjes.
/// Zet dit component op de HUD (het canvas), naast HUDController.
/// </summary>
public class AmmoBulletDisplay : MonoBehaviour
{
    [Header("Bron")]
    [Tooltip("Leeg = automatisch zoeken")]
    [SerializeField] private WeaponController weapon;

    [Header("Kogels")]
    [Tooltip("hud_ammo_bullet_5x10_0")]
    [SerializeField] private Sprite bulletFull;
    [Tooltip("hud_ammo_bullet_5x10_1")]
    [SerializeField] private Sprite bulletEmpty;
    [Tooltip("Meer kogels in het magazijn dan dit? Dan icoontjes + getal (bv. X12).")]
    [SerializeField, Min(1)] private int maxIcons = 10;
    [Tooltip("Schermpixels per art-pixel. 6.25 = zelfde schaal als de healthbar (48 px breed = 300).")]
    [SerializeField] private float pixelScale = 6.25f;
    [Tooltip("Ruimte tussen de kogels, in art-pixels")]
    [SerializeField] private int spacingPixels = 1;
    [Tooltip("Positie linksonder in canvas-eenheden (1920x1080), zelfde systeem als de HealthBar (40, 40)")]
    [SerializeField] private Vector2 position = new Vector2(40, 100);

    [Header("Getal")]
    [Tooltip("apocalypse_font_8x8_outline (PNG)")]
    [SerializeField] private Texture2D font;
    [Tooltip("1 = even groot als de rest van de pixel-UI, 0.67 = iets kleiner")]
    [SerializeField, Range(0.3f, 1.5f)] private float textSize = 0.67f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color emptyColor = new Color32(0xC9, 0x3B, 0x3B, 0xFF); // #C93B3B
    [Tooltip("Het font kent geen kleine letters of x, daarom een hoofdletter X")]
    [SerializeField] private string countPrefix = "X";
    [SerializeField] private string reloadText = "HERLADEN";

    [Header("Leeg magazijn")]
    [SerializeField] private bool blinkWhenEmpty = true;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private float blinkInterval = 0.12f;

    private readonly List<Image> icons = new List<Image>();
    private PixelText overflowText;
    private PixelText reserveText;
    private Coroutine blinkRoutine;
    private int lastMagazine = -1;
    private bool overflow;

    private void Start()
    {
        if (weapon == null) weapon = FindAnyObjectByType<WeaponController>();
        if (weapon == null)
        {
            Debug.LogWarning("AmmoBulletDisplay: geen WeaponController gevonden");
            return;
        }

        Build(weapon.MagazineSize);
        weapon.OnAmmoChanged += Refresh;
        Refresh(weapon.CurrentMagazine, weapon.ReserveAmmo, weapon.IsReloading);
    }

    private void OnDestroy()
    {
        if (weapon != null) weapon.OnAmmoChanged -= Refresh;
    }

    private void Build(int magazineSize)
    {
        var row = new GameObject("AmmoRow", typeof(RectTransform));
        row.layer = gameObject.layer;
        var rect = (RectTransform)row.transform;
        rect.SetParent(transform, false);
        rect.anchorMin = rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = position;

        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = spacingPixels * pixelScale;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        var fitter = row.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        overflow = magazineSize > maxIcons;
        int count = Mathf.Min(magazineSize, maxIcons);
        Vector2 iconSize = bulletFull != null
            ? bulletFull.rect.size * pixelScale
            : new Vector2(5, 10) * pixelScale;

        for (int i = 0; i < count; i++)
        {
            var icon = new GameObject("Kogel" + i, typeof(RectTransform), typeof(Image));
            icon.layer = row.layer;
            icon.transform.SetParent(rect, false);
            ((RectTransform)icon.transform).sizeDelta = iconSize;

            var image = icon.GetComponent<Image>();
            image.sprite = bulletFull;
            image.raycastTarget = false;
            icons.Add(image);
        }

        if (overflow) overflowText = CreateText(rect, "MagazijnTekst", 4, iconSize.y);
        reserveText = CreateText(rect, "ReserveTekst", Mathf.Max(reloadText.Length, 4) + 1, iconSize.y);
    }

    // Leeg vak in de rij met daarover een pixeltekst (breedte in letters)
    private PixelText CreateText(RectTransform parent, string name, int chars, float height)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
        go.layer = parent.gameObject.layer;
        var rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.sizeDelta = new Vector2(chars * 8f * pixelScale * textSize, height);

        if (font == null) return null;
        var text = PixelText.Attach(rect, font, PixelText.Align.Left, textColor);
        text.SizeMultiplier = textSize;
        return text;
    }

    private void Refresh(int magazine, int reserve, bool reloading)
    {
        for (int i = 0; i < icons.Count; i++)
            icons[i].sprite = i < magazine ? bulletFull : bulletEmpty;

        if (overflowText != null) overflowText.Text = countPrefix + magazine;

        if (reserveText != null)
        {
            reserveText.Text = reloading ? reloadText : countPrefix + reserve;
            reserveText.Color = !reloading && reserve == 0 ? emptyColor : textColor;
        }

        // Alleen knipperen op het moment dat het magazijn leeg raakt
        if (blinkWhenEmpty && magazine == 0 && lastMagazine > 0 && !reloading)
        {
            if (blinkRoutine != null) StopCoroutine(blinkRoutine);
            blinkRoutine = StartCoroutine(BlinkRoutine());
        }
        else if (magazine > 0 && blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
            SetIconsVisible(true);
        }

        lastMagazine = magazine;
    }

    private IEnumerator BlinkRoutine()
    {
        float end = Time.time + blinkDuration;
        bool visible = true;
        while (Time.time < end)
        {
            visible = !visible;
            SetIconsVisible(visible);
            yield return new WaitForSeconds(blinkInterval);
        }
        SetIconsVisible(true);
        blinkRoutine = null;
    }

    private void SetIconsVisible(bool visible)
    {
        foreach (Image icon in icons)
            if (icon != null) icon.enabled = visible;
    }
}
