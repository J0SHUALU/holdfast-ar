using System;
using HoldfastAR.Core;

namespace HoldfastAR.Data
{
    /// <summary>
    /// Runtime stats for the current match: score, kills and the countdown timer.
    /// Fields are private and only change through methods, and every change is
    /// broadcast through GameEvents so the HUD updates itself.
    /// </summary>
    public class GameSession
    {
        public DifficultySettings Difficulty { get; }
        public int Score { get; private set; }
        public int EnemiesDefeated { get; private set; }
        public float TimeRemaining { get; private set; }
        public float TimeSurvived => Difficulty.matchDuration - TimeRemaining;
        public bool IsTimeUp => TimeRemaining <= 0f;

        public GameSession(DifficultySettings difficulty)
        {
            Difficulty = difficulty;
            TimeRemaining = difficulty.matchDuration;
            GameEvents.RaiseScoreChanged(Score);
            GameEvents.RaiseKillsChanged(EnemiesDefeated);
            GameEvents.RaiseTimeRemainingChanged(TimeRemaining);
        }

        public void Tick(float deltaTime)
        {
            if (IsTimeUp) return;
            TimeRemaining = Math.Max(0f, TimeRemaining - deltaTime);
            GameEvents.RaiseTimeRemainingChanged(TimeRemaining);
        }

        public void RegisterKill(int baseScore)
        {
            EnemiesDefeated++;
            Score += (int)Math.Round(baseScore * Difficulty.scoreMultiplier);
            GameEvents.RaiseKillsChanged(EnemiesDefeated);
            GameEvents.RaiseScoreChanged(Score);
        }

        public SessionRecord ToRecord(bool survived) => new SessionRecord
        {
            date = DateTime.Now.ToString("dd MMM HH:mm"),
            difficulty = Difficulty.displayName,
            score = Score + (survived ? (int)Math.Round(250 * Difficulty.scoreMultiplier) : 0),
            enemiesDefeated = EnemiesDefeated,
            timeSurvived = TimeSurvived,
            survived = survived
        };
    }
}
