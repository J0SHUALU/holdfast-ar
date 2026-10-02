using HoldfastAR.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class HudPanel : UIPanel
    {
        private RectTransform _healthFill;
        private Image _healthFillImage;
        private Text _healthText;
        private Text _timerText;
        private Text _scoreText;
        private readonly SpawnMarkers _spawnMarkers = new SpawnMarkers();
        private Camera _camera;
        private Image _damageFlash;
        private Text _popup;
        private float _flashAlpha;
        private float _popupTime;
        private float _scorePunch;

        protected override void OnBuild()
        {
            _damageFlash = UIFactory.Image(Backdrop, "DamageFlash", new Color(1f, 0f, 0f, 0f));
            _damageFlash.rectTransform.Stretch();

            RectTransform healthBox = UIFactory.Image(Root, "HealthBox", UITheme.Panel, UISkin.Panel).rectTransform
                .Place(new Vector2(0f, 1f), new Vector2(360, 150), new Vector2(36, -50));
            UIFactory.Text(healthBox, "Label", "HEALTH", UITheme.SmallSize - 4, UITheme.TextMuted, TextAnchor.UpperLeft, FontStyle.Bold)
                .rectTransform.Stretch(22);
            RectTransform barBg = UIFactory.Image(healthBox, "BarBg", new Color(0f, 0f, 0f, 0.65f), UISkin.Bar).rectTransform
                .Place(new Vector2(0f, 0f), new Vector2(210, 40), new Vector2(22, 28));
            _healthFillImage = UIFactory.Image(barBg, "Fill", UITheme.Success, UISkin.Bar);
            _healthFill = _healthFillImage.rectTransform;
            _healthFill.anchorMin = Vector2.zero;
            _healthFill.anchorMax = Vector2.one;
            _healthFill.offsetMin = _healthFill.offsetMax = Vector2.zero;
            _healthText = UIFactory.Text(healthBox, "Value", "100", UITheme.BodySize, UITheme.Text, TextAnchor.LowerRight, FontStyle.Bold);
            _healthText.rectTransform.Stretch(22);

            RectTransform timerBox = UIFactory.Image(Root, "TimerBox", UITheme.Panel, UISkin.Panel).rectTransform
                .Place(new Vector2(0.5f, 1f), new Vector2(240, 150), new Vector2(0, -50));
            UIFactory.Text(timerBox, "Label", "TIME", UITheme.SmallSize - 6, UITheme.TextMuted, TextAnchor.UpperCenter, FontStyle.Bold)
                .rectTransform.Stretch(14);
            _timerText = UIFactory.Text(timerBox, "Value", "0:00", UITheme.HeadingSize, UITheme.Text, TextAnchor.LowerCenter, FontStyle.Bold);
            _timerText.rectTransform.Stretch(10);

            RectTransform scoreBox = UIFactory.Image(Root, "ScoreBox", UITheme.Panel, UISkin.Panel).rectTransform
                .Place(new Vector2(1f, 1f), new Vector2(300, 150), new Vector2(-36, -50));
            _scoreText = UIFactory.Text(scoreBox, "Score", "0", UITheme.HeadingSize, UITheme.Amber, TextAnchor.LowerRight);
            _scoreText.rectTransform.Stretch(18);
            UIFactory.Text(scoreBox, "Label", "SCORE", UITheme.SmallSize - 6, UITheme.TextMuted, TextAnchor.UpperLeft)
                .rectTransform.Stretch(18);

            BuildCrosshair();
            _spawnMarkers.Build(Root);

            _popup = UIFactory.Text(Root, "Popup", "", UITheme.HeadingSize, UITheme.Gold, style: FontStyle.Bold);
            _popup.rectTransform.Place(new Vector2(0.5f, 0.5f), new Vector2(600, 120), new Vector2(0, 140));

            Image fire = UIFactory.Image(Root, "FireButton", new Color(1f, 0.3f, 0.22f, 0.92f), UISkin.Button);
            fire.raycastTarget = true;
            fire.rectTransform.Place(new Vector2(1f, 0f), new Vector2(250, 250), new Vector2(-40, 90));
            UIFactory.Text(fire.transform, "Label", "FIRE", UITheme.HeadingSize, UITheme.Text, style: FontStyle.Bold).rectTransform.Stretch();
            var hold = fire.gameObject.AddComponent<HoldButton>();
            hold.HeldChanged += held => Game.PlayerWeapon.TriggerHeld = held;
        }

        private void BuildCrosshair()
        {
            UIFactory.Image(Root, "Crosshair", UITheme.Accent, UISkin.Crosshair).rectTransform
                .Place(new Vector2(0.5f, 0.5f), new Vector2(110, 110));
        }

        protected override void OnShow()
        {
            GameEvents.PlayerHealthChanged += OnHealthChanged;
            GameEvents.PlayerDamaged += OnDamaged;
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.TimeRemainingChanged += OnTimeChanged;
            GameEvents.EnemyKilled += OnEnemyKilled;
            GameEvents.EnemySpawned += _spawnMarkers.Track;
            _camera = Game.PlayerHealth.GetComponent<Camera>();
            _flashAlpha = 0f;
            _popupTime = 0f;
            _popup.text = "";
        }

        protected override void OnHide()
        {
            GameEvents.PlayerHealthChanged -= OnHealthChanged;
            GameEvents.PlayerDamaged -= OnDamaged;
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.TimeRemainingChanged -= OnTimeChanged;
            GameEvents.EnemyKilled -= OnEnemyKilled;
            GameEvents.EnemySpawned -= _spawnMarkers.Track;
            _spawnMarkers.Clear();
        }

        public override void Tick(float deltaTime)
        {
            _flashAlpha = Mathf.MoveTowards(_flashAlpha, 0f, deltaTime * 1.6f);
            _damageFlash.color = new Color(1f, 0f, 0f, _flashAlpha);
            _spawnMarkers.Tick(deltaTime, _camera);

            _scorePunch = Mathf.MoveTowards(_scorePunch, 0f, deltaTime * 4f);
            _scoreText.rectTransform.localScale = Vector3.one * (1f + 0.25f * _scorePunch);

            if (_popupTime > 0f)
            {
                _popupTime -= deltaTime;
                float a = Mathf.Clamp01(_popupTime / 0.8f);
                _popup.color = new Color(UITheme.Gold.r, UITheme.Gold.g, UITheme.Gold.b, a);
                _popup.rectTransform.anchoredPosition = new Vector2(0, 140 + (1f - a) * 80f);
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            float t = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            _healthFill.anchorMax = new Vector2(t, 1f);
            _healthFillImage.color = t > 0.5f ? Color.Lerp(UITheme.Amber, UITheme.Success, (t - 0.5f) * 2f) : Color.Lerp(UITheme.Danger, UITheme.Amber, t * 2f);
            _healthText.text = Mathf.CeilToInt(current).ToString();
        }

        private void OnDamaged(float amount) => _flashAlpha = Mathf.Clamp(_flashAlpha + 0.25f + amount / 60f, 0f, 0.6f);

        private void OnScoreChanged(int score)
        {
            _scoreText.text = score.ToString();
            if (score > 0) _scorePunch = 1f;
        }

        private void OnTimeChanged(float seconds)
        {
            _timerText.text = LeaderboardPanel.FormatTime(seconds);
            _timerText.color = seconds <= 10f ? UITheme.Danger : UITheme.Text;
        }

        private void OnEnemyKilled(int scoreValue, Vector3 position)
        {
            int shown = Mathf.RoundToInt(scoreValue * Game.SelectedDifficulty.scoreMultiplier);
            _popup.text = $"+{shown}";
            _popupTime = 0.8f;
        }
    }
}
