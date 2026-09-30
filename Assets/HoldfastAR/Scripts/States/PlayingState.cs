using HoldfastAR.Core;
using HoldfastAR.Data;
using UnityEngine;

namespace HoldfastAR.States
{
    /// <summary>
    /// The match itself: counts down the timer, runs the spawner and listens for
    /// kills and the player's death. Ends in victory when time runs out.
    /// </summary>
    public class PlayingState : GameState
    {
        private GameSession _session;
        private bool _playerDied;

        public PlayingState(GameManager game) : base(game) { }

        public override string Name => "Playing";

        public override void Enter()
        {
            DifficultySettings difficulty = Game.SelectedDifficulty;
            _session = new GameSession(difficulty);
            Game.CurrentSession = _session;
            _playerDied = false;

            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.PlayerDied += OnPlayerDied;

            Game.UI.ShowHud();
            Game.PlayerHealth.ResetHealth(difficulty.playerMaxHealth);
            Game.PlayerWeapon.ResetWeapon();
            Game.PlayerWeapon.CanFire = true;
            Game.Spawner.Begin(difficulty, Game.Placement.SpawnArea, Game.Placement.Arena,
                Game.PlayerHealth.transform, Game.PlayerHealth);
        }

        public override void Tick(float deltaTime)
        {
            if (_playerDied)
            {
                Game.EndMatch(false);
                return;
            }

            _session.Tick(deltaTime);
            Game.Spawner.Tick(deltaTime);

            if (_session.IsTimeUp) Game.EndMatch(true);
        }

        public override void Exit()
        {
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.PlayerDied -= OnPlayerDied;

            Game.PlayerWeapon.CanFire = false;
            Game.PlayerWeapon.ResetWeapon();
            Game.PlayerHealth.Invincible = true;

            // Wipe the battlefield: every enemy and every in-flight bullet goes away.
            Game.Spawner.StopAndClear();
            Game.PlayerProjectiles.ReleaseAll();
            Game.EnemyProjectiles.ReleaseAll();
        }

        private void OnEnemyKilled(int scoreValue, Vector3 position) => _session.RegisterKill(scoreValue);

        private void OnPlayerDied() => _playerDied = true;
    }
}
