using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Zet op een leeg GameObject in de scene MainMenu.</summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string levelScene = "Level_01";

    [Header("UI")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [Tooltip("Achtergrond voor het instellingenpaneel als Settings Panel leeg is (ui_options).")]
    [SerializeField] private Sprite optionsBackground;

    private void Awake()
    {
        // Terugkomen uit een level via pauze/eindscherm: nooit bevroren in het menu.
        Time.timeScale = 1f;
    }

    private void Start()
    {
        ShowMain();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null && settingsPanel.activeSelf)
            ShowMain();
    }

    // Knop: Nieuw spel
    public void NewGame() => LoadLevel();

    // Knop: Instellingen
    public void ShowSettings()
    {
        // Geen paneel gekoppeld in de Inspector: dan bouwt SettingsMenu er zelf een.
        if (settingsPanel == null) settingsPanel = SettingsMenu.Create(GetCanvas(), optionsBackground, ShowMain);

        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // Knop: Terug (in instellingen)
    public void ShowMain()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    // Knop: Afsluiten
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private Transform GetCanvas()
    {
        Canvas canvas = mainPanel != null ? mainPanel.GetComponentInParent<Canvas>() : FindFirstObjectByType<Canvas>();
        return canvas != null ? canvas.transform : transform;
    }

    private void LoadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
    }
}