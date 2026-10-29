using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Instellingenpaneel op basis van de afbeelding ui_options (320x180).
/// De labels en knoppen zitten in de afbeelding; dit script legt er een echte muziekslider
/// en onzichtbare klikvlakken voor STANDAARD en TERUG overheen. Waardes gaan via SaveManager.
/// Wordt in code opgebouwd met SettingsMenu.Create, er hoeft niets in de scene te staan.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    private const float ImageWidth = 320f;
    private const float ImageHeight = 180f;
    private const float DefaultVolume = 0.8f;

    // Kleuren overgenomen uit ui_options
    private static readonly Color RowColor = new Color32(69, 76, 97, 255);
    private static readonly Color BorderColor = new Color32(18, 20, 31, 255);
    private static readonly Color TrackColor = new Color32(30, 34, 48, 255);
    private static readonly Color FillColor = new Color32(242, 177, 56, 255);
    private static readonly Color FillShadowColor = new Color32(224, 96, 44, 255);

    private Slider musicSlider;

    // Bouwt het paneel als laatste kind van parent (komt dus bovenop). Staat uit tot je het aanzet.
    public static GameObject Create(Transform parent, Sprite background, UnityAction onBack)
    {
        RectTransform root = CreateRect(parent, "SettingsPanel", new Rect(0, 0, ImageWidth, ImageHeight));
        root.gameObject.SetActive(false);
        root.SetAsLastSibling();

        Image image = root.gameObject.AddComponent<Image>();   // vangt ook kliks op het menu erachter af
        if (background != null) image.sprite = background;
        else
        {
            image.color = TrackColor;
            Debug.LogWarning("SettingsMenu: geen achtergrond (ui_options) gekoppeld");
        }

        SettingsMenu menu = root.gameObject.AddComponent<SettingsMenu>();
        menu.musicSlider = CreateSlider(root, new Rect(149, 76, 99, 11));
        menu.musicSlider.onValueChanged.AddListener(menu.OnMusicChanged);

        CreateClickArea(root, "StandaardKnop", new Rect(84, 127, 72, 17), menu.ResetToDefaults);
        CreateClickArea(root, "TerugKnop", new Rect(164, 127, 72, 17), onBack);

        return root.gameObject;
    }

    // Bij openen de opgeslagen waarde tonen, zonder meteen opnieuw op te slaan.
    private void OnEnable()
    {
        SaveManager save = SaveManager.Instance;
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(save != null ? save.MusicVolume : DefaultVolume);
    }

    private void OnMusicChanged(float value)
    {
        if (SaveManager.Instance != null) SaveManager.Instance.SetMusicVolume(value);
    }

    // Knop: Standaard (zet ook de geluidseffecten terug)
    private void ResetToDefaults()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetMusicVolume(DefaultVolume);
            SaveManager.Instance.SetSfxVolume(DefaultVolume);
        }
        musicSlider.SetValueWithoutNotify(DefaultVolume);
    }

    // ---------- Opbouw ----------

    // Slider precies over de getekende slider; de rij-kleur eronder verbergt de getekende hendel.
    private static Slider CreateSlider(Transform parent, Rect pixels)
    {
        RectTransform rect = CreateRect(parent, "MuziekSlider", pixels);
        AddImage(rect, RowColor);

        RectTransform border = CreateInset(rect, "Rand", 1, 3, 1, 3);
        AddImage(border, BorderColor);
        RectTransform track = CreateInset(border, "Baan", 1, 1, 1, 1);
        AddImage(track, TrackColor);

        RectTransform fill = CreateInset(track, "Vulling", 0, 0, 0, 0);
        AddImage(fill, FillColor);
        RectTransform shadow = CreateInset(fill, "Schaduw", 0, 0, 0, 0);
        shadow.anchorMax = new Vector2(1f, 1f / 3f);   // onderste rij donkerder, zoals in de afbeelding
        AddImage(shadow, FillShadowColor);

        RectTransform handleArea = CreateInset(rect, "HendelGebied", 4, 1, 4, 1);
        RectTransform handle = CreateInset(handleArea, "Hendel", 0, 0, 0, 0);
        handle.anchorMin = new Vector2(0f, 0f);
        handle.anchorMax = new Vector2(0f, 1f);
        handle.sizeDelta = new Vector2(5f, 0f);
        Image handleImage = AddImage(handle, BorderColor);
        AddImage(CreateInset(handle, "Binnen", 1, 1, 1, 1), FillColor);

        Slider slider = rect.gameObject.AddComponent<Slider>();
        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handleImage;
        slider.transition = Selectable.Transition.None;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(DefaultVolume);
        return slider;
    }

    // Onzichtbare knop over een knop die al in de afbeelding getekend is.
    private static void CreateClickArea(Transform parent, string name, Rect pixels, UnityAction onClick)
    {
        RectTransform rect = CreateRect(parent, name, pixels);
        AddImage(rect, new Color(1f, 1f, 1f, 0f));   // onzichtbaar, maar wel klikbaar

        Button button = rect.gameObject.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
        if (onClick != null) button.onClick.AddListener(onClick);
    }

    private static Image AddImage(RectTransform rect, Color color)
    {
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        return image;
    }

    // Vaste plek in de 320x180-afbeelding, schaalt mee met de ouder.
    private static RectTransform CreateRect(Transform parent, string name, Rect pixels)
    {
        RectTransform rect = CreateInset(parent, name, 0, 0, 0, 0);
        rect.anchorMin = new Vector2(pixels.xMin / ImageWidth, 1f - pixels.yMax / ImageHeight);
        rect.anchorMax = new Vector2(pixels.xMax / ImageWidth, 1f - pixels.yMin / ImageHeight);
        return rect;
    }

    // Vult de ouder, met een rand in pixels aan elke kant.
    private static RectTransform CreateInset(Transform parent, string name, float left, float top, float right, float bottom)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;

        RectTransform rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
        return rect;
    }
}
