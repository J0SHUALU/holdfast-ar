using UnityEngine;

namespace HoldfastAR.Data
{
    [CreateAssetMenu(menuName = "Holdfast AR/Difficulty Settings", fileName = "Difficulty")]
    public class DifficultySettings : ScriptableObject
    {
        public string displayName = "Normal";

        [Header("Match")]
        [Min(10f)] public float matchDuration = 90f;
        [Min(1f)] public float playerMaxHealth = 100f;

        [Header("Spawning")]
        [Min(0.2f)] public float spawnInterval = 2.5f;
        [Min(1)] public int maxAliveEnemies = 6;
        [Range(0f, 1f)] public float shooterChance = 0.4f;

        [Header("Enemy scaling")]
        [Min(0.1f)] public float enemySpeedMultiplier = 1f;
        [Min(0.1f)] public float enemyDamageMultiplier = 1f;
        [Min(0.1f)] public float scoreMultiplier = 1f;

        public static DifficultySettings Create(string name, float duration, float health, float interval,
            int maxAlive, float shooterChance, float speed, float damage, float score)
        {
            var d = CreateInstance<DifficultySettings>();
            d.name = name;
            d.displayName = name;
            d.matchDuration = duration;
            d.playerMaxHealth = health;
            d.spawnInterval = interval;
            d.maxAliveEnemies = maxAlive;
            d.shooterChance = shooterChance;
            d.enemySpeedMultiplier = speed;
            d.enemyDamageMultiplier = damage;
            d.scoreMultiplier = score;
            return d;
        }

        public static DifficultySettings[] CreateDefaults() => new[]
        {
            Create("Easy", 60f, 150f, 3.2f, 4, 0.25f, 0.8f, 0.6f, 0.8f),
            Create("Normal", 90f, 100f, 2.4f, 6, 0.4f, 1f, 1f, 1f),
            Create("Hard", 120f, 80f, 1.6f, 9, 0.5f, 1.3f, 1.4f, 1.5f),
        };
    }
}
