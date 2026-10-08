using System;
using UnityEngine;

/// <summary>
/// Level starten, voltooien, game over en statistieken (tijd, resterende munitie,
/// uitgeschakelde ponies). Zet op een leeg GameObject in Level01.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelState State { get; private set; } = LevelState.Idle;
    public LevelStats Stats { get; private set; } = new LevelStats();

    public event Action OnLevelStarted;
    public event Action<LevelStats> OnLevelCompleted;   // UIManager: eindscherm tonen
    public event Action<LevelStats> OnGameOver;         // UIManager: game over-scherm tonen
    public event Action OnRetry;                        // PlayerHealth: respawn op checkpoint

    int killsAtCheckpoint;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => StartLevel();

    void Update()
    {
        // Time.deltaTime is 0 bij pauze (timeScale 0), dus de timer pauzeert vanzelf.
        if (State == LevelState.Playing) Stats.AddTime(Time.deltaTime);
    }

    public void StartLevel()
    {
        Stats = new LevelStats();
        killsAtCheckpoint = 0;
        Time.timeScale = 1f;
        State = LevelState.Playing;
        OnLevelStarted?.Invoke();
    }

    // Aanroepen vanuit EnemyBase.Die()
    public void RegisterPonyKilled()
    {
        if (State == LevelState.Playing) Stats.RegisterKill();
    }

    // Aanroepen vanuit Checkpoint bij activeren
    public void SetCheckpoint() => killsAtCheckpoint = Stats.PoniesKilled;

    // Aanroepen vanuit de Chinook-trigger: LevelManager.Instance.CompleteLevel(weapon.CurrentAmmo)
    public void CompleteLevel(int ammoLeft)
    {
        if (State != LevelState.Playing) return;
        Stats.AmmoLeft = ammoLeft;
        State = LevelState.Completed;
        Time.timeScale = 0f;
        OnLevelCompleted?.Invoke(Stats);
    }

    // Aanroepen vanuit PlayerHealth bij 0 HP
    public void GameOver(int ammoLeft)
    {
        if (State != LevelState.Playing) return;
        Stats.AmmoLeft = ammoLeft;
        State = LevelState.GameOver;
        Time.timeScale = 0f;
        OnGameOver?.Invoke(Stats);
    }

    // Knop "Opnieuw proberen": positie, HP en munitie worden door PlayerHealth/WeaponController
    // teruggezet via OnRetry. Tijd loopt door, kills gaan terug naar de stand bij het checkpoint.
    public void RetryFromCheckpoint()
    {
        if (State != LevelState.GameOver) return;
        Stats.PoniesKilled = killsAtCheckpoint;
        Time.timeScale = 1f;
        State = LevelState.Playing;
        OnRetry?.Invoke();
    }
}