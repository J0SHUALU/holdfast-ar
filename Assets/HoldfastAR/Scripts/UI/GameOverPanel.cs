using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    /// <summary>End-game summary: result, final score, enemies defeated, time survived, Restart / Main Menu.</summary>
    public class GameOverPanel : UIPanel
    {
        private Text _title;
        private Text _subtitle;
        private Text _score;
        private Text _kills;
        private Text _time;
        private Text _difficulty;
        private Text _best;

        protected override void OnBuild()
        {
            UIFactory.Image(Root, "Dim", UITheme.Dim).rectTransform.Stretch();
            RectTransform card = UIFactory.Image(Root, "Card", UITheme.Card, rounded: true).rectTransform
                .Place(new Vector2(0.5f, 0.5f), new Vector2(900, 1320));
            UIFactory.VerticalLayout(card, 22, 60);

            _title = UIFactory.Text(card, "Title", "", UITheme.TitleSize - 10, UITheme.Accent, style: FontStyle.Bold)
                .SingleLine(UITheme.HeadingSize).Height(130);
            _subtitle = UIFactory.Text(card, "Subtitle", "", UITheme.BodySize, UITheme.TextMuted).Height(70);

            _score = StatRow(card, "FINAL SCORE");
            _kills = StatRow(card, "ENEMIES DEFEATED");
            _time = StatRow(card, "TIME SURVIVED");
            _difficulty = StatRow(card, "DIFFICULTY");

            _best = UIFactory.Text(card, "Best", "", UITheme.BodySize, UITheme.Gold, style: FontStyle.Bold).Height(70);

            UIFactory.Button(card, "RESTART", UITheme.AccentDark, Game.Restart, UITheme.HeadingSize - 6).Height(150);
            UIFactory.Button(card, "MAIN MENU", UITheme.ButtonNeutral, Game.ReturnToMainMenu).Height(130);
        }

        private static Text StatRow(RectTransform parent, string label)
        {
            Image bg = UIFactory.Image(parent, label, UITheme.ButtonNeutral, rounded: true).Height(115);
            UIFactory.Text(bg.transform, "Label", label, UITheme.SmallSize, UITheme.TextMuted, TextAnchor.MiddleLeft, FontStyle.Bold)
                .rectTransform.Stretch(30);
            Text value = UIFactory.Text(bg.transform, "Value", "", UITheme.HeadingSize - 10, UITheme.Text, TextAnchor.MiddleRight, FontStyle.Bold);
            value.rectTransform.Stretch(30);
            return value;
        }

        public void SetResult(SessionRecord record, bool isBest)
        {
            _title.text = record.survived ? "YOU HELD FAST" : "OVERRUN";
            _title.color = record.survived ? UITheme.Success : UITheme.Danger;
            _subtitle.text = record.survived ? "You survived until the timer ran out (+survival bonus)" : "The swarm broke through your defences";
            _score.text = record.score.ToString();
            _kills.text = record.enemiesDefeated.ToString();
            _time.text = LeaderboardPanel.FormatTime(record.timeSurvived);
            _difficulty.text = record.difficulty;
            _best.text = isBest ? "NEW BEST OF YOUR LAST 5!" : "";
        }
    }
}
