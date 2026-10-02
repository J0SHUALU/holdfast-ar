using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class GameOverPanel : UIPanel
    {
        private Text _title;
        private Text _subtitle;
        private Text _score;
        private Text _kills;
        private Text _time;

        protected override void OnBuild()
        {
            UIFactory.Shade(Backdrop, "Shade", UITheme.ShadeTop, UITheme.ShadeMiddle, UITheme.ShadeBottom);
            UIFactory.Image(Backdrop, "Dim", new Color(0f, 0f, 0f, 0.35f)).rectTransform.Stretch();

            Vector2 top = new Vector2(0.5f, 1f);
            _title = UIFactory.Text(Root, "Title", "", 130, UITheme.Accent).SingleLine(70).Glow(UITheme.AccentDark, 4f);
            _title.rectTransform.Place(top, new Vector2(1000, 170), new Vector2(0, -170));
            _subtitle = UIFactory.Text(Root, "Subtitle", "", 30, UITheme.TextMuted).SingleLine(20);
            _subtitle.rectTransform.Place(top, new Vector2(1000, 60), new Vector2(0, -350));

            RectTransform stats = UIFactory.Rect("Stats", Root).Place(new Vector2(0.5f, 0.5f), new Vector2(900, 560), new Vector2(0, 80));
            UIFactory.VerticalLayout(stats, 22);
            _score = StatRow(stats, "FINAL SCORE", UITheme.Amber);
            _kills = StatRow(stats, "ENEMIES DEFEATED", UITheme.Text);
            _time = StatRow(stats, "TIME SURVIVED", UITheme.Text);

            Vector2 bottom = new Vector2(0.5f, 0f);
            Button restart = UIFactory.Button(Root, "RESTART", UITheme.Accent, Game.Restart, UITheme.HeadingSize);
            restart.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)restart.transform).Place(bottom, new Vector2(900, 190), new Vector2(0, 230));

            Button menu = UIFactory.Button(Root, "MAIN MENU", UITheme.ButtonNeutral, Game.ReturnToMainMenu, UITheme.BodySize);
            menu.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)menu.transform).Place(bottom, new Vector2(900, 130), new Vector2(0, 70));
        }

        private static Text StatRow(RectTransform parent, string label, Color valueColor)
        {
            Image bg = UIFactory.Image(parent, label, UITheme.Tile, UISkin.Panel).Height(170);
            UIFactory.Text(bg.transform, "Label", label, 30, UITheme.TextMuted, TextAnchor.MiddleLeft).rectTransform.Stretch(40);
            Text value = UIFactory.Text(bg.transform, "Value", "", 72, valueColor, TextAnchor.MiddleRight);
            value.rectTransform.Stretch(40);
            return value;
        }

        public void SetResult(SessionRecord record)
        {
            _title.text = record.survived ? "DOME HELD" : "OVERRUN";
            _title.color = record.survived ? UITheme.Success : UITheme.Danger;
            _subtitle.text = record.survived ? "EVAC HAS LANDED. THE COLONY IS SAFE" : "THE RAIDERS BROKE THROUGH THE DOME";
            _score.text = record.score.ToString();
            _kills.text = record.enemiesDefeated.ToString();
            _time.text = LeaderboardPanel.FormatTime(record.timeSurvived);
        }
    }
}
