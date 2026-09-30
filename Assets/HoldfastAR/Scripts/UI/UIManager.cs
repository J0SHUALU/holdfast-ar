using System.Collections.Generic;
using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace HoldfastAR.UI
{
    /// <summary>
    /// Builds the canvas and all screens, and switches which one is visible.
    /// Every screen derives from UIPanel, so UIManager handles them polymorphically.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private readonly List<UIPanel> _panels = new List<UIPanel>();
        private RectTransform _safeArea;
        private Rect _lastSafeArea;

        public MainMenuPanel MainMenu { get; private set; }
        public LeaderboardPanel Leaderboard { get; private set; }
        public PlacementPanel Placement { get; private set; }
        public HudPanel Hud { get; private set; }
        public GameOverPanel GameOver { get; private set; }

        private GameManager _game;

        public void Initialize(GameManager game)
        {
            _game = game;
            EnsureEventSystem();

            var canvasGo = new GameObject("Holdfast UI Canvas", typeof(RectTransform));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            _safeArea = UIFactory.Rect("Safe Area", canvasGo.transform).Stretch();
            ApplySafeArea();

            MainMenu = Add(new MainMenuPanel());
            Leaderboard = Add(new LeaderboardPanel());
            Placement = Add(new PlacementPanel());
            Hud = Add(new HudPanel());
            GameOver = Add(new GameOverPanel());
        }

        private T Add<T>(T panel) where T : UIPanel
        {
            panel.Build(_safeArea, this, _game);
            _panels.Add(panel);
            return panel;
        }

        private void Update()
        {
            ApplySafeArea();
            foreach (UIPanel p in _panels)
                if (p.IsVisible) p.Tick(Time.deltaTime);
        }

        private void ShowOnly(UIPanel panel)
        {
            foreach (UIPanel p in _panels)
                if (p != panel) p.Hide();
            panel.Show();
        }

        public void ShowMainMenu() => ShowOnly(MainMenu);
        public void ShowLeaderboard() => ShowOnly(Leaderboard);
        public void ShowPlacement() => ShowOnly(Placement);
        public void ShowHud() => ShowOnly(Hud);

        public void ShowGameOver(SessionRecord record)
        {
            bool isBest = _game.Leaderboard.Sessions.Count > 0 && _game.Leaderboard.BestIndex() == 0;
            GameOver.SetResult(record, isBest);
            ShowOnly(GameOver);
        }

        private void ApplySafeArea()
        {
            if (_safeArea == null) return;
            Rect safe = Screen.safeArea;
            if (safe == _lastSafeArea || Screen.width <= 0 || Screen.height <= 0) return;
            _lastSafeArea = safe;
            _safeArea.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            _safeArea.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            _safeArea.offsetMin = _safeArea.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            go.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
#else
            go.AddComponent<StandaloneInputModule>();
#endif
        }
    }
}
