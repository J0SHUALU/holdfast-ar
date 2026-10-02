using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class MainMenuPanel : UIPanel
    {
        private static readonly Color ChipIdle = new Color(0.14f, 0.11f, 0.22f, 0.9f);

        private Button[] _difficultyButtons;

        protected override void OnBuild()
        {
            UIFactory.Shade(Backdrop, "Shade", UITheme.ShadeTop, UITheme.ShadeMiddle, UITheme.ShadeBottom);

            Vector2 top = new Vector2(0.5f, 1f);
            RectTransform frame = UIFactory.Image(Root, "EmblemFrame", UITheme.Danger, UISkin.Panel).rectTransform
                .Place(top, new Vector2(284, 284), new Vector2(0, -98));
            Image emblem = UIFactory.Image(frame, "Emblem", Color.white);
            emblem.sprite = Resources.Load<Sprite>("UI/AppIcon");
            emblem.preserveAspect = true;
            emblem.rectTransform.Stretch(12);

            UIFactory.Text(Root, "Title", "HOLDFAST <color=#FFBD38>AR</color>", 140, UITheme.Text)
                .SingleLine(80).Glow(UITheme.AccentDark, 4f)
                .rectTransform.Place(top, new Vector2(1000, 170), new Vector2(0, -400));

            UIFactory.Image(Root, "Rule", UITheme.Accent).rectTransform.Place(top, new Vector2(240, 6), new Vector2(0, -585));

            UIFactory.Text(Root, "Tagline", "DEFEND THE DOME UNTIL EVAC ARRIVES", 30, UITheme.TextMuted)
                .SingleLine(20).rectTransform.Place(top, new Vector2(1000, 60), new Vector2(0, -615));

            Vector2 bottom = new Vector2(0.5f, 0f);
            UIFactory.Text(Root, "ThreatLabel", "THREAT LEVEL", 28, UITheme.Amber, TextAnchor.MiddleLeft)
                .rectTransform.Place(bottom, new Vector2(900, 50), new Vector2(0, 610));

            RectTransform row = UIFactory.Image(Root, "DifficultyRow", UITheme.Tile, UISkin.Panel)
                .rectTransform.Place(bottom, new Vector2(900, 140), new Vector2(0, 460));
            var h = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.padding = new RectOffset(14, 14, 14, 14);
            h.spacing = 12;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandWidth = h.childForceExpandHeight = true;

            DifficultySettings[] levels = Game.Difficulties;
            _difficultyButtons = new Button[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                int index = i;
                _difficultyButtons[i] = UIFactory.Button(row, levels[i].displayName.ToUpperInvariant(), ChipIdle,
                    () => SelectDifficulty(index), UITheme.SmallSize);
            }

            Button start = UIFactory.Button(Root, "START MISSION", UITheme.Accent, Game.StartGame, UITheme.HeadingSize);
            start.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)start.transform).Place(bottom, new Vector2(900, 190), new Vector2(0, 230));

            Button board = UIFactory.Button(Root, "LEADERBOARD", UITheme.ButtonNeutral, Manager.ShowLeaderboard, UITheme.SmallSize);
            board.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)board.transform).Place(bottom, new Vector2(444, 130), new Vector2(-228, 70));

            Button settings = UIFactory.Button(Root, "SETTINGS", UITheme.ButtonNeutral, Manager.ShowSettings, UITheme.SmallSize);
            settings.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)settings.transform).Place(bottom, new Vector2(444, 130), new Vector2(228, 70));
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
                _difficultyButtons[i].image.color = selected ? UITheme.Amber : ChipIdle;
                _difficultyButtons[i].GetComponentInChildren<Text>().color = selected ? UITheme.Ink : UITheme.TextMuted;
            }
        }
    }
}
