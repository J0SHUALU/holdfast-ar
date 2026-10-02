using System;
using HoldfastAR.Data;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class SettingsPanel : UIPanel
    {
        private static readonly Color ChipIdle = new Color(0.14f, 0.11f, 0.22f, 0.9f);
        private static readonly string[] OnOff = { "OFF", "ON" };

        private Button[] _sound;
        private Button[] _music;
        private Button[] _vibration;
        private Button[] _markers;

        protected override void OnBuild()
        {
            UIFactory.Shade(Backdrop, "Shade", UITheme.ShadeTop, UITheme.ShadeMiddle, UITheme.ShadeBottom);
            UIFactory.Image(Backdrop, "Dim", new Color(0f, 0f, 0f, 0.35f)).rectTransform.Stretch();

            Vector2 top = new Vector2(0.5f, 1f);
            UIFactory.Text(Root, "Title", "SETTINGS", 100, UITheme.Text).SingleLine(60).Glow(UITheme.AccentDark, 4f)
                .rectTransform.Place(top, new Vector2(1000, 140), new Vector2(0, -140));
            UIFactory.Text(Root, "Subtitle", "MISSION CONTROL", 30, UITheme.Amber)
                .rectTransform.Place(top, new Vector2(1000, 50), new Vector2(0, -290));

            RectTransform list = UIFactory.Rect("Options", Root).Place(new Vector2(0.5f, 0.5f), new Vector2(900, 920), new Vector2(0, 30));
            UIFactory.VerticalLayout(list, 26);

            _sound = Row(list, "SOUND EFFECTS", GameSettings.VolumeLabels, i => GameSettings.SoundLevel = i);
            _music = Row(list, "MUSIC", GameSettings.VolumeLabels, i => GameSettings.MusicLevel = i);
            _vibration = Row(list, "VIBRATION", OnOff, i => GameSettings.Vibration = i == 1);
            _markers = Row(list, "SPAWN MARKERS", OnOff, i => GameSettings.SpawnMarkers = i == 1);

            Button back = UIFactory.Button(Root, "BACK", UITheme.ButtonNeutral, Manager.ShowMainMenu, UITheme.BodySize);
            back.GetComponentInChildren<Text>().color = UITheme.Text;
            ((RectTransform)back.transform).Place(new Vector2(0.5f, 0f), new Vector2(900, 130), new Vector2(0, 70));
        }

        private Button[] Row(RectTransform list, string label, string[] options, Action<int> onSelect)
        {
            Image tile = UIFactory.Image(list, label, UITheme.Tile, UISkin.Panel).Height(210);
            UIFactory.Text(tile.transform, "Label", label, 30, UITheme.Amber, TextAnchor.UpperLeft)
                .rectTransform.Stretch(28);

            RectTransform chips = UIFactory.Rect("Choices", tile.transform);
            chips.anchorMin = new Vector2(0f, 0f);
            chips.anchorMax = new Vector2(1f, 0f);
            chips.pivot = new Vector2(0.5f, 0f);
            chips.sizeDelta = new Vector2(-40f, 104f);
            chips.anchoredPosition = new Vector2(0f, 22f);
            var h = chips.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 12;
            h.childControlWidth = h.childControlHeight = true;
            h.childForceExpandWidth = h.childForceExpandHeight = true;

            var buttons = new Button[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                int index = i;
                buttons[i] = UIFactory.Button(chips, options[i], ChipIdle, () =>
                {
                    onSelect(index);
                    Refresh();
                }, UITheme.SmallSize);
            }
            return buttons;
        }

        protected override void OnShow() => Refresh();

        private void Refresh()
        {
            Highlight(_sound, GameSettings.SoundLevel);
            Highlight(_music, GameSettings.MusicLevel);
            Highlight(_vibration, GameSettings.Vibration ? 1 : 0);
            Highlight(_markers, GameSettings.SpawnMarkers ? 1 : 0);
        }

        private static void Highlight(Button[] buttons, int selected)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                bool on = i == selected;
                buttons[i].image.color = on ? UITheme.Amber : ChipIdle;
                buttons[i].GetComponentInChildren<Text>().color = on ? UITheme.Ink : UITheme.TextMuted;
            }
        }
    }
}
