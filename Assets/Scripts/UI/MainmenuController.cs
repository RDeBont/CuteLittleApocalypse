using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Zet op een leeg GameObject in de scene MainMenu.</summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string levelScene = "Level_01";

    [Header("Opslag")]
    [Tooltip("Bestandsnaam van het JSON-savebestand. Zelfde naam als in SaveManager.")]
    [SerializeField] private string saveFileName = "save.json";

    [Header("UI")]
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        // Terugkomen uit een level via pauze/eindscherm: nooit bevroren in het menu.
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (continueButton != null)
            continueButton.interactable = File.Exists(Path.Combine(Application.persistentDataPath, saveFileName));

        ShowMain();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null && settingsPanel.activeSelf)
            ShowMain();
    }

    // Knop: Nieuw spel
    public void NewGame() => LoadLevel();

    // Knop: Verder spelen (SaveManager laadt de voortgang in het level)
    public void ContinueGame() => LoadLevel();

    // Knop: Instellingen
    public void ShowSettings()
    {
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

    private void LoadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
    }
}