using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    /// <summary>Start menu: title, difficulty selection, Start and Leaderboard buttons.</summary>
    public class MainMenuPanel : UIPanel
    {
        private Button[] _difficultyButtons;
        private Text _difficultyInfo;

        protected override void OnBuild()
        {
            UIFactory.Image(Root, "Dim", UITheme.Dim).rectTransform.Stretch();

            RectTransform card = UIFactory.Image(Root, "Card", UITheme.Panel, rounded: true).rectTransform
                .Place(new Vector2(0.5f, 0.5f), new Vector2(900, 1300));
            UIFactory.VerticalLayout(card, 28, 60);

            UIFactory.Text(card, "Title", "HOLDFAST AR", UITheme.TitleSize, UITheme.Accent, style: FontStyle.Bold)
                .SingleLine(UITheme.HeadingSize).Height(140);
            UIFactory.Text(card, "Subtitle", "Hold your ground. Survive the swarm.", UITheme.BodySize, UITheme.TextMuted).Height(70);
            UIFactory.Text(card, "DifficultyLabel", "DIFFICULTY", UITheme.SmallSize, UITheme.TextMuted, style: FontStyle.Bold).Height(60);

            RectTransform row = UIFactory.Rect("DifficultyRow", card).Height(120);
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 20;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandWidth = h.childForceExpandHeight = true;

            DifficultySettings[] levels = Game.Difficulties;
            _difficultyButtons = new Button[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                int index = i;
                _difficultyButtons[i] = UIFactory.Button(row, levels[i].displayName.ToUpperInvariant(), UITheme.ButtonNeutral,
                    () => SelectDifficulty(index), UITheme.SmallSize + 4);
            }

            _difficultyInfo = UIFactory.Text(card, "DifficultyInfo", "", UITheme.SmallSize, UITheme.Text).Height(90);

            UIFactory.Button(card, "START", UITheme.AccentDark, Game.StartGame, UITheme.HeadingSize).Height(170);
            UIFactory.Button(card, "LEADERBOARD", UITheme.ButtonNeutral, Manager.ShowLeaderboard).Height(130);

            UIFactory.Text(card, "Credit", "by Joshua Chukwuebuka Moses", UITheme.SmallSize, UITheme.TextMuted,
                style: FontStyle.Italic).Height(80);
        }

        protected override void OnShow() => Refresh();

        private void SelectDifficulty(int index)
        {
            Game.SelectDifficulty(index);
            Refresh();
        }

        private void Refresh()
        {
            for (int i = 0; i < _difficultyButtons.Length; i++)
            {
                bool selected = i == Game.DifficultyIndex;
                _difficultyButtons[i].image.color = selected ? UITheme.AccentDark : UITheme.ButtonNeutral;
                _difficultyButtons[i].GetComponentInChildren<Text>().color = selected ? UITheme.Accent : UITheme.Text;
            }

            DifficultySettings d = Game.SelectedDifficulty;
            _difficultyInfo.text = $"Survive {d.matchDuration:0}s  •  {d.playerMaxHealth:0} HP  •  up to {d.maxAliveEnemies} enemies\n" +
                                   $"Enemy speed x{d.enemySpeedMultiplier:0.0}  •  Score x{d.scoreMultiplier:0.0}";
        }
    }
}
