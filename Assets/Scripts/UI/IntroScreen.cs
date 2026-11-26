using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Kort tekstscherm aan het begin van het level. Het spel (en de timer) staat stil
/// tot de speler op een toets drukt. Zet op het GameObject GameScreens in Level01;
/// het paneel wordt in code opgebouwd op dat canvas. Komt niet terug bij Opnieuw proberen.
/// Tekst in het pixelfont (apocalypse_font_8x8): max. 36 tekens per regel, langere regels lopen door.
/// </summary>
public class IntroScreen : MonoBehaviour
{
    private const float CanvasWidth = 320f;
    private const float CanvasHeight = 180f;

    private static readonly Color BackdropColor = new Color32(12, 14, 22, 255);   // #0C0E16 uit het GDD

    [Header("Font")]
    [Tooltip("apocalypse_font_8x8_solid of _outline (PNG)")]
    [SerializeField] private Texture2D font;

    [Header("Tekst")]
    [TextArea(6, 14)]
    [SerializeField] private string story =
        "De stad is gevallen.\n" +
        "Jouw squad ook.\n" +
        "\n" +
        "Alleen jij leeft nog, met informatie die het leger nodig heeft.\n" +
        "\n" +
        "De Chinook wacht op het laatste dak.\n" +
        "De ponies zoeken je al.\n" +
        "\n" +
        "Haal het einde. Zuinig met je kogels.";
    [SerializeField] private string hint = "Druk op een toets";
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color hintColor = new Color(1f, 1f, 1f, 0.7f);

    [Header("Timing")]
    [Tooltip("Zo lang (in echte seconden) kan het scherm niet weggeklikt worden.")]
    [SerializeField] private float minShowTime = 0.5f;

    // PauseMenuController: niet pauzeren zolang dit scherm open is (ook niet in het frame waarin het sluit,
    // anders opent Esc meteen het pauzemenu)
    public static bool IsShowing => showing || Time.frameCount == closedFrame;

    private static bool showing;
    private static int closedFrame = -1;

    private GameObject panel;
    private PixelText hintText;
    private WeaponController weapon;
    private float shownAt;

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("IntroScreen: geen Canvas gevonden, zet dit script op GameScreens");
            return;
        }

        if (font == null) Debug.LogWarning("IntroScreen: geen font gekoppeld, tekst blijft leeg");

        panel = Build(canvas.transform);
        Show();
    }

    private void OnDestroy()
    {
        showing = false;
    }

    private void Update()
    {
        if (!showing) return;

        // LevelManager zet bij de start timeScale op 1; zolang dit scherm open is blijft alles stil.
        Time.timeScale = 0f;

        if (hintText != null) hintText.gameObject.SetActive(Mathf.Repeat(Time.unscaledTime, 1f) < 0.6f);

        // Muisklik telt niet: die zou anders ook als schot gelden
        bool keyPressed = Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2);
        if (keyPressed && Time.unscaledTime - shownAt >= minShowTime)
            Hide();
    }

    private void Show()
    {
        showing = true;
        shownAt = Time.unscaledTime;
        Time.timeScale = 0f;
        panel.SetActive(true);

        weapon = FindFirstObjectByType<WeaponController>();
        if (weapon != null) weapon.enabled = false;
    }

    private void Hide()
    {
        showing = false;
        closedFrame = Time.frameCount;
        panel.SetActive(false);
        Time.timeScale = 1f;

        if (weapon != null) weapon.enabled = true;
    }

    // ---------- Opbouw ----------

    private GameObject Build(Transform parent)
    {
        RectTransform root = CreateRect(parent, "IntroPanel", new Rect(0, 0, CanvasWidth, CanvasHeight));
        root.SetAsLastSibling();

        Image backdrop = root.gameObject.AddComponent<Image>();
        backdrop.color = BackdropColor;

        PixelText storyText = PixelText.Create(root, "Verhaal", new Rect(16, 12, 288, 138), font, PixelText.Align.Center, textColor, true);
        storyText.Text = story;

        hintText = PixelText.Create(root, "Hint", new Rect(16, 160, 288, 8), font, PixelText.Align.Center, hintColor);
        hintText.Text = hint;

        root.gameObject.SetActive(false);
        return root.gameObject;
    }

    // Vaste plek in het 320x180-canvas, schaalt mee met de ouder.
    private static RectTransform CreateRect(Transform parent, string name, Rect pixels)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;

        RectTransform rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(pixels.xMin / CanvasWidth, 1f - pixels.yMax / CanvasHeight);
        rect.anchorMax = new Vector2(pixels.xMax / CanvasWidth, 1f - pixels.yMin / CanvasHeight);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }
}
