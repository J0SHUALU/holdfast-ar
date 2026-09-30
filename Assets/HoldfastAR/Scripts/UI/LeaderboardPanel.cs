using System.Collections.Generic;
using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    /// <summary>Shows the latest 5 saved sessions, newest first, with the best one starred.</summary>
    public class LeaderboardPanel : UIPanel
    {
        private readonly List<Text> _rows = new List<Text>();
        private Text _empty;

        protected override void OnBuild()
        {
            UIFactory.Image(Root, "Dim", UITheme.Dim).rectTransform.Stretch();
            RectTransform card = UIFactory.Image(Root, "Card", UITheme.Panel, rounded: true).rectTransform
                .Place(new Vector2(0.5f, 0.5f), new Vector2(960, 1350));
            UIFactory.VerticalLayout(card, 18, 50);

            UIFactory.Text(card, "Title", "LEADERBOARD", UITheme.HeadingSize + 10, UITheme.Accent, style: FontStyle.Bold).Height(110);
            UIFactory.Text(card, "Subtitle", $"Your latest {Leaderboard.MaxEntries} sessions", UITheme.SmallSize, UITheme.TextMuted).Height(55);

            for (int i = 0; i < Leaderboard.MaxEntries; i++)
            {
                Image rowBg = UIFactory.Image(card, $"Row {i + 1}", UITheme.ButtonNeutral, rounded: true).Height(140);
                Text rowText = UIFactory.Text(rowBg.transform, "Text", "", UITheme.SmallSize, UITheme.Text, TextAnchor.MiddleLeft);
                rowText.rectTransform.Stretch(20);
                _rows.Add(rowText);
            }

            _empty = UIFactory.Text(card, "Empty", "No sessions yet. Go survive!", UITheme.BodySize, UITheme.TextMuted).Height(80);
            UIFactory.Button(card, "BACK", UITheme.AccentDark, Manager.ShowMainMenu).Height(130);
        }

        protected override void OnShow()
        {
            IReadOnlyList<SessionRecord> sessions = Game.Leaderboard.Sessions;
            int best = Game.Leaderboard.BestIndex();
            _empty.gameObject.SetActive(sessions.Count == 0);

            for (int i = 0; i < _rows.Count; i++)
            {
                Transform row = _rows[i].transform.parent;
                bool has = i < sessions.Count;
                row.gameObject.SetActive(has);
                if (!has) continue;

                SessionRecord s = sessions[i];
                string bestTag = i == best ? "   BEST" : "";
                string result = s.survived ? "SURVIVED" : "FELL";
                _rows[i].text = $"<b>#{i + 1}   {s.score} pts</b>{bestTag}\n" +
                                $"<size=28>{s.date}  •  {s.difficulty}  •  {s.enemiesDefeated} kills  •  " +
                                $"{FormatTime(s.timeSurvived)}  •  {result}</size>";
                _rows[i].color = i == best ? UITheme.Gold : UITheme.Text;
            }
        }

        public static string FormatTime(float seconds)
        {
            int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
            return $"{s / 60}:{s % 60:00}";
        }
    }
}
