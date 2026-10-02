using System.Collections.Generic;
using HoldfastAR.AR;
using HoldfastAR.Audio;
using HoldfastAR.Core;
using HoldfastAR.Data;
using UnityEngine;

namespace HoldfastAR.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyFactory factory;
        [SerializeField] private float minSpawnRadius = 0.9f;
        [SerializeField] private float maxSpawnRadius = 2.2f;
        [SerializeField] private float minDistanceFromPlayer = 1.1f;
        [SerializeField] private float firstSpawnDelay = 1.5f;

        private readonly List<Enemy> _alive = new List<Enemy>();
        private DifficultySettings _difficulty;
        private PlaneSpawnArea _area;
        private Transform _arena;
        private Transform _player;
        private IDamageable _playerHealth;
        private float _timer;
        private bool _running;

        public int AliveCount => _alive.Count;

        public void Begin(DifficultySettings difficulty, PlaneSpawnArea area, Transform arena, Transform player, IDamageable playerHealth)
        {
            _difficulty = difficulty;
            _area = area;
            _arena = arena;
            _player = player;
            _playerHealth = playerHealth;
            _timer = firstSpawnDelay;
            _running = true;
        }

        public void Tick(float deltaTime)
        {
            if (!_running) return;
            _timer -= deltaTime;
            if (_timer > 0f) return;

            _timer = _difficulty.spawnInterval * Random.Range(0.8f, 1.2f);
            if (_alive.Count < _difficulty.maxAliveEnemies) SpawnOne();
        }

        private void SpawnOne()
        {
            EnemyType type = Random.value < _difficulty.shooterChance ? EnemyType.Shooter : EnemyType.Melee;
            Vector3 position = _area.GetSpawnPoint(minSpawnRadius, maxSpawnRadius, _player.position, minDistanceFromPlayer);

            Enemy enemy = factory.Create(type, position, _arena, _player, _playerHealth, _difficulty);
            if (enemy == null) return;

            _alive.Add(enemy);
            enemy.Removed += OnEnemyRemoved;
            AudioManager.Instance?.PlayAt(SoundId.EnemySpawn, position);
            GameEvents.RaiseEnemySpawned(enemy.transform);
        }

        private void OnEnemyRemoved(Enemy enemy) => _alive.Remove(enemy);

        public void StopAndClear()
        {
            _running = false;
            for (int i = _alive.Count - 1; i >= 0; i--)
            {
                Enemy e = _alive[i];
                if (e != null)
                {
                    e.Removed -= OnEnemyRemoved;
                    e.Despawn();
                }
            }
            _alive.Clear();
        }
    }
}
