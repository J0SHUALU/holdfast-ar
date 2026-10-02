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
    public class UIManager : MonoBehaviour
    {
        private readonly List<UIPanel> _panels = new List<UIPanel>();
        private const float FrameWidth = 1080f;
        private const float FrameHeight = 1920f;

        private RectTransform _backdrop;
        private RectTransform _safeArea;
        private RectTransform _frame;
        private Rect _lastSafeArea;

        public MainMenuPanel MainMenu { get; private set; }
        public LeaderboardPanel Leaderboard { get; private set; }
        public SettingsPanel Settings { get; private set; }
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
            scaler.matchWidthOrHeight = 0f;
            canvasGo.AddComponent<GraphicRaycaster>();

            _backdrop = UIFactory.Rect("Backdrops", canvasGo.transform).Stretch();
            _safeArea = UIFactory.Rect("Safe Area", canvasGo.transform).Stretch();
            _frame = UIFactory.Rect("Portrait Frame", _safeArea).Place(new Vector2(0.5f, 0.5f), new Vector2(FrameWidth, FrameHeight));
            ApplySafeArea();

            MainMenu = Add(new MainMenuPanel(), _frame);
            Leaderboard = Add(new LeaderboardPanel(), _frame);
            Settings = Add(new SettingsPanel(), _frame);
            Placement = Add(new PlacementPanel(), _frame);
            Hud = Add(new HudPanel(), _frame);
            GameOver = Add(new GameOverPanel(), _frame);
        }

        private T Add<T>(T panel, RectTransform parent) where T : UIPanel
        {
            panel.Build(parent, _backdrop, this, _game);
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
        public void ShowSettings() => ShowOnly(Settings);
        public void ShowPlacement() => ShowOnly(Placement);
        public void ShowHud() => ShowOnly(Hud);

        public void ShowGameOver(SessionRecord record)
        {
            GameOver.SetResult(record);
            ShowOnly(GameOver);
        }

        private void ApplySafeArea()
        {
            if (_safeArea == null) return;
            Rect safe = Screen.safeArea;
            if (safe != _lastSafeArea && Screen.width > 0 && Screen.height > 0)
            {
                _lastSafeArea = safe;
                _safeArea.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
                _safeArea.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
                _safeArea.offsetMin = _safeArea.offsetMax = Vector2.zero;
            }

            Rect area = _safeArea.rect;
            if (area.width <= 0f || area.height <= 0f) return;
            float scale = Mathf.Min(area.width / FrameWidth, area.height / FrameHeight);
            _frame.localScale = new Vector3(scale, scale, 1f);
            _frame.sizeDelta = new Vector2(FrameWidth, Mathf.Max(FrameHeight, area.height / scale));
        }

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;
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
