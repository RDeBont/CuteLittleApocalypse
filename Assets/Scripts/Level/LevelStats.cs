using System;

public enum LevelState { Idle, Playing, Completed, GameOver }

[Serializable]
public class LevelStats
{
    public float TimeSeconds;
    public int AmmoLeft;
    public int PoniesKilled;

    public void AddTime(float deltaTime)
    {
        if (deltaTime > 0f) TimeSeconds += deltaTime;
    }

    public void RegisterKill() => PoniesKilled++;

    public string FormattedTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(TimeSeconds);
        return $"{(int)t.TotalMinutes:00}:{t.Seconds:00}.{t.Milliseconds / 10:00}";
    }
}