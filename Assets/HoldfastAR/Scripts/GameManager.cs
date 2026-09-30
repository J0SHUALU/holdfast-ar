using HoldfastAR.AR;
using HoldfastAR.Combat;
using HoldfastAR.Core;
using HoldfastAR.Data;
using HoldfastAR.Enemies;
using HoldfastAR.Player;
using HoldfastAR.States;
using HoldfastAR.UI;
using UnityEngine;

namespace HoldfastAR
{
    /// <summary>
    /// Top-level coordinator (Singleton). Owns the state machine and exposes the
    /// systems that states need. Flow: MainMenu -> Placement -> Playing -> GameOver.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private const string DifficultyPrefsKey = "HoldfastAR.Difficulty";

        [Header("Systems")]
        [SerializeField] private ARPlacementController placement;
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerWeapon playerWeapon;
        [SerializeField] private ProjectilePool playerProjectiles;
        [SerializeField] private ProjectilePool enemyProjectiles;
        [SerializeField] private UIManager ui;

        [Header("Difficulty")]
        [SerializeField] private DifficultySettings[] difficulties;

        private readonly GameStateMachine _stateMachine = new GameStateMachine();
        private int _difficultyIndex = 1;

        public ARPlacementController Placement => placement;
        public EnemySpawner Spawner => spawner;
        public PlayerHealth PlayerHealth => playerHealth;
        public PlayerWeapon PlayerWeapon => playerWeapon;
        public ProjectilePool PlayerProjectiles => playerProjectiles;
        public ProjectilePool EnemyProjectiles => enemyProjectiles;
        public UIManager UI => ui;
        public Leaderboard Leaderboard { get; private set; }
        public GameSession CurrentSession { get; set; }
        public DifficultySettings[] Difficulties => difficulties;
        public int DifficultyIndex => _difficultyIndex;
        public DifficultySettings SelectedDifficulty => difficulties[_difficultyIndex];
        public string CurrentStateName => _stateMachine.Current?.Name ?? "None";

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (difficulties == null || difficulties.Length == 0) difficulties = DifficultySettings.CreateDefaults();
            _difficultyIndex = Mathf.Clamp(PlayerPrefs.GetInt(DifficultyPrefsKey, 1), 0, difficulties.Length - 1);
            Leaderboard = new Leaderboard();
        }

        private void Start()
        {
            ui.Initialize(this);
            _stateMachine.ChangeState(new MainMenuState(this));
        }

        private void Update() => _stateMachine.Tick(Time.deltaTime);

        // ---- Called by UI and states ------------------------------------------

        public void SelectDifficulty(int index)
        {
            _difficultyIndex = Mathf.Clamp(index, 0, difficulties.Length - 1);
            PlayerPrefs.SetInt(DifficultyPrefsKey, _difficultyIndex);
        }

        /// <summary>Start button: place the arena first if it has not been placed yet.</summary>
        public void StartGame()
        {
            if (placement.IsPlaced) BeginMatch();
            else _stateMachine.ChangeState(new PlacementState(this));
        }

        public void BeginMatch() => _stateMachine.ChangeState(new PlayingState(this));

        public void EndMatch(bool survived) => _stateMachine.ChangeState(new GameOverState(this, survived));

        public void Restart() => BeginMatch();

        public void ReturnToMainMenu() => _stateMachine.ChangeState(new MainMenuState(this));
    }
}
