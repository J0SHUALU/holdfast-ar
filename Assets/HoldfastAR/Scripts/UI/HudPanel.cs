using HoldfastAR.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    /// <summary>
    /// In-game HUD: health bar, score, kills, time remaining, crosshair and FIRE button.
    /// Updates purely by listening to GameEvents (Observer) - it never polls gameplay code.
    /// Also shows damage feedback (red screen flash) and "+score" popups.
    /// </summary>
    public class HudPanel : UIPanel
    {
        private RectTransform _healthFill;
        private Image _healthFillImage;
        private Text _healthText;
        private Text _timerText;
        private Text _scoreText;
        private Text _killsText;
        private Image _damageFlash;
        private Text _popup;
        private float _flashAlpha;
        private float _popupTime;
        private float _scorePunch;

        protected override void OnBuild()
        {
            _damageFlash = UIFactory.Image(Root, "DamageFlash", new Color(1f, 0f, 0f, 0f));
            _damageFlash.rectTransform.Stretch();

            // Health (top-left)
            RectTransform healthBox = UIFactory.Image(Root, "HealthBox", UITheme.Panel, rounded: true).rectTransform
                .Place(new Vector2(0f, 1f), new Vector2(460, 150), new Vector2(40, -60));
            UIFactory.Text(healthBox, "Label", "HEALTH", UITheme.SmallSize - 4, UITheme.TextMuted, TextAnchor.UpperLeft, FontStyle.Bold)
                .rectTransform.Stretch(22);
            RectTransform barBg = UIFactory.Image(healthBox, "BarBg", new Color(0f, 0f, 0f, 0.6f), rounded: true).rectTransform
                .Place(new Vector2(0f, 0f), new Vector2(300, 46), new Vector2(22, 26));
            _healthFillImage = UIFactory.Image(barBg, "Fill", UITheme.Success, rounded: true);
            _healthFill = _healthFillImage.rectTransform;
            _healthFill.anchorMin = Vector2.zero;
            _healthFill.anchorMax = Vector2.one;
            _healthFill.offsetMin = _healthFill.offsetMax = Vector2.zero;
            _healthText = UIFactory.Text(healthBox, "Value", "100", UITheme.BodySize, UITheme.Text, TextAnchor.LowerRight, FontStyle.Bold);
            _healthText.rectTransform.Stretch(22);

            // Timer (top-centre)
            RectTransform timerBox = UIFactory.Image(Root, "TimerBox", UITheme.Panel, rounded: true).rectTransform
                .Place(new Vector2(0.5f, 1f), new Vector2(260, 150), new Vector2(0, -60));
            UIFactory.Text(timerBox, "Label", "TIME", UITheme.SmallSize - 6, UITheme.TextMuted, TextAnchor.UpperCenter, FontStyle.Bold)
                .rectTransform.Stretch(14);
            _timerText = UIFactory.Text(timerBox, "Value", "0:00", UITheme.HeadingSize, UITheme.Text, TextAnchor.LowerCenter, FontStyle.Bold);
            _timerText.rectTransform.Stretch(10);

            // Score + kills (top-right)
            RectTransform scoreBox = UIFactory.Image(Root, "ScoreBox", UITheme.Panel, rounded: true).rectTransform
                .Place(new Vector2(1f, 1f), new Vector2(300, 150), new Vector2(-40, -60));
            _scoreText = UIFactory.Text(scoreBox, "Score", "0", UITheme.HeadingSize, UITheme.Accent, TextAnchor.UpperRight, FontStyle.Bold);
            _scoreText.rectTransform.Stretch(18);
            _killsText = UIFactory.Text(scoreBox, "Kills", "0 KILLS", UITheme.SmallSize - 4, UITheme.TextMuted, TextAnchor.LowerRight, FontStyle.Bold);
            _killsText.rectTransform.Stretch(18);

            BuildCrosshair();

            _popup = UIFactory.Text(Root, "Popup", "", UITheme.HeadingSize, UITheme.Gold, style: FontStyle.Bold);
            _popup.rectTransform.Place(new Vector2(0.5f, 0.5f), new Vector2(600, 120), new Vector2(0, 140));

            // FIRE button (bottom-right, thumb-friendly)
            Image fire = UIFactory.Image(Root, "FireButton", new Color(0.9f, 0.2f, 0.15f, 0.85f), rounded: true);
            fire.raycastTarget = true;
            fire.rectTransform.Place(new Vector2(1f, 0f), new Vector2(330, 330), new Vector2(-60, 160));
            UIFactory.Text(fire.transform, "Label", "FIRE", UITheme.HeadingSize, UITheme.Text, style: FontStyle.Bold).rectTransform.Stretch();
            var hold = fire.gameObject.AddComponent<HoldButton>();
            hold.HeldChanged += held => Game.PlayerWeapon.TriggerHeld = held;
        }

        private void BuildCrosshair()
        {
            RectTransform cross = UIFactory.Rect("Crosshair", Root).Place(new Vector2(0.5f, 0.5f), new Vector2(120, 120));
            Vector2[] offsets = { new Vector2(0, 34), new Vector2(0, -34), new Vector2(34, 0), new Vector2(-34, 0) };
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2 size = i < 2 ? new Vector2(8, 30) : new Vector2(30, 8);
                UIFactory.Image(cross, "Tick", UITheme.Accent).rectTransform.Place(new Vector2(0.5f, 0.5f), size, offsets[i]).pivot = new Vector2(0.5f, 0.5f);
            }
            UIFactory.Image(cross, "Dot", UITheme.Text, rounded: true).rectTransform.Place(new Vector2(0.5f, 0.5f), new Vector2(12, 12));
        }

        protected override void OnShow()
        {
            GameEvents.PlayerHealthChanged += OnHealthChanged;
            GameEvents.PlayerDamaged += OnDamaged;
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.KillsChanged += OnKillsChanged;
            GameEvents.TimeRemainingChanged += OnTimeChanged;
            GameEvents.EnemyKilled += OnEnemyKilled;
            _flashAlpha = 0f;
            _popupTime = 0f;
            _popup.text = "";
        }

        protected override void OnHide()
        {
            GameEvents.PlayerHealthChanged -= OnHealthChanged;
            GameEvents.PlayerDamaged -= OnDamaged;
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.KillsChanged -= OnKillsChanged;
            GameEvents.TimeRemainingChanged -= OnTimeChanged;
            GameEvents.EnemyKilled -= OnEnemyKilled;
        }

        public override void Tick(float deltaTime)
        {
            _flashAlpha = Mathf.MoveTowards(_flashAlpha, 0f, deltaTime * 1.6f);
            _damageFlash.color = new Color(1f, 0f, 0f, _flashAlpha);

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
            _healthFillImage.color = Color.Lerp(UITheme.Danger, UITheme.Success, t);
            _healthText.text = Mathf.CeilToInt(current).ToString();
        }

        private void OnDamaged(float amount) => _flashAlpha = Mathf.Clamp(_flashAlpha + 0.25f + amount / 60f, 0f, 0.6f);

        private void OnScoreChanged(int score)
        {
            _scoreText.text = score.ToString();
            if (score > 0) _scorePunch = 1f;
        }

        private void OnKillsChanged(int kills) => _killsText.text = kills == 1 ? "1 KILL" : $"{kills} KILLS";

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
