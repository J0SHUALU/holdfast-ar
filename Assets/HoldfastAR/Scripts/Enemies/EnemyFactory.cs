using HoldfastAR.Combat;
using HoldfastAR.Core;
using HoldfastAR.Data;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    /// <summary>
    /// Factory pattern: the only place that knows which prefab belongs to which
    /// EnemyType and how to wire an enemy's dependencies. The spawner just asks
    /// for "a Shooter here" and gets back a ready-to-fight Enemy.
    /// </summary>
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private MeleeEnemy meleePrefab;
        [SerializeField] private ShooterEnemy shooterPrefab;
        [SerializeField] private ProjectilePool enemyProjectilePool;

        public Enemy Create(EnemyType type, Vector3 position, Transform parent,
            Transform target, IDamageable targetHealth, DifficultySettings difficulty)
        {
            Enemy prefab = type == EnemyType.Melee ? meleePrefab : (Enemy)shooterPrefab;
            if (prefab == null)
            {
                Debug.LogError($"EnemyFactory: no prefab for {type}", this);
                return null;
            }

            Vector3 toTarget = target.position - position;
            toTarget.y = 0f;
            Quaternion facing = toTarget.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(toTarget) : Quaternion.identity;

            Enemy enemy = Instantiate(prefab, position, facing, parent);
            enemy.name = $"{type} Enemy";
            enemy.Initialize(new EnemyContext(target, targetHealth, difficulty, enemyProjectilePool));
            return enemy;
        }
    }
}
