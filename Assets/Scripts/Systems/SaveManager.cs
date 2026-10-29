using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveData
{
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
    public float bestTimeSeconds = 0f;   // 0 = nog geen tijd
}

/// <summary>
/// Slaat volume-instellingen en de beste tijd op in save.json (persistentDataPath).
/// Zet op een leeg GameObject in de scene MainMenu; blijft bestaan tussen scenes.
/// Voortgang binnen het level wordt bewust niet opgeslagen.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Opslag")]
    [Tooltip("Bestandsnaam van het JSON-savebestand.")]
    [SerializeField] private string saveFileName = "save.json";

    public SaveData Data { get; private set; } = new SaveData();

    public float MusicVolume => Data.musicVolume;
    public float SfxVolume => Data.sfxVolume;
    public float BestTimeSeconds => Data.bestTimeSeconds;
    public bool HasBestTime => Data.bestTimeSeconds > 0f;

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private LevelManager subscribedLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnsubscribeLevel();
    }

    // Voor als de SaveManager direct in het level staat (testen zonder hoofdmenu).
    private void Start() => SubscribeLevel();

    // LevelManager zet Instance in Awake, dat is al gebeurd als sceneLoaded afgaat.
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => SubscribeLevel();

    private void SubscribeLevel()
    {
        if (Instance != this) return;   // dubbele SaveManager wordt al opgeruimd

        LevelManager level = LevelManager.Instance;
        if (level == null || level == subscribedLevel) return;

        UnsubscribeLevel();
        subscribedLevel = level;
        subscribedLevel.OnLevelCompleted += HandleLevelCompleted;
    }

    private void UnsubscribeLevel()
    {
        if (subscribedLevel != null)
            subscribedLevel.OnLevelCompleted -= HandleLevelCompleted;
        subscribedLevel = null;
    }

    private void HandleLevelCompleted(LevelStats stats) => TrySetBestTime(stats.TimeSeconds);

    public void Load()
    {
        Data = new SaveData();

        if (!File.Exists(SavePath)) return;

        try
        {
            // Overwrite: ontbrekende velden houden hun standaardwaarde.
            JsonUtility.FromJsonOverwrite(File.ReadAllText(SavePath), Data);
        }
        catch (Exception e)
        {
            Debug.LogWarning("SaveManager: save.json kon niet gelezen worden, standaardwaarden gebruikt. " + e.Message);
            Data = new SaveData();
        }

        Data.musicVolume = ClampVolume(Data.musicVolume);
        Data.sfxVolume = ClampVolume(Data.sfxVolume);
        if (float.IsNaN(Data.bestTimeSeconds) || Data.bestTimeSeconds < 0f) Data.bestTimeSeconds = 0f;
    }

    public void Save()
    {
        try
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(Data, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning("SaveManager: opslaan mislukt. " + e.Message);
        }
    }

    // Slider "Muziek" (OnValueChanged, Dynamic float)
    public void SetMusicVolume(float volume)
    {
        Data.musicVolume = ClampVolume(volume);
        Save();
    }

    // Slider "Geluidseffecten" (OnValueChanged, Dynamic float)
    public void SetSfxVolume(float volume)
    {
        Data.sfxVolume = ClampVolume(volume);
        Save();
    }

    // Geeft true terug als dit een nieuwe beste tijd is.
    public bool TrySetBestTime(float seconds)
    {
        if (seconds <= 0f) return false;
        if (HasBestTime && seconds >= Data.bestTimeSeconds) return false;

        Data.bestTimeSeconds = seconds;
        Save();
        return true;
    }

    private static float ClampVolume(float volume)
    {
        return float.IsNaN(volume) ? 0.8f : Mathf.Clamp01(volume);
    }
}
