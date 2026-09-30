using HoldfastAR.Combat;
using HoldfastAR.Core;
using HoldfastAR.Data;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    /// <summary>Everything an enemy needs from the outside world, injected by the EnemyFactory.</summary>
    public readonly struct EnemyContext
    {
        public readonly Transform Target;
        public readonly IDamageable TargetHealth;
        public readonly DifficultySettings Difficulty;
        public readonly ProjectilePool EnemyProjectiles;

        public EnemyContext(Transform target, IDamageable targetHealth, DifficultySettings difficulty, ProjectilePool enemyProjectiles)
        {
            Target = target;
            TargetHealth = targetHealth;
            Difficulty = difficulty;
            EnemyProjectiles = enemyProjectiles;
        }
    }
}
