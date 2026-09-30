using System;
using UnityEngine;

namespace HoldfastAR.Core
{
    /// <summary>
    /// Central event hub (Observer pattern).
    /// Gameplay systems publish, UI and audio subscribe. Nobody needs a direct
    /// reference to the HUD, so gameplay code stays decoupled from presentation.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<float, float> PlayerHealthChanged;   // current, max
        public static event Action<float> PlayerDamaged;                // amount
        public static event Action PlayerDied;
        public static event Action<int> ScoreChanged;
        public static event Action<int> KillsChanged;
        public static event Action<float> TimeRemainingChanged;         // seconds
        public static event Action<int, Vector3> EnemyKilled;           // score value, position
        public static event Action<string> GameStateChanged;            // state name

        public static void RaisePlayerHealthChanged(float current, float max) => PlayerHealthChanged?.Invoke(current, max);
        public static void RaisePlayerDamaged(float amount) => PlayerDamaged?.Invoke(amount);
        public static void RaisePlayerDied() => PlayerDied?.Invoke();
        public static void RaiseScoreChanged(int score) => ScoreChanged?.Invoke(score);
        public static void RaiseKillsChanged(int kills) => KillsChanged?.Invoke(kills);
        public static void RaiseTimeRemainingChanged(float seconds) => TimeRemainingChanged?.Invoke(seconds);
        public static void RaiseEnemyKilled(int scoreValue, Vector3 position) => EnemyKilled?.Invoke(scoreValue, position);
        public static void RaiseGameStateChanged(string stateName) => GameStateChanged?.Invoke(stateName);
    }
}
