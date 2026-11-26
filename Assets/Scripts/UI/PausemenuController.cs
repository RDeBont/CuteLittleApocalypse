using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Zet op een GameObject in Level01 (blijft actief, de panelen zelf staan uit).</summary>
public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string mainMenuScene = "MainMenu";

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        ForceResumeState();
    }

    private void Start()
    {
        ForceResumeState();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (settingsPanel != null && settingsPanel.activeSelf)
            CloseSettings();                 // Esc in instellingen: terug naar pauzemenu
        else if (IsPaused)
            Resume();
        else if (CanPause())
            Pause();
    }

    // Niet pauzeren tijdens het begintekstscherm, game over of het eindscherm (daar staat timeScale al op 0).
    private bool CanPause()
    {
        if (IntroScreen.IsShowing) return false;
        return LevelManager.Instance == null || LevelManager.Instance.State == LevelState.Playing;
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // Knop: Doorgaan
    public void Resume()
    {
        ForceResumeState();
    }

    // Knop: Instellingen
    public void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // Knop: Terug (in instellingen)
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // Knop: Terug naar hoofdmenu (ook voor "Hoofdmenu" op het eindscherm en game over)
    public void QuitToMainMenu()
    {
        ForceResumeState();
        SceneManager.LoadScene(mainMenuScene);
    }

    // Knop: level volledig herstarten (optioneel)
    public void RestartLevel()
    {
        ForceResumeState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ForceResumeState()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
}