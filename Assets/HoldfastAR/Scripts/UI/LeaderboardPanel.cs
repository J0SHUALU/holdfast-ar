using System.Collections.Generic;
using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class LeaderboardPanel : UIPanel
    {
        private readonly List<Text> _scores = new List<Text>();
        private readonly List<Text> _details = new List<Text>();
        private readonly List<Image> _rows = new List<Image>();
        private Text _empty;

        protected override void OnBuild()
        {
            UIFactory.Shade(Backdrop, "Shade", UITheme.ShadeTop, UITheme.ShadeMiddle, UITheme.ShadeBottom);
            UIFactory.Image(Backdrop, "Dim", new Color(0f, 0f, 0f, 0.35f)).rectTransform.Stretch();

            Vector2 top = new Vector2(0.5f, 1f);
            UIFactory.Text(Root, "Title", "LEADERBOARD", 100, UITheme.Text).SingleLine(60).Glow(UITheme.AccentDark, 4f)
                .rectTransform.Place(top, new Vector2(1000, 140), new Vector2(0, -140));
            UIFactory.Text(Root, "Subtitle", $"LAST {Leaderboard.MaxEntries} MISSIONS", 30, UITheme.Amber)
                .rectTransform.Place(top, new Vector2(1000, 50), new Vector2(0, -290));

            RectTransform list = UIFactory.Rect("List", Root).Place(new Vector2(0.5f, 0.5f), new Vector2(900, 900), new Vector2(0, 40));
            UIFactory.VerticalLayout(list, 18);
            for (int i = 0; i < Leaderboard.MaxEntries; i++)
            {
                Image row = UIFactory.Image(list, $"Row {i + 1}", UITheme.Tile, UISkin.Panel).Height(160);
                UIFactory.Text(row.transform, "Rank", $"{i + 1}", 64, UITheme.Accent, TextAnchor.MiddleLeft).rectTransform.Stretch(36);
                Text score = UIFactory.Text(row.transform, "Score", "", 56, UITheme.Text, TextAnchor.UpperRight);
                score.rectTransform.Stretch(30);
                Text details = UIFactory.Text(row.transform, "Details", "", 26, UITheme.TextMuted, TextAnchor.LowerRight);
                details.rectTransform.Stretch(30);
                _rows.Add(row);
                _scores.Add(score);
                _details.Add(details);
            }

            _empty = UIFactory.Text(Root, "Empty", "NO MISSIONS YET", 40, UITheme.TextMuted);
            _empty.rectTransform.Place(new Vector2(0.5f, 0.5f), new Vector2(900, 80), new Vector2(0, 40));

            Button back = UIFactory.Button(Root, "BACK", UITheme.ButtonNeutral, Manager.ShowMainMenu, UITheme.BodySize);
            back.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)back.transform).Place(new Vector2(0.5f, 0f), new Vector2(900, 130), new Vector2(0, 70));
        }

        protected override void OnShow()
        {
            IReadOnlyList<SessionRecord> sessions = Game.Leaderboard.Sessions;
            int best = Game.Leaderboard.BestIndex();
            _empty.gameObject.SetActive(sessions.Count == 0);

            for (int i = 0; i < _rows.Count; i++)
            {
                bool has = i < sessions.Count;
                _rows[i].gameObject.SetActive(has);
                if (!has) continue;

                SessionRecord s = sessions[i];
                _scores[i].text = s.score.ToString();
                _scores[i].color = i == best ? UITheme.Amber : UITheme.Text;
                _details[i].text = $"{(s.survived ? "HELD" : "OVERRUN")}  {FormatTime(s.timeSurvived)}  {s.enemiesDefeated} KILLS  {s.difficulty.ToUpperInvariant()}";
            }
        }

        public static string FormatTime(float seconds)
        {
            int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
            return $"{s / 60}:{s % 60:00}";
        }
    }
}
