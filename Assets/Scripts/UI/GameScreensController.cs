using System;
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

    [Header("Eindscherm scores")]
    [Tooltip("apocalypse_font_8x8_solid of _outline (PNG)")]
    [SerializeField] private Texture2D font;
    [SerializeField] private Color valueColor = Color.white;

    private PixelText timeText;
    private PixelText ammoText;
    private PixelText killsText;

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

        if (timeText != null) timeText.Text = FormatMinutesSeconds(stats.TimeSeconds);
        if (ammoText != null) ammoText.Text = stats.AmmoLeft.ToString();
        if (killsText != null) killsText.Text = stats.PoniesKilled.ToString();
    }

    // Kort (02:41) zodat het naast de labels past, net als in het ontwerp
    private static string FormatMinutesSeconds(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
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

        // Oude TextMeshPro-teksten in dit paneel uitzetten, de waardes komen nu in het pixelfont
        foreach (TMPro.TMP_Text oldText in levelCompletePanel.GetComponentsInChildren<TMPro.TMP_Text>(true))
            oldText.gameObject.SetActive(false);

        // Waardes rechts naast de labels TIJD / MUNITIE / VIJANDEN (labels staan op y 80, 90 en 100)
        if (font == null) Debug.LogWarning("GameScreensController: geen font gekoppeld, scores blijven leeg");
        timeText = PixelText.Create(panel, "TimeText", new Rect(154, 79, 60, 8), font, PixelText.Align.Right, valueColor);
        ammoText = PixelText.Create(panel, "AmmoText", new Rect(154, 89, 60, 8), font, PixelText.Align.Right, valueColor);
        killsText = PixelText.Create(panel, "KillsText", new Rect(154, 99, 60, 8), font, PixelText.Align.Right, valueColor);
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
