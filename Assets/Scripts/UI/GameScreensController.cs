using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Toont het eindscherm en het game over-scherm op basis van de LevelManager-events.
/// Zet op het GameObject GameScreens in Level01 (blijft actief, de panelen zelf staan uit).
/// De knoppen en labels zitten in de achtergrondafbeelding; dit script legt er bij het
/// starten onzichtbare klikvlakken en de waardes overheen. Toetsen: Enter = bovenste knop, Esc = onderste.
/// </summary>
public class GameScreensController : MonoBehaviour
{
    // Afbeeldingen zijn 320x180 pixels, alle posities hieronder in pixels vanaf linksboven.
    private const float ImageWidth = 320f;
    private const float ImageHeight = 180f;

    [Header("Panelen")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("Eindscherm teksten (leeg = automatisch aanmaken)")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text ammoText;

    [Header("Opmaak automatische teksten")]
    [SerializeField] private float fontSize = 8f;
    [SerializeField] private Color valueColor = new Color(0.78f, 0.80f, 0.88f);
    [SerializeField] private Color highlightColor = new Color(1f, 0.67f, 0.2f);

    private LevelManager level;

    private void Awake()
    {
        BuildLevelCompleteScreen();
        BuildGameOverScreen();
    }

    private void Start()
    {
        HideAll();

        level = LevelManager.Instance;
        if (level == null)
        {
            Debug.LogWarning("GameScreensController: geen LevelManager in de scene");
            return;
        }

        level.OnLevelStarted += HideAll;
        level.OnRetry += HideAll;
        level.OnLevelCompleted += ShowLevelComplete;
        level.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        if (level == null) return;

        level.OnLevelStarted -= HideAll;
        level.OnRetry -= HideAll;
        level.OnLevelCompleted -= ShowLevelComplete;
        level.OnGameOver -= ShowGameOver;
    }

    private void Update()
    {
        bool confirm = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);
        bool back = Input.GetKeyDown(KeyCode.Escape);
        if (!confirm && !back) return;

        if (IsActive(levelCompletePanel))
        {
            if (confirm) NextLevel();
            else QuitToMainMenu();
        }
        else if (IsActive(gameOverPanel))
        {
            if (confirm) RetryFromCheckpoint();
            else QuitToMainMenu();
        }
    }

    // ---------- Schermen tonen ----------

    private void ShowLevelComplete(LevelStats stats)
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);

        if (timeText != null) timeText.text = stats.FormattedTime();
        if (ammoText != null) ammoText.text = stats.AmmoLeft.ToString();
        if (killsText != null) killsText.text = stats.PoniesKilled.ToString();
    }

    private void ShowGameOver(LevelStats stats)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private void HideAll()
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // ---------- Knoppen ----------

    // Knop: Volgende (eindscherm). Volgende scene in de Build Settings, anders terug naar het menu.
    public void NextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        Time.timeScale = 1f;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(mainMenuScene);
    }

    // Knop: Restart (game over-scherm)
    public void RetryFromCheckpoint()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.RetryFromCheckpoint();
    }

    // Knop: Menu / Quit
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    // ---------- Opbouw ----------

    private void BuildLevelCompleteScreen()
    {
        if (levelCompletePanel == null) return;
        Transform panel = levelCompletePanel.transform;

        CreateClickArea(panel, "VolgendeKnop", new Rect(124, 116, 72, 15), NextLevel);
        CreateClickArea(panel, "MenuKnop", new Rect(124, 134, 72, 15), QuitToMainMenu);

        // Waardes rechts naast de labels TIJD / MUNITIE / VIJANDEN
        if (timeText == null) timeText = CreateText(panel, "TimeText", new Rect(150, 76, 58, 10), valueColor, TextAlignmentOptions.Right);
        if (ammoText == null) ammoText = CreateText(panel, "AmmoText", new Rect(150, 86, 58, 10), valueColor, TextAlignmentOptions.Right);
        if (killsText == null) killsText = CreateText(panel, "KillsText", new Rect(150, 96, 58, 10), highlightColor, TextAlignmentOptions.Right);
    }

    private void BuildGameOverScreen()
    {
        if (gameOverPanel == null) return;
        Transform panel = gameOverPanel.transform;

        CreateClickArea(panel, "RestartKnop", new Rect(124, 105, 72, 14), RetryFromCheckpoint);
        CreateClickArea(panel, "QuitKnop", new Rect(124, 124, 72, 14), QuitToMainMenu);
    }

    // Onzichtbare knop over een knop die al in de afbeelding getekend is.
    private static void CreateClickArea(Transform parent, string name, Rect pixels, UnityAction onClick)
    {
        RectTransform rect = CreateRect(parent, name, pixels);

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);   // onzichtbaar, maar wel klikbaar

        Button button = rect.gameObject.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(onClick);
    }

    private TMP_Text CreateText(Transform parent, string name, Rect pixels, Color color, TextAlignmentOptions alignment)
    {
        RectTransform rect = CreateRect(parent, name, pixels);

        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        text.text = string.Empty;
        return text;
    }

    // Zet een RectTransform op een vaste plek in de 320x180-afbeelding, schaalt mee met het paneel.
    private static RectTransform CreateRect(Transform parent, string name, Rect pixels)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;

        RectTransform rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(pixels.xMin / ImageWidth, 1f - pixels.yMax / ImageHeight);
        rect.anchorMax = new Vector2(pixels.xMax / ImageWidth, 1f - pixels.yMin / ImageHeight);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static bool IsActive(GameObject panel) => panel != null && panel.activeInHierarchy;
}
